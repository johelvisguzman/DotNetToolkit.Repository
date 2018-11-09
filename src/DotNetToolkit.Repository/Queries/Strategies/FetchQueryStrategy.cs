namespace DotNetToolkit.Repository.Queries.Strategies
{
    using Extensions;
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

        /// <inheritdoc />
        IEnumerable<string> IFetchQueryStrategy<T>.PropertyPaths
        {
            get { return _properties; }
        }

        /// <inheritdoc />
        public IFetchQueryStrategy<T> Fetch(Expression<Func<T, object>> path)
        {
            return Fetch(path.ToIncludeString());
        }

        /// <inheritdoc />
        public IFetchQueryStrategy<T> Fetch(string path)
        {
            _properties.Add(path);
            return this;
        }

        #endregion
    }
}
