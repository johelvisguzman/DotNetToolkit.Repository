namespace DotNetToolkit.Repository.Extensions
{
    using JetBrains.Annotations;
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

        public static string ToSHA256([NotNull] this string value)
        {
            Guard.NotEmpty(value, nameof(value));

            var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashInBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
            var sb = new StringBuilder();

            foreach (var b in hashInBytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
