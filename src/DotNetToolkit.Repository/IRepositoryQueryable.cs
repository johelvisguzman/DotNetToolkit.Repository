namespace DotNetToolkit.Repository
{
    using System.Linq;

    /// <summary>
    /// Represents a repository that exposes a <see cref="System.Linq.IQueryable{TEntity}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IRepositoryQueryable<out TEntity> where TEntity : class
    {
        /// <summary>
        /// Returns the entity <see cref="System.Linq.IQueryable{TEntity}" />.
        /// </summary>
        IQueryable<TEntity> AsQueryable();
    }
}
