namespace DotNetToolkit.Repository.Utility
{
    using Extensions;
    using JetBrains.Annotations;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Utility class to do <c>null</c> and other checks.
    /// </summary>
    [DebuggerStepThrough]
    internal static class Guard
    {
        [NotNull]
        [ContractAnnotation("value:null => halt")]
        public static T NotNull<T>([ValidatedNotNull] [NoEnumeration] T value, [InvokerParameterName] string parameterName = null)
        {
#if NETSTANDARD2_0
            if (typeof(T).IsNullableType() && value == null)
#else
            if (ReferenceEquals(value, null))
#endif
                throw new ArgumentNullException(parameterName);

            return value;
        }

        [NotNull]
        [ContractAnnotation("value:null => halt")]
        public static string NotEmpty([ValidatedNotNull] string value, [InvokerParameterName] string parameterName = null)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);

            if (value.Trim().Length == 0)
                throw new ArgumentException("Value must not be empty.", parameterName);

            return value;
        }

        [NotNull]
        [ContractAnnotation("value:null => halt")]
        public static ICollection<T> NotEmpty<T>([ValidatedNotNull] [NoEnumeration] ICollection<T> value, [InvokerParameterName] string parameterName = null)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);

            if (value.Count == 0)
                throw new ArgumentException("Value must not be empty.", parameterName);

            return value;
        }

        [NotNull]
        [ContractAnnotation("value:null => halt")]
        public static T EnsureNotNull<T>([ValidatedNotNull] [NoEnumeration] T value, string message, params object[] args) where T : class
        {
#if NETSTANDARD2_0
            if (typeof(T).IsNullableType() && value == null)
#else
            if (ReferenceEquals(value, null))
#endif
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, message, args));

            return value;
        }
    }
}
