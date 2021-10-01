namespace DotNetToolkit.Repository.Utility
{
    using Configuration.Conventions.Internal;
    using Extensions.Internal;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal class FetchHelper<T>
    {
        #region Fields

        private readonly Func<Type, IEnumerable<object>> _innerQueryCallback;
        private readonly Dictionary<string, bool> _fetchedQueries = new Dictionary<string, bool>();
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

        public IEnumerable<T> Include(IEnumerable<T> query, string path)
        {
            Guard.NotNull(query, nameof(query));
            Guard.NotNull(path, nameof(path));

            var pathList = path.Split('.');
            var currentPath = pathList[pathList.Length - 1];
            var firstPath = pathList[0];
            var parentFullPath = GetParentPath(path);

            // Prevent from trying to fetch a navigation property without fetching it's parent first
            if (pathList.Length > 1 && (_fetchedQueries.Any() && !_fetchedQueries.ContainsKey(parentFullPath)))
            {
                return query;
            }

            var firstPi = typeof(T).GetProperty(firstPath);
            var lastPi = GetPropertyInfos(parentFullPath).Last();
            var lastPiType = lastPi.PropertyType.GetGenericTypeOrDefault();

            IEnumerable<object> outerQuery;

            if (pathList.Length > 1)
            {
                var currentPi = lastPiType.GetProperty(currentPath);
                var currentPiType = currentPi.PropertyType.GetGenericTypeOrDefault();
                var currentQuery = GetQuery(currentPiType);
                var lastQuery = GetQuery(query, parentFullPath); // should include fetched entities

                outerQuery = Join(lastQuery, currentQuery, currentPi);

                if (firstPath != parentFullPath)
                {
                    outerQuery = GetPropertyInfosReverse(parentFullPath)
                        .Aggregate(outerQuery, (current, pi) => current.Select(o => pi.GetSafeValue(o)));
                }
            }
            else
            {
                outerQuery = GetQuery(lastPiType);
            }

            _fetchedQueries[path] = true;

            // Join with main query
            query = Join(query.Cast<object>(), outerQuery, firstPi).Cast<T>();

            return query;
        }

        #endregion

        #region Private Methods

        private IEnumerable<object> GetQuery(IEnumerable<T> query, string path)
        {
            return GetPropertyInfos(path)
                .Aggregate(query.Cast<object>(), (current, pi) =>
                {
                    return pi.PropertyType.IsGenericCollection()
                        // Needs to flatten the collection query
                        ? current.Select(o => pi.GetSafeValue(o))
                            .Cast<IEnumerable<object>>()
                            .SelectMany(x => x)
                        : current.Select(o => pi.GetSafeValue(o));
                });
        }

        private IEnumerable<object> GetQuery(Type type)
        {
            return _innerQueryCallback(type);
        }

        private static string GetParentPath(string path)
        {
            // if path = TableA.TableB.TableC
            // then parentPath = TableA.TableB
            var i = path.LastIndexOf('.');
            return i >= 0 ? path.Substring(0, i) : path;
        }

        private static PropertyInfo[] GetPropertyInfos(string path)
        {
            // assumes we are able to get to the end of starting from the main type
            // this means that all properties need to link to each other somehow
            var pathList = path.Split('.');
            var piList = new PropertyInfo[pathList.Length];

            PropertyInfo pi = typeof(T).GetProperty(pathList[0]);
            piList[0] = pi;

            for (int i = 1; i < pathList.Length; i++)
            {
                pi = pi.PropertyType.GetProperty(pathList[i]);
                piList[i] = pi;
            }

            return piList;
        }

        private static PropertyInfo[] GetPropertyInfosReverse(string path)
        {
            var piList = GetPropertyInfos(path);
            var result = new List<PropertyInfo>();
            var pathListReverse = path.Split('.').Reverse().ToArray();

            for (int i = 0; i < pathListReverse.Length; i++)
            {
                bool found = false;

                for (int j = 0; j < piList.Length - i; j++)
                {
                    if (piList[j + 1].Name == pathListReverse[i])
                    {
                        var pi = piList[j + 1].PropertyType
                            .GetGenericTypeOrDefault()
                            .GetProperty(piList[j].Name);

                        result.Add(pi);

                        found = true;

                        break;
                    }
                }

                if (!found) break;
            }

            return result.ToArray();
        }

        private static IEnumerable<object> Join(IEnumerable<object> outer, IEnumerable<object> inner, PropertyInfo rightNavPi)
        {
            // Only do a join when the primary table has a foreign key property for the join table
            var fkConventionResult = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(rightNavPi);
            if (fkConventionResult != null)
            {
                var leftNavPi = fkConventionResult.LeftNavPi;
                var leftKeysToJoinOn = fkConventionResult.LeftKeysToJoinOn;
                var rightKeysToJoinOn = fkConventionResult.RightKeysToJoinOn;
                var rightPiType = rightNavPi.PropertyType.TryGetGenericTypeOrDefault(out bool isRightPiCollection);

                // key selector functions
                object[] outerKeySelectorFunc(object o)
                {
                    return leftKeysToJoinOn.Select(pi => pi.GetSafeValue(o)).ToArray();
                }

                object[] innerKeySelectorFunc(object i)
                {
                    return rightKeysToJoinOn.Select(pi => pi.GetSafeValue(i)).ToArray();
                }

                if (isRightPiCollection)
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
                                }), rightPiType);
                                // Sets the join table property in the main table
                                rightNavPi.SetValue(o, items);
                            }

                            return o;
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
                            // Sets the join table property in the main table
                            rightNavPi.SetValue(o, i);
                        }

                        return o;
                    }, _comparer);
            }
            return outer;
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
    }
}
