namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using Helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;

    /// <summary>
    /// Represents an internal sql expression translator.
    /// </summary>
    internal class ExpressionTranslator
    {
        #region Fields

        private readonly IComparer<ExpressionType> _comparer = new OperatorPrecedenceComparer();
        private readonly StringBuilder _sb = new StringBuilder();
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        private Mapper _mapper;

        #endregion

        #region Public Methods

        /// <summary>
        /// Translates the specified predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="sql">The sql query string.</param>
        /// <param name="parameters">The parameters.</param>
        public void Translate<T>(Expression<Func<T, bool>> predicate, Mapper mapper, out string sql, out Dictionary<string, object> parameters)
        {
            _mapper = mapper;

            _sb.Append("(");

            Visit(predicate.Body);

            _sb.Append(")");

            sql = _sb.ToString();
            parameters = _parameters;
        }

        #endregion

        #region Private Methods

        private void Visit(Expression node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.NodeType)
            {
                case ExpressionType.Call:
                    TranslateMethodCall((MethodCallExpression)node);
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
                    TranslateComparison((BinaryExpression)node);
                    break;
                case ExpressionType.Constant:
                    TranslateConstant((ConstantExpression)node);
                    break;
                default:
                    throw new NotSupportedException($"The expression operator '{node.NodeType}' is not supported");
            }
        }

        private void Visit(Expression parent, Expression child)
        {
            if (_comparer.Compare(child.NodeType, parent.NodeType) > 0)
            {
                _sb.Append("(");
                Visit(child);
                _sb.Append(")");
            }
            else
            {
                Visit(child);
            }
        }

        private void TranslateMethodCall(MethodCallExpression node)
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
                var variableExpression = firstExpression;
                if (!(secondExpression is ConstantExpression constantExpression))
                {
                    constantExpression = firstExpression as ConstantExpression;
                    variableExpression = secondExpression;
                }

                var propertyInfo = ExpressionHelper.GetPropertyInfo(variableExpression);
                var value = ExpressionHelper.GetPropertyValue(constantExpression);
                var tableType = ExpressionHelper.GetMemberExpression(variableExpression).Expression.Type;
                var tableName = _mapper.GetTableName(tableType);
                var tableAlias = _mapper.GetTableAlias(tableName);
                var columnAlias = _mapper.GetColumnAlias(propertyInfo);

                _sb.Append($"[{tableAlias}].[{columnAlias}]");

                switch (node.Method.Name)
                {
                    case "Equals":
                        _sb.Append($" = @{columnAlias}");
                        break;
                    case "StartsWith":
                        _sb.Append($" LIKE @{columnAlias}");
                        value = $"{value}%";
                        break;
                    case "EndsWith":
                        _sb.Append($" LIKE @{columnAlias}");
                        value = $"%{value}";
                        break;
                    case "Contains":
                        _sb.Append($" LIKE @{columnAlias}");
                        value = $"%{value}%";
                        break;
                    default:
                        throw new NotSupportedException(node.Method.Name + " isn't supported");
                }

                _parameters.Add($"@{columnAlias}", value);
            }
        }

        private void TranslateComparison(BinaryExpression node)
        {
            var constantExpression = node.Right as ConstantExpression ?? node.Left as ConstantExpression;

            if (constantExpression == null)
            {
                Visit(node, node.Left);

                switch (node.NodeType)
                {
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                        _sb.Append(" AND ");
                        break;
                    case ExpressionType.Or:
                    case ExpressionType.OrElse:
                        _sb.Append(" OR ");
                        break;
                }

                Visit(node, node.Right);
            }
            else
            {
                var variableExpression = node.Right is ConstantExpression ? node.Left : node.Right;
                var propertyInfo = ExpressionHelper.GetPropertyInfo(variableExpression);
                var value = ExpressionHelper.GetPropertyValue(constantExpression);
                var tableType = ExpressionHelper.GetMemberExpression(variableExpression).Expression.Type;
                var tableName = _mapper.GetTableName(tableType);
                var tableAlias = _mapper.GetTableAlias(tableName);
                var columnAlias = _mapper.GetColumnAlias(propertyInfo);

                _sb.Append($"[{tableAlias}].[{columnAlias}]");

                switch (node.NodeType)
                {
                    case ExpressionType.Equal:
                        _sb.Append(IsNullConstant(node.Right) ? " IS " : " = ");
                        break;
                    case ExpressionType.NotEqual:
                        _sb.Append(IsNullConstant(node.Right) ? " IS NOT " : " <> ");
                        break;
                    case ExpressionType.LessThan:
                        _sb.Append(" < ");
                        break;
                    case ExpressionType.LessThanOrEqual:
                        _sb.Append(" <= ");
                        break;
                    case ExpressionType.GreaterThan:
                        _sb.Append(" > ");
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        _sb.Append(" >= ");
                        break;
                    default:
                        throw new NotSupportedException($"The binary operator '{node.NodeType}' is not supported");
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

                _parameters.Add($"@{columnAlias}", value);
                _sb.Append($"@{columnAlias}");
            }
        }

        private void TranslateConstant(ConstantExpression node)
        {
            var value = (bool)node.Value ? 1 : 0;

            _sb.Append($"1 = {value}");
        }

        private static bool IsNullConstant(Expression exp)
        {
            return exp.NodeType == ExpressionType.Constant && ((ConstantExpression)exp).Value == null;
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