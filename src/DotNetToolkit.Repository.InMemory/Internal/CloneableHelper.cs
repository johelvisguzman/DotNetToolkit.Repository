namespace DotNetToolkit.Repository.InMemory.Internal
{
    using Extensions.Internal;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Utility;

    internal static class CloneableHelper
    {
        private readonly static ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> _cache = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();

        public static object DeepCopy(object entity)
        {
            Guard.NotNull(entity, nameof(entity));

            var type = entity.GetType();
            var newItem = FastActivator.CreateInstance(type);

            if (!_cache.TryGetValue(type, out IEnumerable<PropertyInfo> properties))
            {
                properties = type.GetRuntimeProperties().Where(x => x.IsPrimitive());

                _cache[type] = properties;
            }

            foreach (var propInfo in properties)
            {
                if (propInfo.CanWrite)
                    propInfo.SetValue(newItem, propInfo.GetValue(entity, null), null);
            }

            return newItem;
        }
    }
}
