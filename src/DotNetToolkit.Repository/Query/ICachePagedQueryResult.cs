namespace DotNetToolkit.Repository.Query
{
    /// <summary>
    /// Represents an internal cache paged query result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface ICachePagedQueryResult<out TResult> : ICacheQueryResult<IPagedQueryResult<TResult>>
    {
    }
}
