namespace DotNetToolkit.Repository.Utility
{
    using JetBrains.Annotations;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Contains various utilities for hashing.
    /// </summary>
    internal static class Hasher
    {
        public static string ComputeMD5([NotNull] string value)
        {
            using (var md5 = MD5.Create())
            {
                return ComputeHashAlgorithm(md5, value);
            }
        }

        private static string ComputeHashAlgorithm([NotNull] HashAlgorithm algorithm, [NotNull] string value)
        {
            Guard.NotNull(algorithm, nameof(algorithm));
            Guard.NotEmpty(value, nameof(value));

            var bytes = Encoding.Unicode.GetBytes(value.ToCharArray());
            var hash = algorithm.ComputeHash(bytes);

            return hash
                .Aggregate(
                    new StringBuilder(32),
                    (sb, b) => sb.Append(b.ToString("x2")))
                .ToString();
        }
    }
}
