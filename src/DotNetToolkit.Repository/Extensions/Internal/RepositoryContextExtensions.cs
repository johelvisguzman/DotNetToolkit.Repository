namespace DotNetToolkit.Repository.Extensions.Internal
{
    using Configuration;
    using JetBrains.Annotations;
    using Repository.Internal;
    using Utility;

    internal static class RepositoryContextExtensions
    {
        public static IRepositoryContextAsync AsAsync([NotNull] this IRepositoryContext source)
        {
            return new RepositoryContextAsyncWrapper(Guard.NotNull(source, nameof(source)));
        }
    }
}
