namespace DotNetToolkit.Repository.Utility
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
            if (first == null)
                throw new ArgumentNullException(nameof(first));

            if (second == null)
                throw new ArgumentNullException(nameof(second));

            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>    
        /// Combines the first predicate with the second using the logical "or".
        /// </summary>    
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));

            if (second == null)
                throw new ArgumentNullException(nameof(second));

            return first.Compose(second, Expression.OrElse);
        }

        /// <summary>    
        /// Negates the predicate.
        /// </summary>    
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> exp)
        {
            if (exp == null)
                throw new ArgumentNullException(nameof(exp));

            var negated = Expression.Not(exp.Body);
            return Expression.Lambda<Func<T, bool>>(negated, exp.Parameters);
        }

        /// <summary>
        /// Returns an expression property selector for the specified property path.
        /// </summary>
        public static Expression GetExpression<T>(string propertyPath)
        {
            if (propertyPath == null)
                throw new ArgumentNullException(nameof(propertyPath));

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
        public static object GetExpressionValue(Expression exp)
        {
            if (exp == null)
                throw new ArgumentNullException(nameof(exp));

            if (exp.NodeType == ExpressionType.Constant)
                return ((ConstantExpression)exp).Value;

            var convertToString = false;

            if (exp.NodeType == ExpressionType.Call)
            {
                var methodCallExpression = (MethodCallExpression)exp;

                if (methodCallExpression.Method.Name != "ToString")
                    throw new NotSupportedException(methodCallExpression.Method.Name + " isn't supported");

                convertToString = true;

                var expr1 = methodCallExpression.Object ?? methodCallExpression.Arguments[0];

                if (expr1.NodeType == ExpressionType.Constant)
                    return ((ConstantExpression)expr1).Value.ToString();
            }

            var memberExpression = GetMemberExpression(exp);

            object value = null;

            if (memberExpression.Expression is ConstantExpression container)
            {
                switch (memberExpression.Member)
                {
                    case FieldInfo fieldInfo:
                        value = fieldInfo.GetValue(container.Value);
                        break;
                    case PropertyInfo propertyInfo:
                        value = propertyInfo.GetValue(container.Value, null);
                        break;
                }
            }
            else if (memberExpression.Expression.NodeType != ExpressionType.Parameter)
            {
                var innerMemberExpression = GetMemberExpression(memberExpression.Expression);

                while (innerMemberExpression.Expression.NodeType == ExpressionType.MemberAccess)
                {
                    innerMemberExpression = (MemberExpression)innerMemberExpression.Expression;
                }

                if (innerMemberExpression.Expression.NodeType != ExpressionType.Parameter)
                {
                    var objectMember = Expression.Convert(memberExpression, typeof(object));
                    var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                    var getter = getterLambda.Compile();

                    value = getter();
                }
            }

            return convertToString && value != null ? value.ToString() : value;
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
        /// Gets the property information.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns>The property info from the specified expression</returns>
        public static PropertyInfo GetPropertyInfo(Expression exp)
        {
            if (exp == null)
                throw new ArgumentNullException(nameof(exp));

            return (PropertyInfo)GetMemberExpression(exp)?.Member;
        }

        /// <summary>
        /// Gets the hashing information for the specified expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>The generated hash.</returns>
        // https://blogs.msdn.microsoft.com/mattwar/2007/08/01/linq-building-an-iqueryable-provider-part-iii/
        public static string TranslateToString(Expression exp)
        {
            if (exp == null)
                throw new ArgumentNullException(nameof(exp));

            // locally evaluate as much of the query as possible
            exp = Evaluator.PartialEval(exp);

            // support local collections
            exp = LocalCollectionExpander.Rewrite(exp);

            return exp.ToString();
        }

        /// <summary>
        /// Gets the member expression.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>The member expression.</returns>
        public static MemberExpression GetMemberExpression(Expression exp)
        {
            if (exp == null)
                throw new ArgumentNullException(nameof(exp));

            switch (exp)
            {
                case MethodCallExpression methodCallExpression:
                    {
                        return (MemberExpression)(methodCallExpression.Object ?? methodCallExpression.Arguments[0]);
                    }
                case MemberExpression memberExpression:
                    {
                        return memberExpression;
                    }
                case UnaryExpression unaryExpression:
                    {
                        return (MemberExpression)unaryExpression.Operand;
                    }
                case BinaryExpression binaryExpression:
                    {
                        if (binaryExpression.Left is UnaryExpression left)
                            return (MemberExpression)left.Operand;

                        return (MemberExpression)binaryExpression.Left;
                    }
                case LambdaExpression lambdaExpression:
                    {
                        switch (lambdaExpression.Body)
                        {
                            case MemberExpression body:
                                return body;
                            case UnaryExpression expressionBody:
                                return (MemberExpression)expressionBody.Operand;
                        }

                        break;
                    }
            }

            return null;
        }

        /// <summary>
        /// Creates a ConstantExpression that has the Value property set to the specified expression's value.
        /// </summary>
        /// <param name="exp">The expression.</param>
        /// <returns>The constant expression.</returns>
        internal static ConstantExpression AsConstantExpression(this Expression exp)
        {
            if (exp == null)
                throw new ArgumentNullException(nameof(exp));

            if (exp is ConstantExpression constantExpression)
                return constantExpression;

            var value = GetExpressionValue(exp);

            return value != null ? Expression.Constant(value) : null;
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

    internal class IdentityExpression<TElement>
    {
        public static Expression<Func<TElement, TElement>> Instance
        {
            get { return x => x; }
        }
    }

    internal class IdentityFunction<TElement>
    {
        public static Func<TElement, TElement> Instance
        {
            get { return x => x; }
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
