namespace DotNetToolkit.Repository.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static class AssemblyExtensions
    {
        // EntityFramework6/src/Common/AssemblyExtensions.cs
        public static IEnumerable<Type> GetAccessibleTypes(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            try
            {
#if NET40
                return assembly.GetTypes();
#else
                return assembly.DefinedTypes.Select(t => t.AsType());
#endif
            }
            catch (ReflectionTypeLoadException ex)
            {
                // The exception is thrown if some types cannot be loaded in partial trust.
                // For our purposes we just want to get the types that are loaded, which are
                // provided in the Types property of the exception.
                return ex.Types.Where(t => t != null);
            }
        }
    }
}
