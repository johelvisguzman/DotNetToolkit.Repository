namespace DotNetToolkit.Repository.InMemory.Internal
{
    using Extensions;
    using System;
    using System.Linq;
    using System.Reflection;
    using Utility;

    internal static class CloneableHelper
    {
        public static object DeepCopy(object entity)
        {
            Guard.NotNull(entity, nameof(entity));

            var newItem = Activator.CreateInstance(entity.GetType());
            var properties = entity.GetType().GetRuntimeProperties().Where(x => x.IsPrimitive());

            foreach (var propInfo in properties)
            {
                if (propInfo.CanWrite)
                    propInfo.SetValue(newItem, propInfo.GetValue(entity, null), null);
            }

            return newItem;
        }
    }
}
