namespace DotNetToolkit.Repository.Extensions
{
    using JetBrains.Annotations;
    using System.Reflection;
    using Utility;

    internal static class PropertyInfoExtensions
    {
        /// <summary>
        /// Determines if the specified property is a complex type.
        /// </summary>
        /// <param name="pi">The property info.</param>
        /// <returns><c>true</c> if the specified type is a complex type; otherwise, <c>false</c>.</returns>
        public static bool IsComplex([NotNull] this PropertyInfo pi)
        {
            return Guard.NotNull(pi, nameof(pi)).PropertyType.Namespace != "System";
        }

        /// <summary>
        /// Determines if the specified property is a primitive type.
        /// </summary>
        /// <param name="pi">The property info.</param>
        /// <returns><c>true</c> if the specified type is a primitive type; otherwise, <c>false</c>.</returns>
        public static bool IsPrimitive([NotNull] this PropertyInfo pi)
        {
            return !IsComplex(pi);
        }
    }
}
