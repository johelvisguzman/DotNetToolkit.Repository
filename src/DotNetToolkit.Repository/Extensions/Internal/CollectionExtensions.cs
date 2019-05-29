namespace DotNetToolkit.Repository.Extensions.Internal
{
    using JetBrains.Annotations;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Utility;

    internal static class CollectionExtensions
    {
        public static string ToConcatenatedString<T>([NotNull] this IEnumerable<T> source, [NotNull] Func<T, string> selector, [NotNull] string separator)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(selector, nameof(selector));
            Guard.NotEmpty(separator, nameof(separator));

            var b = new StringBuilder();
            bool needSeparator = false;

            foreach (var item in source)
            {
                if (needSeparator)
                    b.Append(separator);

                b.Append(selector(item));
                needSeparator = true;
            }

            return b.ToString();
        }

        public static LinkedList<T> ToLinkedList<T>([NotNull] this IEnumerable<T> source)
        {
            Guard.NotNull(source, nameof(source));

            return new LinkedList<T>(source);
        }
    }
}
