namespace DotNetToolkit.Repository.Queries
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents a paging option.
    /// </summary>
    /// <typeparam name="T">The entity type of the repository.</typeparam>
    /// <typeparam name="TSortKey">The type of the property that is being sorted.</typeparam>
    public class PagingOptions<T, TSortKey> : SortingOptions<T, TSortKey>
    {
        #region Fields

        private const int DefaultPageSize = 100;
        private int _pageIndex;
        private int _pageSize;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the number of rows of the data page to retrieve.
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException($"{nameof(PageSize)} cannot be lower than zero.");

                _pageSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the zero-based index of the data page to retrieve.
        /// </summary>
        public int PageIndex
        {
            get { return _pageIndex; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(PageIndex), "Cannot be lower than 1.");

                _pageIndex = value;
            }
        }

        /// <summary>
        /// Gets the page count.
        /// </summary>
        public int PageCount { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PagingOptions{TEntity, TSortKey}"/> class.
        /// </summary>
        /// <param name="pageIndex">The zero-based index of the data page to retrieve.</param>
        /// <param name="sorting">The sorting expression.</param>
        /// <param name="isDescending">if set to <c>true</c> use descending order; otherwise, use ascending.</param>
        public PagingOptions(int pageIndex, Expression<Func<T, TSortKey>> sorting, bool isDescending = false) : base(sorting, isDescending)
        {
            PageIndex = pageIndex;
            PageSize = DefaultPageSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagingOptions{TEntity, TSortKey}"/> class.
        /// </summary>
        /// <param name="pageIndex">The zero-based index of the data page to retrieve.</param>
        /// <param name="pageSize">The number of rows of the data page to retrieve.</param>
        /// <param name="sorting">The sorting expression.</param>
        /// <param name="isDescending">if set to <c>true</c> use descending order; otherwise, use ascending.</param>
        public PagingOptions(int pageIndex, int pageSize, Expression<Func<T, TSortKey>> sorting, bool isDescending = false) : base(sorting, isDescending)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        #endregion

        #region Overrides of SortingOptions<TEntity,TKey>

        /// <summary>
        /// Applies the current options to the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The new query with the defined options applied.</returns>
        public override IQueryable<T> Apply(IQueryable<T> query)
        {
            var pageCount = (int)Math.Ceiling(query.Count() / (decimal)PageSize);

            if (PageIndex > pageCount)
                throw new ArgumentOutOfRangeException(nameof(PageIndex), "Cannot be greater than the total number of pages.");

            PageCount = pageCount;

            query = base.Apply(query);

            return query.Skip((PageIndex - 1) * PageSize).Take(PageSize);
        }

        #endregion
    }

    /// <summary>
    /// Represents a paging option.
    /// </summary>
    /// <typeparam name="T">The entity type of the repository.</typeparam>
    public class PagingOptions<T> : SortingOptions<T>
    {
        #region Fields

        private const int DefaultPageSize = 100;
        private int _pageIndex;
        private int _pageSize;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the number of rows of the data page to retrieve.
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException($"{nameof(PageSize)} cannot be lower than zero.");

                _pageSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the zero-based index of the data page to retrieve.
        /// </summary>
        public int PageIndex
        {
            get { return _pageIndex; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(PageIndex), "Cannot be lower than 1.");

                _pageIndex = value;
            }
        }

        /// <summary>
        /// Gets the page count.
        /// </summary>
        public int PageCount { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PagingOptions{TEntity}"/> class.
        /// </summary>
        /// <param name="pageIndex">The zero-based index of the data page to retrieve.</param>
        /// <param name="sorting">The sorting expression.</param>
        /// <param name="isDescending">if set to <c>true</c> use descending order; otherwise, use ascending.</param>
        public PagingOptions(int pageIndex, string sorting, bool isDescending = false) : base(sorting, isDescending)
        {
            PageIndex = pageIndex;
            PageSize = DefaultPageSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagingOptions{TEntity}"/> class.
        /// </summary>
        /// <param name="pageIndex">The zero-based index of the data page to retrieve.</param>
        /// <param name="pageSize">The number of rows of the data page to retrieve.</param>
        /// <param name="sorting">The sorting expression.</param>
        /// <param name="isDescending">if set to <c>true</c> use descending order; otherwise, use ascending.</param>
        public PagingOptions(int pageIndex, int pageSize, string sorting, bool isDescending = false) : base(sorting, isDescending)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagingOptions{TEntity}"/> class.
        /// </summary>
        /// <param name="pageIndex">The zero-based index of the data page to retrieve.</param>
        /// <param name="pageSize">The number of rows of the data page to retrieve.</param>
        public PagingOptions(int pageIndex, int pageSize) : this(pageIndex, pageSize, null, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagingOptions{TEntity}"/> class.
        /// </summary>
        /// <param name="pageIndex">The zero-based index of the data page to retrieve.</param>
        public PagingOptions(int pageIndex) : this(pageIndex, null, false)
        {
        }

        #endregion

        #region Overrides of SortingOptions<TEntity,TKey>

        /// <summary>
        /// Applies the current options to the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The new query with the defined options applied.</returns>
        public override IQueryable<T> Apply(IQueryable<T> query)
        {
            var pageCount = (int)Math.Ceiling(query.Count() / (decimal)PageSize);

            if (PageIndex > pageCount)
                throw new ArgumentOutOfRangeException(nameof(PageIndex), "Cannot be greater than the total number of pages.");

            PageCount = pageCount;

            query = base.Apply(query);

            return query.Skip((PageIndex - 1) * PageSize).Take(PageSize);
        }

        #endregion
    }
}
