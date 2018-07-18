namespace DotNetToolkit.Repository.Helpers
{
    using Configuration;
    using Properties;
    using System;

    internal static class RepositoryContextHelper
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
