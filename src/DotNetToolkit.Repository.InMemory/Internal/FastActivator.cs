namespace DotNetToolkit.Repository.InMemory.Internal
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;
    using System.Reflection;

    internal static class FastActivator
    {
        private readonly static ConcurrentDictionary<Type, Func<object>> _cache = new ConcurrentDictionary<Type, Func<object>>();

        public static T CreateInstance<T>() where T : class
        {
            return (T)CreateInstance(typeof(T));
        }

        public static object CreateInstance(Type type)
        {
            if (!type.GetTypeInfo().IsClass)
                return Activator.CreateInstance(type);

            if (!_cache.TryGetValue(type, out Func<object> f))
            {
                f = Expression.Lambda<Func<object>>(Expression.New(type)).Compile();

                _cache[type] = f;
            }

            return f();
        }
    }
}
