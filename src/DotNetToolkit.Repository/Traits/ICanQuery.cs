namespace DotNetToolkit.Repository.Traits
{
    using System.Linq;

    /// <summary>
    /// Represents a trait for returning a <see cref="System.Linq.IQueryable{TEntity}" /> from a repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface ICanQuery<out TEntity> where TEntity : class
    {
        /// <summary>
        /// Returns the entity <see cref="System.Linq.IQueryable{TEntity}" />.
        /// </summary>
        IQueryable<TEntity> AsQueryable();
    }
}

namespace DotNetToolkit.Repository.Transactions.Traits
{
    using System.Linq;

    /// <summary>
    /// Represents a trait for returning a <see cref="System.Linq.IQueryable{TEntity}" /> from a unit of work repository.
    /// </summary>
    public interface ICanQuery
    {
        /// <summary>
        /// Returns the entity <see cref="System.Linq.IQueryable{TEntity}" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        IQueryable<TEntity> AsQueryable<TEntity>() where TEntity : class;
    }
}
