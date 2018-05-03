namespace DotNetToolkit.Repository.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Contains various utilities for expressions.
    /// </summary>
    public static class ExpressionHelper
    {
        /// <summary>    
        /// Combines the first predicate with the second using the logical "and".
        /// </summary>    
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>    
        /// Combines the first predicate with the second using the logical "or".
        /// </summary>    
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        /// <summary>    
        /// Negates the predicate.
        /// </summary>    
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            var negated = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
        }

        /// <summary>
        /// Returns an expression property selector for the specified property path.
        /// </summary>
        public static Expression GetExpression<T>(string propertyPath)
        {
            var parts = (propertyPath ?? "").Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            var type = typeof(T);
            var param = Expression.Parameter(type, "x");

            Expression body = param;

            foreach (var pi in parts.Select(prop => type.GetTypeInfo().GetDeclaredProperty(prop)))
            {
                body = Expression.Property(body, pi);
                type = pi.PropertyType;
            }

            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);

            return Expression.Lambda(delegateType, body, param);
        }

        /// <summary>
        /// Returns the value of the property for the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>The value of the property for the specified expression</returns>
        public static object GetPropertyValue(Expression exp)
        {
            var objectMember = Expression.Convert(exp, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();

            return getter();
        }

        /// <summary>
        /// Returns the name of the property for the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>The name of the property for the specified expression</returns>
        public static string GetPropertyName(Expression exp)
        {
            if (exp == null)
                throw new ArgumentNullException(nameof(exp));

            return GetMemberExpression(exp)?.Member.Name;
        }

        /// <summary>
        /// Gets the property path.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns>The property path built from the specified expression</returns>
        public static string GetPropertyPath(Expression exp)
        {
            if (exp == null)
                throw new ArgumentNullException(nameof(exp));

            var stack = new Stack<string>();

            var me = GetMemberExpression(exp);

            while (me != null)
            {
                stack.Push(me.Member.Name);
                me = me.Expression as MemberExpression;
            }

            return string.Join(".", stack.ToArray());
        }

        /// <summary>
        /// Gets the member expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The member expression.</returns>
        public static MemberExpression GetMemberExpression(Expression expression)
        {
            switch (expression)
            {
                case MethodCallExpression expr:
                    return (MemberExpression)expr.Arguments[0];

                case MemberExpression memberExpression:
                    return memberExpression;

                case UnaryExpression unaryExpression:
                    return (MemberExpression)unaryExpression.Operand;

                case BinaryExpression binaryExpression:
                    var binaryExpr = binaryExpression;

                    if (binaryExpr.Left is UnaryExpression left)
                        return (MemberExpression)left.Operand;

                    return (MemberExpression)binaryExpr.Left;

                case LambdaExpression expression1:
                    var lambdaExpression = expression1;

                    switch (lambdaExpression.Body)
                    {
                        case MemberExpression body:
                            return body;
                        case UnaryExpression expressionBody:
                            return (MemberExpression)expressionBody.Operand;
                    }
                    break;
            }

            return null;
        }

        internal static void CollectRelationalMembers(Expression exp, IList<PropertyInfo> members)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    CollectRelationalMembers(((LambdaExpression)exp).Body, members);
                    break;
                case ExpressionType.MemberAccess:
                    var mexp = (MemberExpression)exp;
                    CollectRelationalMembers(mexp.Expression, members);
                    members.Add((PropertyInfo)mexp.Member);

                    break;
                case ExpressionType.Call:
                    var cexp = (MethodCallExpression)exp;

                    if (cexp.Method.IsStatic == false)
                        throw new InvalidOperationException("Invalid type of expression.");

                    foreach (var arg in cexp.Arguments)
                        CollectRelationalMembers(arg, members);

                    break;
                case ExpressionType.Parameter:
                    return;
                default:
                    throw new InvalidOperationException("Invalid type of expression.");
            }
        }

        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // zip parameters (map from parameters of second to parameters of first)    
            var map = first.Parameters
                .Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with the parameters in the first    
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // create a merged lambda expression with parameters from the first expression    
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }
    }

    class ParameterRebinder : ExpressionVisitor
    {
        readonly Dictionary<ParameterExpression, ParameterExpression> map;

        ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;

            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }

            return base.VisitParameter(p);
        }
    }
}
