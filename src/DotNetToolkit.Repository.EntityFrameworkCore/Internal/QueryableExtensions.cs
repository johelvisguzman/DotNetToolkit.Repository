﻿namespace DotNetToolkit.Repository.EntityFrameworkCore.Internal
{
    using Microsoft.EntityFrameworkCore;
    using Queries;
    using System;
    using System.Linq;

    internal static class QueryableExtensions
    {
        public static IQueryable<T> ApplyFetchingOptions<T>(this IQueryable<T> query, IQueryOptions<T> options) where T : class
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (options?.FetchStrategy != null)
                query = options.FetchStrategy.PropertyPaths.Aggregate(query, (current, path) => current.Include(path));

            return query;
        }
    }
}