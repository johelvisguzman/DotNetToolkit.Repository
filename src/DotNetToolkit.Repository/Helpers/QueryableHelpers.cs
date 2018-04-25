namespace DotNetToolkit.Repository.Helpers
{
    using System;
    using System.Linq;
    using System.Reflection;

    internal static class QueryableHelpers
    {
        private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string propertyName, string methodName)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var lambda = ExpressionHelper.GetExpression<T>(propertyName);
            var type = ExpressionHelper.GetMemberExpression(lambda).Type;

            var result = typeof(Queryable)
                .GetRuntimeMethods()
                .Single(method => method.Name == methodName &&
                                  method.IsGenericMethodDefinition &&
                                  method.GetGenericArguments().Length == 2 &&
                                  method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { source, lambda });

            return (IOrderedQueryable<T>)result;
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return ApplyOrder<T>(source, propertyName, "OrderBy");
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return ApplyOrder<T>(source, propertyName, "OrderByDescending");
        }
    }
}
