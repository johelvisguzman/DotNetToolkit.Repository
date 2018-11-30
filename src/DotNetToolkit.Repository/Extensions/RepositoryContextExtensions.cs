namespace DotNetToolkit.Repository.Extensions
{
    using Configuration;
    using Internal;
    using System;

    internal static class RepositoryContextExtensions
    {
        public static IRepositoryContextAsync AsAsync(this IRepositoryContext source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            
            return new RepositoryContextAsyncWrapper(source);
        }
    }
}
