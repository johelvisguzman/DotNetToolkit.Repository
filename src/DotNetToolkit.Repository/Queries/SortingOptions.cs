namespace DotNetToolkit.Repository.Queries
{
    using Helpers;
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents a sorting option.
    /// </summary>
    /// <typeparam name="T">The entity type of the repository.</typeparam>
    /// <typeparam name="TSortKey">The type of the property that is being sorted.</typeparam>
    public class SortingOptions<T, TSortKey> : IQueryOptions<T>
    {
        #region Fields

        private readonly Expression<Func<T, TSortKey>> _sortingExpression;

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
        /// Initializes a new instance of the <see cref="SortingOptions{TEntity, TSortKey}"/> class.
        /// </summary>
        /// <param name="sorting">The sorting expression.</param>
        /// <param name="isDescending">if set to <c>true</c> use descending order; otherwise, use ascending.</param>
        public SortingOptions(Expression<Func<T, TSortKey>> sorting, bool isDescending = false)
        {
            if (sorting == null)
                throw new ArgumentNullException(nameof(sorting));

            _sortingExpression = sorting;
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
            query = IsDescending
                ? query.OrderByDescending(_sortingExpression)
                : query.OrderBy(_sortingExpression);

            return query;
        }

        #endregion
    }

    /// <summary>
    /// Represents a sorting option.
    /// </summary>
    /// <typeparam name="T">The entity type of the repository.</typeparam>
    public class SortingOptions<T> : IQueryOptions<T>
    {
        #region Fields

        private readonly string _sorting;

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
        /// Initializes a new instance of the <see cref="SortingOptions{TEntity}"/> class.
        /// </summary>
        /// <param name="sorting">The sorting expression.</param>
        /// <param name="isDescending">if set to <c>true</c> use descending order; otherwise, use ascending.</param>
        public SortingOptions(string sorting, bool isDescending = false)
        {
            _sorting = sorting;
            IsDescending = isDescending;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortingOptions{TEntity}"/> class.
        /// </summary>
        /// <param name="isDescending">if set to <c>true</c> use descending order; otherwise, use ascending.</param>
        public SortingOptions(bool isDescending = false) : this(null, isDescending)
        {
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
            if (!string.IsNullOrEmpty(_sorting))
            {
                query = IsDescending
                        ? query.OrderByDescending(_sorting)
                        : query.OrderBy(_sorting);
            }
            else
            {
                var primaryKeyPropertyInfo = ConventionHelper.GetPrimaryKeyPropertyInfo(typeof(T));
                var primaryKeyPropertyName = primaryKeyPropertyInfo.Name;

                query = IsDescending
                    ? query.OrderByDescending(primaryKeyPropertyName)
                    : query.OrderBy(primaryKeyPropertyName);
            }

            return query;
        }

        #endregion
    }
}
