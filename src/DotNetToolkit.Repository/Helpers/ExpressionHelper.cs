namespace DotNetToolkit.Repository.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    // https://github.com/SharpRepository/SharpRepository/tree/master/SharpRepository.Repository/Helpers/ExpressionHelper.cs
    internal static class ExpressionHelper
    {
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
    }
}
