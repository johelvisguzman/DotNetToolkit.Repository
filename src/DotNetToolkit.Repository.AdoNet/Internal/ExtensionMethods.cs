namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using System;

    internal static class ExtensionMethods
    {
        public static string Between(this string value, string a, string b)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            var posA = value.IndexOf(a, StringComparison.Ordinal);
            var posB = value.LastIndexOf(b, StringComparison.Ordinal);
            if (posA == -1 || posB == -1)
                return null;

            var adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
                return null;

            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }
    }
}
