namespace DotNetToolkit.Repository.Utility
{
    using Extensions.Internal;
    using JetBrains.Annotations;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Utility class to do <c>null</c> and other checks.
    /// </summary>
    [DebuggerStepThrough]
    internal static class Guard
    {
        [NotNull]
        [ContractAnnotation("value:null => halt")]
        public static T NotNull<T>([ValidatedNotNull] [NoEnumeration] T value, [InvokerParameterName] string parameterName)
        {
#if NETSTANDARD2_0
            if (typeof(T).IsNullableType() && value == null)
#else
            if (ReferenceEquals(value, null))
#endif
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        [NotNull]
        [ContractAnnotation("value:null => halt")]
        public static string NotEmpty([ValidatedNotNull] string value, [InvokerParameterName] string parameterName)
        {
            Exception e = null;
            if (value is null)
            {
                e = new ArgumentNullException(parameterName);
            }
            else if (value.Trim().Length == 0)
            {
                e = new ArgumentException("Value must not be empty.", parameterName);
            }

            if (e != null)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw e;
            }

            return value;
        }

        [NotNull]
        [ContractAnnotation("value:null => halt")]
        public static ICollection<T> NotEmpty<T>([ValidatedNotNull] [NoEnumeration] ICollection<T> value, [InvokerParameterName] string parameterName)
        {
            NotNull(value, parameterName);

            if (value.Count == 0)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException("Value must not be empty.", parameterName);
            }

            return value;
        }

        [NotNull]
        [ContractAnnotation("value:null => halt")]
        public static T EnsureNotNull<T>([ValidatedNotNull] [NoEnumeration] T value, string message) where T : class
        {
#if NETSTANDARD2_0
            if (typeof(T).IsNullableType() && value == null)
#else
            if (ReferenceEquals(value, null))
#endif
                throw new InvalidOperationException(message);

            return value;
        }
    }
}
