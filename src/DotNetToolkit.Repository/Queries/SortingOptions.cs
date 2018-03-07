namespace DotNetToolkit.Repository.Queries
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents a sorting option.
    /// </summary>
    /// <typeparam name="T">The entity type of the repository.</typeparam>
    /// <typeparam name="TKey">The type of the property that is being sorted.</typeparam>
    public class SortingOptions<T, TKey> : IQueryOptions<T>
    {
        #region Fields

        private readonly Expression<Func<T, TKey>> _sorting;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this sorting option is descending.
        /// </summary>
        /// <value>
        /// <c>true</c> if this sorting option is descending; otherwise, <c>false</c>.
        /// </value>
        public bool IsDescending { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SortingOptions{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="sorting">The sorting expression.</param>
        /// <param name="isDescending">if set to <c>true</c> use descending order; otherwise, use ascending.</param>
        public SortingOptions(Expression<Func<T, TKey>> sorting, bool isDescending = false)
        {
            _sorting = sorting;

            IsDescending = isDescending;
        }

        #endregion

        #region Implementation of IQueryOptions<T>

        /// <summary>
        /// Applies the current options to the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The new query with the defined options applied.</returns>
        public virtual IQueryable<T> Apply(IQueryable<T> query)
        {
            if (_sorting != null)
            {
                query = IsDescending
                    ? query.OrderByDescending(_sorting)
                    : query.OrderBy(_sorting);
            }

            return query;
        }

        #endregion
    }
}
