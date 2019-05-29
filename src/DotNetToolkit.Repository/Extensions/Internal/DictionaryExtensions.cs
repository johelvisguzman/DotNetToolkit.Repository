namespace DotNetToolkit.Repository.Extensions.Internal
{
    using JetBrains.Annotations;
    using System.Collections.Generic;
    using System.Linq;

    internal static class DictionaryExtensions
    {
        public static string ToDebugString<TKey, TValue>([CanBeNull] this IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null || dictionary.Count == 0)
                return "{ }";

            return "{ " + string.Join(", ", dictionary.Select(kv =>
            {
                if (kv.Value is string)
                    return kv.Key + " = '" + kv.Value + "'";
                return kv.Key + " = " + kv.Value;
            }).ToArray()) + " }";
        }
    }
}
