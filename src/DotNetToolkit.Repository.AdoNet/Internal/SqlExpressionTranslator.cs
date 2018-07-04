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
    internal class SqlExpressionTranslator
    {
        #region Fields

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
            _sb.Append("WHERE ");

            var node = PartialEvaluator.Evaluate(predicate.Body);

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
                case ExpressionType.NotEqual:
                    TranslateComparison((BinaryExpression)node);
                    break;
                default:
                    throw new NotSupportedException($"The expression operator '{node.NodeType}' is not supported");
            }

            sql = _sb.ToString();
            parameters = _parameters;
        }

        #endregion

        #region Private Methods

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
                        _sb.Append(" = ");
                        break;
                    case "Contains":
                        _sb.Append(" LIKE ");
                        value = value + "%";
                        break;
                    default:
                        throw new NotSupportedException(node.Method.Name + " isn't supported");
                }

                _sb.Append($"@{columnAlias}");
                _parameters.Add($"@{columnAlias}", value);
            }
        }

        private void TranslateComparison(BinaryExpression node)
        {
            var variableExpression = node.Left;
            var constantExpression = node.Right as ConstantExpression;
            var propertyInfo = ExpressionHelper.GetPropertyInfo(variableExpression);
            var value = ExpressionHelper.GetPropertyValue(constantExpression);
            var tableType = ExpressionHelper.GetMemberExpression(variableExpression).Expression.Type;
            var tableName = _mapper.GetTableName(tableType);
            var tableAlias = _mapper.GetTableAlias(tableName);
            var columnAlias = _mapper.GetColumnAlias(propertyInfo);

            _sb.Append($"[{tableAlias}].[{columnAlias}]");

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

            _sb.Append($"@{columnAlias}");
            _parameters.Add($"@{columnAlias}", value);
        }

        private static bool IsNullConstant(Expression exp)
        {
            return (exp.NodeType == ExpressionType.Constant && ((ConstantExpression)exp).Value == null);
        }

        #endregion
    }
}