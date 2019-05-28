﻿namespace DotNetToolkit.Repository.Extensions.Internal
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
            Guard.NotNull(type, nameof(type));

            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// Determines whether or not the specified type is a collection type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns><c>true</c> if the specified type is a <see cref="ICollection{T}"/>; otherwise, <c>false</c>.</returns>
        public static bool IsGenericCollection([NotNull] this Type type)
        {
            Guard.NotNull(type, nameof(type));

            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>);
        }

        /// <summary>
        /// Determines whether if the specified type is nullable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is nullable; otherwise, <c>false</c>.</returns>
        public static bool IsNullableType([NotNull] this Type type)
        {
            Guard.NotNull(type, nameof(type));

            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Determines whether this instance is enumerable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is enumerable; otherwise, <c>false</c>.</returns>
        public static bool IsEnumerable([NotNull] this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(Guard.NotNull(type, nameof(type)));
        }

        /// <summary>
        /// Determines whether or not the specified type implements the specified interface type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="interfaceType">The interface type to check</param>
        /// <returns><c>true</c> if specified type implements the specified interface type; otherwise, <c>false</c>.</returns>
        public static bool ImplementsInterface([NotNull] this Type type, [NotNull] Type interfaceType)
        {
            Guard.NotNull(type, nameof(type));
            Guard.NotNull(interfaceType, nameof(interfaceType));

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
            Guard.NotNull(type, nameof(type));
            Guard.NotNull(genericType, nameof(genericType));

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
            Guard.NotNull(type, nameof(type));

            object Result = null;

            if (type == typeof(string))
            {
                Result = value;
            }
            else if (type == typeof(int))
            {
                Result = Convert.ToInt32(value);
            }
            else if (type == typeof(byte))
            {
                Result = Convert.ToByte(value);
            }
            else if (type == typeof(decimal))
            {
                Result = Convert.ToDecimal(value);
            }
            else if (type == typeof(double))
            {
                Result = Convert.ToDouble(value);
            }
            else if (type == typeof(bool))
            {
                Result = value.ToBoolean();
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
                    throw new Exception($"Type conversion not handled in {nameof(ConvertTo)} method.");
                }
#else
                return Convert.ChangeType(value, type);
#endif
            }

            return Result;
        }

        /// <summary>
        /// Creates a new instance of the specified type with a constructor that best matches the collection of specified parameters; otherwise, creates an instance of the specified type using that type's default constructor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="keyValues">The key value parameters.</param>
        /// <returns>The new instance of the specified type.</returns>
        public static object InvokeConstructor([NotNull] this Type type, [CanBeNull] Dictionary<string, string> keyValues)
        {
            Guard.NotNull(type, nameof(type));

            if (keyValues == null || keyValues.Count == 0)
                return Activator.CreateInstance(type);

            var kvs = keyValues.ToDictionary(x => x.Key, x => x.Value);
            var ctors = type.GetConstructors();
            var ctorsParams = ctors.ToDictionary(x => x, x => x.GetParameters());
            var keys = kvs.Keys;

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

            object obj;

            if (matchedCtorParams.Key != null)
            {
                // Try to get all the values for the parameters we already have,
                // and set the rest to their default value
                var args = new List<object>();

                args.AddRange(matchedCtorParams.Value.Select(pi =>
                {
                    // If we find a matching parameter, then delete it from the collection,
                    // that way we don't try to initialize a property that has the same name
                    if (kvs.ContainsKey(pi.Name))
                    {
                        kvs.TryGetValue(pi.Name, out var value);
                        kvs.Remove(pi.Name);

                        return pi.ParameterType.ConvertTo(value);
                    }

                    return pi.ParameterType.GetDefault();
                }));

                obj = matchedCtorParams.Key.Invoke(args.ToArray());
            }
            else
            {
                obj = Activator.CreateInstance(type);
            }

            if (kvs.Any())
            {
                // Try to initialize properties that match
                type.GetRuntimeProperties()
                    .Where(x => x.CanWrite && x.GetSetMethod(nonPublic: true).IsPublic)
                    .Join(kvs,
                        pi => pi.Name,
                        kv => kv.Key,
                        (pi, kv) => new
                        {
                            PropertyInfo = pi,
                            Value = pi.PropertyType.ConvertTo(kv.Value)
                        })
                    .ToList()
                    .ForEach(q => q.PropertyInfo.SetValue(obj, q.Value));
            }

            return obj;
        }
    }
}