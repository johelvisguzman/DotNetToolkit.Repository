namespace DotNetToolkit.Repository.Test.Helpers
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class ExpressionHelper
    {
        public static PropertyInfo GetPropertyInfo<T>(Expression<Func<T, object>> propertyExpression)
        {
            var memberExpr = propertyExpression.Body as MemberExpression;
            if (memberExpr == null)
                return null;

            return (PropertyInfo)memberExpr.Member;
        }
    }
}
