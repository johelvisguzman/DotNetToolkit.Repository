namespace DotNetToolkit.Repository.Extensions
{
    using Configuration;
    using Internal;
    using JetBrains.Annotations;
    using Utility;

    internal static class RepositoryContextExtensions
    {
        public static IRepositoryContextAsync AsAsync([NotNull] this IRepositoryContext source)
        {
            return new RepositoryContextAsyncWrapper(Guard.NotNull(source));
        }
    }
}
