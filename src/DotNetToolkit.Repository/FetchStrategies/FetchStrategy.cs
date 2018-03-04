namespace DotNetToolkit.Repository.FetchStrategies
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// An implementation of <see cref="IFetchStrategy{T}" />.
    /// </summary>
    public class FetchStrategy<T> : IFetchStrategy<T>
    {
        #region Fields

        private readonly IList<string> _properties;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FetchStrategy{T}" /> class.
        /// </summary>
        public FetchStrategy()
        {
            _properties = new List<string>();
        }

        #endregion

        #region IFetchStrategy<T> Members

        /// <summary>
        /// Gets the collection of related objects to include in the query results.
        /// </summary>
        public IEnumerable<string> IncludePaths
        {
            get { return _properties; }
        }

        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="path">A lambda expression representing the path to include.</param>
        /// <returns>A new <see cref="IFetchStrategy{T}" /> with the defined query path.</returns>
        public IFetchStrategy<T> Include(Expression<Func<T, object>> path)
        {
            return Include(path.ToIncludeString());
        }

        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="path">The dot-separated list of related objects to return in the query results.</param>
        /// <returns>A new <see cref="IFetchStrategy{T}" /> with the defined query path.</returns>
        public IFetchStrategy<T> Include(string path)
        {
            _properties.Add(path);
            return this;
        }

        #endregion
    }
}
