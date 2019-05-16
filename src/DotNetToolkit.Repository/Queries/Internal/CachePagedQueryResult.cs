namespace DotNetToolkit.Repository.Queries.Internal
{
    using JetBrains.Annotations;

    /// <summary>
    /// An implementation of <see cref="ICachePagedQueryResult{TResult}" />.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    internal class CachePagedQueryResult<TResult> : CacheQueryResult<IPagedQueryResult<TResult>>, ICachePagedQueryResult<TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CachePagedQueryResult{TResult}"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public CachePagedQueryResult([CanBeNull] IPagedQueryResult<TResult> result) : base(result) { }
    }
}
