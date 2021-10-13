namespace DotNetToolkit.Repository.Utility
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal static class FastActivator
    {
        #region Fields

        private readonly static ConcurrentDictionary<Tuple<Type, Type[]>, ObjectActivator> _cache = 
            new ConcurrentDictionary<Tuple<Type, Type[]>, ObjectActivator>(
                new TupleComparer());

        #endregion

        #region Delegates

        private delegate object ObjectActivator(params object[] args);

        #endregion

        #region Private Methods

        private static ObjectActivator GetActivator(ConstructorInfo ctor)
        {
            return (ObjectActivator)GetActivatorDelegate(typeof(ObjectActivator), ctor);
        }

        private static Delegate GetActivatorDelegate(Type activatorDelegateType, ConstructorInfo ctor)
        {
            var paramsInfo = ctor.GetParameters();

            var param = Expression.Parameter(typeof(object[]), "args");

            var argsExp = new Expression[paramsInfo.Length];

            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                var paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp = Expression.ArrayIndex(param, index);

                Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            var newExp = Expression.New(ctor, argsExp);

            var lambda = Expression.Lambda(activatorDelegateType, newExp, param);

            return lambda.Compile();
        }

        private static ConstructorInfo GetMatchingConstructor(Type type, object[] args, out Type[] types)
        {
            types = (
                from arg in args
                where arg != null
                select arg.GetType()
                ).ToArray();

            return type.GetConstructor(types);
        }

        #endregion

        #region Public Methods

        public static object CreateInstance(Type type, params object[] args)
        {
            if (type.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(type, args);
            }

            var ctor = GetMatchingConstructor(type, args, out var argTypes);
            if (ctor == null)
            {
                if (args == null || args.Length == 0)
                {
                    throw new InvalidOperationException(
                        string.Format("No default constructor exists for class {0}", type.FullName));
                }

                throw new InvalidOperationException(
                    string.Format("No matching constructor found for class {0}", type.FullName));
            }

            var key = Tuple.Create(type, argTypes);
            if (!_cache.TryGetValue(key, out var activator))
            {
                activator = GetActivator(ctor);

                _cache[key] = activator;
            }

            return activator(args);
        }

        public static T CreateInstance<T>(params object[] args)
        {
            return (T)CreateInstance(typeof(T), args);
        }

        #endregion

        #region Nested Type: TupleComparer

        class TupleComparer : IEqualityComparer<Tuple<Type, Type[]>>
        {
            public bool Equals(object[] x, object[] y)
            {
                return x.Length == y.Length && Enumerable.SequenceEqual(x, y);
            }

            public bool Equals(Tuple<Type, Type[]> x, Tuple<Type, Type[]> y)
            {
                return x.Item1 == y.Item1 && Enumerable.SequenceEqual(x.Item2, y.Item2);
            }

            public int GetHashCode(object[] o)
            {
                var result = o.Aggregate((a, b) => a.GetHashCode() ^ b.GetHashCode());
                return result != null ? result.GetHashCode() : 0;
            }

            public int GetHashCode(Tuple<Type, Type[]> o)
            {
                unchecked
                {
                    int hash = 17;

                    hash = hash * 23 + o.Item1.GetHashCode();

                    foreach (var item in o.Item2)
                    {
                        hash = hash * 23 + ((item != null) ? item.GetHashCode() : 0);
                    }

                    return hash;
                }
            }
        }

        #endregion
    }
}
