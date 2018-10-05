namespace DotNetToolkit.Repository.Extensions
{
    using System;
    using System.Reflection;

    internal static class PropertyInfoExtensions
    {
        /// <summary>
        /// Determines if the specified property is a complex type.
        /// </summary>
        /// <param name="pi">The property info.</param>
        /// <returns><c>true</c> if the specified type is a complex type; otherwise, <c>false</c>.</returns>
        public static bool IsComplex(this PropertyInfo pi)
        {
            if (pi == null)
                throw new ArgumentNullException(nameof(pi));

            return pi.PropertyType.Namespace != "System";
        }

        /// <summary>
        /// Determines if the specified property is a primitive type.
        /// </summary>
        /// <param name="pi">The property info.</param>
        /// <returns><c>true</c> if the specified type is a primitive type; otherwise, <c>false</c>.</returns>
        public static bool IsPrimitive(this PropertyInfo pi)
        {
            if (pi == null)
                throw new ArgumentNullException(nameof(pi));

            return !IsComplex(pi);
        }
    }
}
