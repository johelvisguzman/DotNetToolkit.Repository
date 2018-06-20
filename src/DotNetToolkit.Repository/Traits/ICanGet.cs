namespace DotNetToolkit.Repository.Traits
{
    using FetchStrategies;
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents a trait for getting items from a repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the primary key.</typeparam>
    public interface ICanGet<TEntity, in TKey> where TEntity : class
    {
        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <return>The entity found.</return>
        TEntity Get(TKey key);

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        TEntity Get(TKey key, IFetchStrategy<TEntity> fetchStrategy);

        /// <summary>
        /// Gets a specific projected entity result with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        TResult Get<TResult>(TKey key, Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Gets a specific projected entity result with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        TResult Get<TResult>(TKey key, Expression<Func<TEntity, TResult>> selector, IFetchStrategy<TEntity> fetchStrategy);

        /// <summary>
        /// Determines whether the repository contains an entity with the given primary key value
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        bool Exists(TKey key);
    }

    /// <summary>
    /// Represents a trait for getting items from a repository.
    /// </summary>
    public interface ICanGet
    {
        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <return>The entity found.</return>
        TEntity Get<TEntity>(object key) where TEntity : class;

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        TEntity Get<TEntity>(object key, IFetchStrategy<TEntity> fetchStrategy) where TEntity : class;

        /// <summary>
        /// Gets a specific projected entity result with the given primary key value in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        TResult Get<TEntity, TResult>(object key, Expression<Func<TEntity, TResult>> selector) where TEntity : class;

        /// <summary>
        /// Gets a specific projected entity result with the given primary key value in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        TResult Get<TEntity, TResult>(object key, Expression<Func<TEntity, TResult>> selector, IFetchStrategy<TEntity> fetchStrategy) where TEntity : class;
    }
}