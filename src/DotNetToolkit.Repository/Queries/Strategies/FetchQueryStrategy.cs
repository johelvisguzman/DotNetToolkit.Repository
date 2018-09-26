namespace DotNetToolkit.Repository.Queries.Strategies
{
    using Helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// An implementation of <see cref="IFetchQueryStrategy{T}" />.
    /// </summary>
    public class FetchQueryStrategy<T> : IFetchQueryStrategy<T>
    {
        #region Fields

        private readonly IList<string> _properties;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FetchQueryStrategy{T}" /> class.
        /// </summary>
        public FetchQueryStrategy()
        {
            _properties = new List<string>();
        }

        #endregion

        #region Implementation of IFetchQueryStrategy<T>

        /// <summary>
        /// Gets the collection of related objects to include in the query results.
        /// </summary>
        IEnumerable<string> IFetchQueryStrategy<T>.IncludePaths
        {
            get { return _properties; }
        }

        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="path">A lambda expression representing the path to include.</param>
        /// <returns>A new <see cref="IFetchQueryStrategy{T}" /> with the defined query path.</returns>
        public IFetchQueryStrategy<T> Include(Expression<Func<T, object>> path)
        {
            return Include(path.ToIncludeString());
        }

        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="path">The dot-separated list of related objects to return in the query results.</param>
        /// <returns>A new <see cref="IFetchQueryStrategy{T}" /> with the defined query path.</returns>
        public IFetchQueryStrategy<T> Include(string path)
        {
            _properties.Add(path);
            return this;
        }

        #endregion
    }
}
