﻿namespace DotNetToolkit.Repository.Extensions.Internal
{
    using JetBrains.Annotations;
    using Query.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using Utility;

    // https://github.com/SharpRepository/SharpRepository/tree/master/SharpRepository.Repository/FetchStrategies/FetchQueryStrategyExtensions.cs
    internal static class FetchQueryStrategyExtensions
    {
        /// <summary>
        ///  Evaluates the Linq expression and returns the name of the property or the multiple level deep string representation of the Expression (i.e. prop.Collection.Property).
        /// </summary>
        /// <typeparam name="T">Type being evaluated</typeparam>
        /// <param name="selector">Name of the property per the Linq expression</param>
        /// <returns></returns>
        public static string ToIncludeString<T>([NotNull] this Expression<Func<T, object>> selector)
        {
            Guard.NotNull(selector, nameof(selector));

            // Retrieve member path:
            var members = new List<PropertyInfo>();
            ExpressionHelper.CollectRelationalMembers(selector, members);

            // Build string path:
            var sb = new StringBuilder();
            var separator = "";
            foreach (var member in members)
            {
                sb.Append(separator);
                sb.Append(member.Name);
                separator = ".";
            }

            // return concatenated string
            return sb.ToString();
        }

        /// <summary>
        /// Converts the specified paths to <see cref="IFetchQueryStrategy{T}" />.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="paths">A collection of lambda expressions representing the paths to include.</param>
        /// <returns>The new fetch strategy options instance.</returns>
        public static IFetchQueryStrategy<T> ToFetchQueryStrategy<T>([NotNull] this string[] paths)
        {
            Guard.NotNull(paths, nameof(paths));

            var fetchStrategy = new FetchQueryStrategy<T>();

            if (paths.Length > 0)
            {
                foreach (var path in paths)
                {
                    fetchStrategy.Fetch(path);
                }
            }

            return fetchStrategy;
        }

        /// <summary>
        /// Converts the specified paths to <see cref="IFetchQueryStrategy{T}" />.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="paths">A collection of lambda expressions representing the paths to include.</param>
        /// <returns>The new fetch strategy options instance.</returns>
        public static IFetchQueryStrategy<T> ToFetchQueryStrategy<T>([NotNull] this Expression<Func<T, object>>[] paths)
        {
            Guard.NotNull(paths, nameof(paths));

            var fetchStrategy = new FetchQueryStrategy<T>();

            if (paths.Length > 0)
            {
                foreach (var path in paths)
                {
                    fetchStrategy.Fetch(path);
                }
            }

            return fetchStrategy;
        }
    }
}
