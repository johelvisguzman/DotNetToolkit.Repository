namespace DotNetToolkit.Repository.Utility
{
    using Configuration.Conventions.Internal;
    using Extensions.Internal;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal class FetchHelper
    {
        #region Fields

        private readonly Func<Type, IEnumerable<object>> _innerQueryCallback;
        private static readonly IEqualityComparer<object[]> _comparer = new ObjectArrayComparer();

        private static readonly MethodInfo _castMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast));
        private static readonly MethodInfo _toListMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList));

        #endregion

        #region Constructors

        public FetchHelper(Func<Type, IEnumerable<object>> innerQueryCallback)
        {
            _innerQueryCallback = Guard.NotNull(innerQueryCallback, nameof(innerQueryCallback));
        }

        #endregion

        #region Public Methods

        public IEnumerable<T> Include<T>(IEnumerable<T> query, string path)
        {
            Guard.NotNull(query, nameof(query));
            Guard.NotNull(path, nameof(path));

            IEnumerable<object> outerQuery = null;
            Type pathPropType = null, lastPathPropType = null;
            string pathPropName = null;

            // Join nested nav queries
            //
            // if path = "B.C.D"
            // then, reverse the path to "D.C.B",
            // and join all the way down to the main query:
            // D > C > B > MainQuery
            var pathPropReversedList = GetPropertyInfos<T>(path).Select(x => new { x.Name, x.PropertyType }).Reverse().ToArray();
            for (int i = 0; i < pathPropReversedList.Length; i++)
            {
                var pathProp = pathPropReversedList[i];
                pathPropType = pathProp.PropertyType;
                pathPropName = pathProp.Name;

                var currentQuery = _innerQueryCallback(pathPropType.GetGenericTypeOrDefault());

                outerQuery = i != 0
                    ? Join(outerQuery, lastPathPropType, currentQuery, pathPropType, pathPropName)
                        .Select(x => x.Inner)
                    : currentQuery;

                lastPathPropType = pathPropType;
            }

            // Join with main query
            query = Join(query, typeof(T), outerQuery, pathPropType, pathPropName)
                .Select(x => x.Outer);

            return query;
        }

        #endregion

        #region Private Methods

        private static PropertyInfo[] GetPropertyInfos<T>(string path)
        {
            // assumes we are able to get to the end of starting from the main type
            // this means that all properties need to link to each other somehow
            var pathList = path.Split('.');
            var piList = new PropertyInfo[pathList.Length];

            PropertyInfo pi = typeof(T).GetProperty(pathList[0]);
            piList[0] = pi;

            for (int i = 1; i < pathList.Length; i++)
            {
                pi = pi.PropertyType.GetGenericTypeOrDefault().GetProperty(pathList[i]);
                piList[i] = pi;
            }

            return piList;
        }

        private static IEnumerable<JoinResult<TOuter, object>> Join<TOuter>(IEnumerable<TOuter> outer, Type outerType, IEnumerable<object> inner, Type innerType, string propertyName)
        {
            // Only do a join when the primary table has a foreign key property for the join table
            var innerDefaultType = innerType.TryGetGenericTypeOrDefault(out bool isInnerTypeCollection);
            var fkConventionResult = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(outerType, innerDefaultType, propertyName);
            
            if (fkConventionResult != null)
            {
                var leftNavPi = fkConventionResult.LeftNavPi;
                var leftKeysToJoinOn = fkConventionResult.LeftKeysToJoinOn;
                var rightNavPi = fkConventionResult.RightNavPi;
                var rightKeysToJoinOn = fkConventionResult.RightKeysToJoinOn;
                var isRightTypeCollection = rightNavPi != null ? rightNavPi.PropertyType.IsEnumerable() : isInnerTypeCollection;

                // key selector functions
                object[] outerKeySelectorFunc(TOuter o)
                {
                    return leftKeysToJoinOn.Select(pi => pi.GetSafeValue(o)).ToArray();
                }

                object[] innerKeySelectorFunc(object i)
                {
                    return rightKeysToJoinOn.Select(pi => pi.GetSafeValue(i)).ToArray();
                }

                if (isRightTypeCollection)
                {
                    return outer.GroupJoin(
                        inner,
                        outerKeySelectorFunc,
                        innerKeySelectorFunc,
                        (o, i) =>
                        {
                            if (i != null)
                            {
                                // Cast collection
                                var items = ToList(i.Select(item =>
                                {
                                    if (leftNavPi != null)
                                    {
                                        // Sets the main table property in the join table
                                        leftNavPi.SetValue(item, o);
                                    }

                                    return item;
                                }), innerDefaultType);

                                if (rightNavPi != null)
                                {
                                    // Sets the join table property in the main table
                                    rightNavPi.SetValue(o, items);
                                }
                            }

                            return new JoinResult<TOuter, object>(o, i);
                        }, _comparer);
                }

                return LeftJoin(
                    outer,
                    inner,
                    outerKeySelectorFunc,
                    innerKeySelectorFunc,
                    (o, i) =>
                    {
                        if (i != null)
                        {
                            if (leftNavPi != null)
                            {
                                // Sets the main table property in the join table
                                leftNavPi.SetValue(i, o);
                            }

                            if (rightNavPi != null)
                            {
                                // Sets the join table property in the main table
                                rightNavPi.SetValue(o, i);
                            }
                        }

                        return new JoinResult<TOuter, object>(o, i);
                    }, _comparer);
            }

            return outer.Select(o => new JoinResult<TOuter, object>(o, null));
        }

        private static ICollection ToList(IEnumerable items, Type type)
        {
            var castItems = _castMethod
                .MakeGenericMethod(new Type[] { type })
                .Invoke(null, new object[] { items });

            var list = _toListMethod
                .MakeGenericMethod(new Type[] { type })
                .Invoke(null, new object[] { castItems });

            return (ICollection)list;
        }

        private static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            return outer.GroupJoin(
                    inner,
                    outerKeySelector,
                    innerKeySelector,
                    (o, i) => new { outer = o, innerCollection = i },
                    comparer)
                .SelectMany(
                    a => a.innerCollection.DefaultIfEmpty(),
                    (a, i) => resultSelector(a.outer, i));
        }

        #endregion

        #region Nested Type: ObjectArrayComparer

        class ObjectArrayComparer : IEqualityComparer<object[]>
        {
            public bool Equals(object[] x, object[] y)
            {
                return x.Length == y.Length && Enumerable.SequenceEqual(x, y);
            }

            public int GetHashCode(object[] o)
            {
                var result = o.Aggregate((a, b) => a.GetHashCode() ^ b.GetHashCode());
                return result != null ? result.GetHashCode() : 0;
            }
        }

        #endregion

        #region Nested Type: JoinResult

        class JoinResult<TOuter, TInner>
        {
            public TOuter Outer { get; }
            public TInner Inner { get; }
            public JoinResult(TOuter outer, TInner inner)
            {
                Outer = outer;
                Inner = inner;
            }
        }

        #endregion
    }
}
