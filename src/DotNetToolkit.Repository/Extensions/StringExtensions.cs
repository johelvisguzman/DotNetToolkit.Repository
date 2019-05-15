namespace DotNetToolkit.Repository.Extensions
{
    using JetBrains.Annotations;
    using System;
    using System.Linq;
    using System.Text;
    using Utility;

    internal static class StringExtensions
    {
        public static string Indent([CanBeNull] this string value, int size)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var strArray = value.Split('\n');
            var sb = new StringBuilder();

            foreach (var s in strArray)
            {
                sb.Append('\n');
                sb.Append(new string(' ', size));
                sb.Append(s);
            }

            return sb.ToString().Substring(1);
        }

        public static bool ToBoolean(this string value)
        {
            if (value == null || value.Trim().Length == 0)
                return false;

            switch (value.ToLower())
            {
                case "true":
                    return true;
                case "on":
                    return true;
                case "1":
                    return true;
                case "0":
                    return false;
                case "false":
                    return false;
                case "off":
                    return false;
                default:
                    throw new InvalidCastException($"Cannot cast '{value}' to boolean.");
            }
        }
    }
}
