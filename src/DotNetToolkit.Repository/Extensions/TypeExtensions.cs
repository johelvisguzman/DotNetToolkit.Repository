namespace DotNetToolkit.Repository.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static class TypeExtensions
    {
        /// <summary>
        /// Gets the default value of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The default value of the specified type.</returns>
        public static object GetDefault(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// Determines whether or not the specified type is a collection type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns><c>true</c> if the specified type is a <see cref="ICollection{T}"/>; otherwise, <c>false</c>.</returns>
        public static bool IsGenericCollection(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>);
        }

        /// <summary>
        /// Determines whether this instance is enumerable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is enumerable; otherwise, <c>false</c>.</returns>
        public static bool IsEnumerable(this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines whether or not the specified type implements the specified interface type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="interfaceType">The interface type to check</param>
        /// <returns><c>true</c> if specified type implements the specified interface type; otherwise, <c>false</c>.</returns>
        public static bool ImplementsInterface(this Type type, Type interfaceType)
        {
            return interfaceType.IsAssignableFrom(type) ||
                   type.IsGenericType(interfaceType) ||
                   type.GetTypeInfo().ImplementedInterfaces.Any(@interface => IsGenericType(@interface, interfaceType));
        }

        /// <summary>
        /// Determines whether or not the specified type is a generic type of the specified interface type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="genericType">The generic type to check</param>
        /// <returns><c>true</c> if the specified type is a generic type of the specified interface type; otherwise, <c>false</c>.</returns>
        public static bool IsGenericType(this Type type, Type genericType)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;
        }
    }
}
