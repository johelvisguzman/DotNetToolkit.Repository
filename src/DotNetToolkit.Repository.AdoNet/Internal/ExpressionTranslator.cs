namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using Utility;

    /// <summary>
    /// Represents an internal sql expression translator.
    /// </summary>
    internal class ExpressionTranslator
    {
        #region Fields

        private readonly IComparer<ExpressionType> _comparer = new OperatorPrecedenceComparer();
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        private Func<Type, string> _getTableAlias;
        private Func<PropertyInfo, Type, string> _getColumnAlias;

        #endregion

        #region Public Methods

        /// <summary>
        /// Translates the specified predicate.
        /// </summary>
        public void Translate<T>(Expression<Func<T, bool>> predicate, Func<Type, string> getTableAlias, Func<PropertyInfo, Type, string> getColumnAlias, out string sql, out Dictionary<string, object> parameters)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (getTableAlias == null)
                throw new ArgumentNullException(nameof(getTableAlias));

            if (getColumnAlias == null)
                throw new ArgumentNullException(nameof(getColumnAlias));

            _getTableAlias = getTableAlias;
            _getColumnAlias = getColumnAlias;

            var sb = new StringBuilder();

            sb.Append("(");
            Visit(predicate.Body, sb);
            sb.Append(")");

            sql = sb.ToString();
            parameters = _parameters;
        }

        #endregion

        #region Private Methods

        private void Visit(Expression node, StringBuilder sb)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.NodeType)
            {
                case ExpressionType.Call:
                    TranslateMethodCall((MethodCallExpression)node, sb);
                    break;
                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Not:
                case ExpressionType.NotEqual:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    TranslateComparison((BinaryExpression)node, sb);
                    break;
                case ExpressionType.Constant:
                    TranslateConstant((ConstantExpression)node, sb);
                    break;
                default:
                    throw new NotSupportedException($"The expression operator '{node.NodeType}' is not supported");
            }
        }

        private void Visit(Expression parent, Expression child, StringBuilder sb)
        {
            if (_comparer.Compare(child.NodeType, parent.NodeType) > 0)
            {
                var temp = new StringBuilder();

                Visit(child, temp);

                if (temp.Length > 0)
                {
                    sb.Append("(");
                    sb.Append(temp);
                    sb.Append(")");
                }
            }
            else
            {
                Visit(child, sb);
            }
        }

        private void TranslateMethodCall(MethodCallExpression node, StringBuilder sb)
        {
            var arguments = node.Arguments.ToArray();

            // assume that static and instance Equals mean the same thing for all classes (i.e. an equality test)
            Expression firstExpression = null;
            Expression secondExpression = null;

            if (node.Object == null)
            {
                // static Equals method
                if (arguments.Length == 2)
                {
                    firstExpression = arguments[0];
                    secondExpression = arguments[1];
                }
            }
            else
            {
                // instance Equals method
                if (arguments.Length == 1)
                {
                    firstExpression = node.Object;
                    secondExpression = arguments[0];
                }
            }

            if (firstExpression != null && secondExpression != null)
            {
                var leftConstantExpression = firstExpression.AsConstantExpression();
                var rightConstantExpression = secondExpression.AsConstantExpression();

                string left;
                string right;

                if (leftConstantExpression != null && rightConstantExpression != null)
                {
                    left = ExpressionHelper.GetExpressionValue(leftConstantExpression).ToString();
                    right = ExpressionHelper.GetExpressionValue(rightConstantExpression).ToString();
                }
                else if (leftConstantExpression == null && rightConstantExpression == null)
                {
                    TranslateVariableExpression(firstExpression, out left);
                    TranslateVariableExpression(secondExpression, out right);
                }
                else
                {
                    if (leftConstantExpression != null)
                        TranslateParameterizedVariableExpression(secondExpression, leftConstantExpression, node.Method.Name, out right, out left);
                    else
                        TranslateParameterizedVariableExpression(firstExpression, rightConstantExpression, node.Method.Name, out left, out right);
                }

                sb.Append(left);

                switch (node.Method.Name)
                {
                    case "Equals":
                        sb.Append(" = ");
                        break;
                    case "StartsWith":
                    case "EndsWith":
                    case "Contains":
                        sb.Append(" LIKE ");
                        break;
                    default:
                        throw new NotSupportedException(node.Method.Name + " isn't supported");
                }

                sb.Append(right);
            }
        }

        private void TranslateComparison(BinaryExpression node, StringBuilder sb)
        {
            if (node.NodeType == ExpressionType.And ||
                node.NodeType == ExpressionType.AndAlso ||
                node.NodeType == ExpressionType.Or ||
                node.NodeType == ExpressionType.OrElse)
            {
                var temp = new StringBuilder();

                Visit(node, node.Left, temp);

                if (temp.Length > 0)
                {
                    switch (node.NodeType)
                    {
                        case ExpressionType.And:
                        case ExpressionType.AndAlso:
                            temp.Append(" AND ");
                            break;
                        case ExpressionType.Or:
                        case ExpressionType.OrElse:
                            temp.Append(" OR ");
                            break;
                    }
                }

                Visit(node, node.Right, temp);

                sb.Append(temp);
            }
            else
            {
                var leftConstantExpression = node.Left.AsConstantExpression();
                var rightConstantExpression = node.Right.AsConstantExpression();

                string left;
                string right;

                if (leftConstantExpression != null && rightConstantExpression != null)
                {
                    left = ExpressionHelper.GetExpressionValue(leftConstantExpression).ToString();
                    right = ExpressionHelper.GetExpressionValue(rightConstantExpression).ToString();
                }
                else if (leftConstantExpression == null && rightConstantExpression == null)
                {
                    TranslateVariableExpression(node.Left, out left);
                    TranslateVariableExpression(node.Right, out right);
                }
                else
                {
                    if (leftConstantExpression != null)
                        TranslateParameterizedVariableExpression(node.Right, leftConstantExpression, out right, out left);
                    else
                        TranslateParameterizedVariableExpression(node.Left, rightConstantExpression, out left, out right);
                }

                if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right))
                    return;

                sb.Append(left);

                switch (node.NodeType)
                {
                    case ExpressionType.Equal:
                        sb.Append(IsNullConstant(rightConstantExpression) ? " IS " : " = ");
                        break;
                    case ExpressionType.NotEqual:
                        sb.Append(IsNullConstant(rightConstantExpression) ? " IS NOT " : " <> ");
                        break;
                    case ExpressionType.LessThan:
                        sb.Append(" < ");
                        break;
                    case ExpressionType.LessThanOrEqual:
                        sb.Append(" <= ");
                        break;
                    case ExpressionType.GreaterThan:
                        sb.Append(" > ");
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        sb.Append(" >= ");
                        break;
                    default:
                        throw new NotSupportedException($"The binary operator '{node.NodeType}' is not supported");
                }

                sb.Append(right);
            }
        }

        private void TranslateVariableExpression(Expression variableExpression, out string column, out string columnAlias)
        {
            var propertyInfo = ExpressionHelper.GetPropertyInfo(variableExpression);
            var tableType = ExpressionHelper.GetMemberExpression(variableExpression).Expression.Type;
            var tableAlias = _getTableAlias(tableType);

            columnAlias = _getColumnAlias(propertyInfo, tableType);
            column = !string.IsNullOrEmpty(columnAlias)
                ? $"[{tableAlias}].[{columnAlias}]"
                : null;
        }

        private void TranslateVariableExpression(Expression variableExpression, out string column)
        {
            TranslateVariableExpression(variableExpression, out column, out var columnAlias);
        }

        private void TranslateParameterizedVariableExpression(Expression variableExpression, ConstantExpression constantExpression, string methodName, out string column, out string parameter)
        {
            TranslateVariableExpression(variableExpression, out column, out var columnAlias);

            if (string.IsNullOrEmpty(column))
            {
                parameter = null;
                return;
            }

            // In cases where the same property is being used multiple times in an expression, we need to generate
            // an alias for that property
            // For example:
            //    x =>  x.Id == 1 || x.Id > 2
            //      should translate to
            //    x => x.Id == @Id || x.Id > @Id1
            if (_parameters.ContainsKey($"@{columnAlias}"))
            {
                var columnAliasCount = System.Text.RegularExpressions.Regex.Match(columnAlias, @"\d+").Value;

                if (!string.IsNullOrEmpty(columnAliasCount))
                {
                    columnAlias = columnAlias.TrimEnd(columnAliasCount.ToCharArray());
                    columnAlias = $"{columnAlias}{int.Parse(columnAliasCount) + 1}";
                }
                else
                {
                    columnAlias = $"{columnAlias}{1}";
                }
            }

            parameter = $"@{columnAlias}";

            var value = ExpressionHelper.GetExpressionValue(constantExpression);

            if (!string.IsNullOrEmpty(methodName))
            {
                switch (methodName)
                {
                    case "Equals":
                        break;
                    case "StartsWith":
                        value = $"{value}%";
                        break;
                    case "EndsWith":
                        value = $"%{value}";
                        break;
                    case "Contains":
                        value = $"%{value}%";
                        break;
                    default:
                        throw new NotSupportedException(methodName + " isn't supported");
                }
            }

            _parameters.Add(parameter, value);
        }

        private void TranslateParameterizedVariableExpression(Expression variableExpression, ConstantExpression constantExpression, out string column, out string parameter)
        {
            TranslateParameterizedVariableExpression(variableExpression, constantExpression, null, out column, out parameter);
        }

        private void TranslateConstant(ConstantExpression node, StringBuilder sb)
        {
            var value = (bool)node.Value ? 1 : 0;

            sb.Append($"1 = {value}");
        }

        private static bool IsNullConstant(ConstantExpression exp)
        {
            if (exp == null)
                return false;

            return exp.Value == null;
        }

        #endregion
    }

    internal class OperatorPrecedenceComparer : Comparer<ExpressionType>
    {
        public override int Compare(ExpressionType x, ExpressionType y)
        {
            return Precedence(x).CompareTo(Precedence(y));
        }

        private static int Precedence(ExpressionType expressionType)
        {
            // Follows the rules of operator precedence from:
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/index
            switch (expressionType)
            {
                case ExpressionType.UnaryPlus:
                    return 13;
                case ExpressionType.Multiply:
                    return 12;
                case ExpressionType.Add:
                    return 11;
                case ExpressionType.LeftShift:
                case ExpressionType.RightShift:
                    return 10;
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.TypeIs:
                case ExpressionType.TypeAs:
                    return 9;
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    return 8;
                case ExpressionType.And:
                    return 7;
                case ExpressionType.ExclusiveOr:
                    return 6;
                case ExpressionType.Or:
                    return 5;
                case ExpressionType.AndAlso:
                    return 4;
                case ExpressionType.OrElse:
                    return 3;
                case ExpressionType.Coalesce:
                    return 2;
                case ExpressionType.Conditional:
                    return 1;
                default:
                    return 0;
            }
        }
    }
}