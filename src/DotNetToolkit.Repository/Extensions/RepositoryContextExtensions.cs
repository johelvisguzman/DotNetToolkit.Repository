namespace DotNetToolkit.Repository.Extensions
{
    using System;
    using Configuration;
    using Properties;

    internal static class RepositoryContextExtensions
    {
        public static IRepositoryContextAsync AsAsync(this IRepositoryContext source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var async = source as IRepositoryContextAsync;
            if (async != null)
                return async;

            throw new NotSupportedException(Resources.IRepositoryContextNotAsync);
        }
    }
}
