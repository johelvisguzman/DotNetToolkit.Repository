namespace DotNetToolkit.Repository.Extensions
{
    using System.Text;

    internal static class StringExtensions
    {
        public static string Indent(this string value, int size)
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
    }
}
