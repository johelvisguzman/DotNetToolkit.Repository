namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using System.Data.Common;
    using System.Text;

    internal static class DbParameterCollectionExtensions
    {
        public static string ToDebugString(this DbParameterCollection collection)
        {
            if (collection == null || collection.Count == 0)
                return "{ }";

            var sb = new StringBuilder();

            sb.Append("{ ");

            foreach (DbParameter p in collection)
            {
                sb.Append(p.ParameterName + " = " + p.Value);
                sb.Append(", ");
            }

            sb.Remove(sb.Length - 2, 2);

            sb.Append(" }");

            return sb.ToString();
        }
    }
}
