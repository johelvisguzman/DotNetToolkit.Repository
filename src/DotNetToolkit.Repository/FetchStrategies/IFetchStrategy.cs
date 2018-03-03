﻿namespace DotNetToolkit.Repository.FetchStrategies
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// An abstraction which defines the child objects that should be retrieved when loading the entity.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public interface IFetchStrategy<T>
    {
        /// <summary>
        /// Gets the collection of related objects to include in the query results.
        /// </summary>
        IEnumerable<string> IncludePaths { get; }

        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="path">A lambda expression representing the path to include.</param>
        /// <returns>A new <see cref="IFetchStrategy{T}" /> with the defined query path.</returns>
        IFetchStrategy<T> Include(Expression<Func<T, object>> path);

        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="path">The dot-separated list of related objects to return in the query results.</param>
        /// <returns>A new <see cref="IFetchStrategy{T}" /> with the defined query path.</returns>
        IFetchStrategy<T> Include(string path);
    }
}
