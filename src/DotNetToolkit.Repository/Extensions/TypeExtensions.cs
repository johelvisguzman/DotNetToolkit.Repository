namespace DotNetToolkit.Repository.Extensions
{
    using JetBrains.Annotations;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Utility;

    internal static class TypeExtensions
    {
        /// <summary>
        /// Gets the default value of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The default value of the specified type.</returns>
        public static object GetDefault([NotNull] this Type type)
        {
            Guard.NotNull(type);

            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// Determines whether or not the specified type is a collection type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns><c>true</c> if the specified type is a <see cref="ICollection{T}"/>; otherwise, <c>false</c>.</returns>
        public static bool IsGenericCollection([NotNull] this Type type)
        {
            Guard.NotNull(type);

            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>);
        }

        /// <summary>
        /// Determines whether if the specified type is nullable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is nullable; otherwise, <c>false</c>.</returns>
        public static bool IsNullableType([NotNull] this Type type)
        {
            Guard.NotNull(type);

            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Determines whether this instance is enumerable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is enumerable; otherwise, <c>false</c>.</returns>
        public static bool IsEnumerable([NotNull] this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(Guard.NotNull(type));
        }

        /// <summary>
        /// Determines whether or not the specified type implements the specified interface type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="interfaceType">The interface type to check</param>
        /// <returns><c>true</c> if specified type implements the specified interface type; otherwise, <c>false</c>.</returns>
        public static bool ImplementsInterface([NotNull] this Type type, [NotNull] Type interfaceType)
        {
            Guard.NotNull(type);
            Guard.NotNull(interfaceType);

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
        public static bool IsGenericType([NotNull] this Type type, [NotNull] Type genericType)
        {
            Guard.NotNull(type);
            Guard.NotNull(genericType);

            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;
        }

        /// <summary>
        /// Converts the type to the specified string value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value to convert to.</param>
        /// <returns>The converted result.</returns>
        public static object ConvertTo([NotNull] this Type type, [CanBeNull] string value)
        {
            Guard.NotNull(type);

            object Result = null;

            if (type == typeof(string))
            {
                Result = value;
            }
            else if (type == typeof(int))
            {
                Result = int.Parse(value, NumberStyles.Integer, CultureInfo.CurrentCulture);
            }
            else if (type == typeof(byte))
            {
                Result = Convert.ToByte(value);
            }
            else if (type == typeof(decimal))
            {
                Result = decimal.Parse(value, NumberStyles.Any, CultureInfo.CurrentCulture);
            }
            else if (type == typeof(double))
            {
                Result = double.Parse(value, NumberStyles.Any, CultureInfo.CurrentCulture);
            }
            else if (type == typeof(bool))
            {
                if (value.ToLower() == "true" || value.ToLower() == "on" || value == "1")
                    Result = true;
                else
                    Result = false;
            }
            else if (type == typeof(DateTime))
            {
                Result = Convert.ToDateTime(value, CultureInfo.CurrentCulture);
            }
            else if (type.GetTypeInfo().IsEnum)
            {
                Result = Enum.Parse(type, value);
            }
            else
            {
#if !NETSTANDARD1_3
                var converter = System.ComponentModel.TypeDescriptor.GetConverter(type);

                if (converter.CanConvertFrom(typeof(string)))
                {
                    Result = converter.ConvertFromString(value);
                }
                else
                {
                    throw new Exception("Type Conversion not handled in ConvertTo method.");
                }
#else
                return Convert.ChangeType(value, type);
#endif
            }

            return Result;
        }

        internal static object InvokeConstructor([NotNull] this Type type, [CanBeNull] Dictionary<string, string> keyValues)
        {
            Guard.NotNull(type);

            if (keyValues == null || keyValues.Count == 0)
                return Activator.CreateInstance(type);

            var ctors = type.GetConstructors();
            var ctorsParams = ctors.ToDictionary(x => x, x => x.GetParameters());
            var keys = keyValues.Keys;

            // try to find an exact match
            var matchedCtorParams = ctorsParams
                 .FirstOrDefault(ctorParams => ctorParams.Value
                     .Select(x => x.Name)
                     .OrderBy(x => x)
                     .SequenceEqual(keys.OrderBy(x => x)));

            if (matchedCtorParams.Key == null)
            {
                // try to find at a constructor that has the highest count for matching parameters
                var maxMatchedCount = 0;

                foreach (var ctorParams in ctorsParams)
                {
                    // gets the number of matches found
                    var count =
                        (
                            from pi in ctorParams.Value
                            from key in keys
                            where key.Equals(pi.Name)
                            select pi
                        )
                        .Count();

                    if (count > maxMatchedCount)
                    {
                        maxMatchedCount = count;
                        matchedCtorParams = ctorParams;

                        // This is the hightest match count we can possible get
                        // and if so, break out of here
                        if (maxMatchedCount == keys.Count)
                            break;
                    }
                }
            }

            if (matchedCtorParams.Key != null)
            {
                // Try to get all the values for the parameters we already have,
                // and set the rest to their default value
                var args = new List<object>();

                args.AddRange(matchedCtorParams.Value.Select(pi =>
                    keyValues.ContainsKey(pi.Name)
                        ? pi.ParameterType.ConvertTo(keyValues[pi.Name])
                        : pi.ParameterType.GetDefault()));

                // Create instance
                return matchedCtorParams.Key.Invoke(args.ToArray());
            }

            // Try to invoke the default constructor
            return Activator.CreateInstance(type);
        }
    }
}
