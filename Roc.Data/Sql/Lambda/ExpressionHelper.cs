using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace Roc.Data
{
    internal class ExpressionHelper
    {
        public static MemberExpression GetMemberExpression(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return expression as MemberExpression;
                case ExpressionType.Not:
                case ExpressionType.Convert:
                    return GetMemberExpression((expression as UnaryExpression).Operand);
            }
            return null;
        }

        public static string GetColumnName(MemberExpression member)
        {
            if (member != null) return member.Member.Name;
            return string.Empty;
        }

        public static string GetTableName(MemberExpression expression)
        {
            Type type = expression.Member.DeclaringType;
            if (type != null) return type.Name;
            return string.Empty;
        }

        public static object GetExpressionValue(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    return (expression as ConstantExpression).Value;
                case ExpressionType.Call:
                    return GetMethodCallValue(expression as MethodCallExpression);
                case ExpressionType.MemberAccess:
                    var memberExpr = (expression as MemberExpression);
                    object obj = memberExpr.Type;
                    if (memberExpr.Expression != null)
                        obj = GetExpressionValue(memberExpr.Expression);
                    return ResolveValue((dynamic)memberExpr.Member, obj);
                default:
                    throw new ArgumentException("Expected constant expression");
            }
        }

        public static object GetMethodCallValue(MethodCallExpression m)
        {
            try
            {
                var arguments = m.Arguments.Select(GetExpressionValue).ToArray();
                object obj = null;
                if (m.Object != null) obj = GetExpressionValue(m.Object);
                return m.Method.Invoke(obj, arguments);
            }
            catch
            {
                throw new Exception("请不要使用静态方法,可以使用实例方法,或者使用临时变量");
            }
        }

        private static object ResolveValue(PropertyInfo property, object obj)
        {
            return property.GetValue(obj, null);
        }

        private static object ResolveValue(FieldInfo field, object obj)
        {
            return field.GetValue(obj);
        }
    }
}
