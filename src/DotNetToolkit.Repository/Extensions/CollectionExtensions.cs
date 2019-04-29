namespace DotNetToolkit.Repository.Extensions
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
            Guard.NotNull(source);
            Guard.NotNull(selector);
            Guard.NotEmpty(separator);

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
            Guard.NotNull(source);

            return new LinkedList<T>(source);
        }
    }
}
