using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Roc.Data.Sql
{
    internal class ExpressionQueryTrasfer
    {
        private SqlBuilder builder;

        public ExpressionQueryTrasfer(SqlBuilder builder)
        {
            this.builder = builder;
        }

        public void ResolveWhere<T>(Expression<Func<T, bool>> expression)
        {
            builder.TableType = typeof(T);
            builder.PartType = SqlPartType.Where;
            var node = ResolveQuery((dynamic)expression.Body);
            builder.BuildWhere(node);
        }

        public void ResolveWhereIn<T, TKey>(SqlInType type, Expression<Func<T, TKey>> expression, IEnumerable<object> values, bool andOr)
        {
            builder.TableType = typeof(T);
            builder.PartType = SqlPartType.Where;
            var member = ExpressionHelper.GetMemberExpression(expression.Body);
            if (member != null)
            {
                if (andOr) builder.And();
                else builder.Or();
                InNode node = new InNode(type, new MemberNode(member), values);
                builder.BuildWhere(node);
            }
        }

        public void ResolveWhereIn<T, TKey, TResult>(SqlInType type, Expression<Func<T, TKey>> expression, IQuery<TResult> sql, bool andOr)
        {
            builder.TableType = typeof(T);
            builder.PartType = SqlPartType.Where;
            var member = ExpressionHelper.GetMemberExpression(expression.Body);
            if (member != null)
            {
                if (andOr) builder.And();
                else builder.Or();
                var node = new MemberNode(member);
                builder.BuildWhereIn(node, type, sql);
            }
        }

        public void ResolveJoin<T, TResult>(Expression<Func<T, TResult, bool>> expression, string aliasName, SqlJoinType type)
        {
            var body = expression.Body;
            if (builder.ContainsBinaryNodeType(body.NodeType))
            {
                Type tableType = typeof(TResult);
                string tableName = tableType.Name;
                builder.TableType = tableType;
                builder.AddTableName(tableName, aliasName);
                builder.BuildJoinTable(type, tableName, aliasName);

                var node = ResolveQuery((dynamic)body);
                builder.BuildJoin(node);
            }
            else throw new ArgumentException("不能解析Join条件表达式", "expression");
        }

        public void ResolveHaving<T>(SqlFunctionType type, Expression<Func<T, bool>> expression)
        {
            builder.And();
            builder.FunctionType = type;
            builder.PartType = SqlPartType.Having;

            var node = ResolveQuery((dynamic)expression.Body);
            builder.BuildWhere(node);
        }

        public void ResolveHaving<T, TKey>(SqlFunctionType type, Expression<Func<T, TKey>> expression, ExpressionType expType, object value)
        {
            builder.FunctionType = type;
            builder.PartType = SqlPartType.Having;

            var member = ExpressionHelper.GetMemberExpression(expression.Body);
            if (member != null)
            {
                OperationNode node = new OperationNode();
                node.Left = new MemberNode(member);
                node.Right = new ValueNode(value);
                node.Operator = expType;
                builder.BuildWhere(node);
            }
        }

        #region private method
        private Node ResolveQuery(ConstantExpression constantExpression)
        {
            return new ValueNode(constantExpression.Value);
        }

        private Node ResolveQuery(UnaryExpression unaryExpression)
        {
            SingleOperationNode node = new SingleOperationNode();
            node.Operator = unaryExpression.NodeType;
            node.Child = ResolveQuery((dynamic)unaryExpression.Operand);
            if (node.Operator == ExpressionType.ArrayLength)
            {
                if (node.Child is ValueNode)
                {
                    var value = (node.Child as ValueNode).Value;
                    Type type = value.GetType();
                    var property = type.GetProperty("Length");
                    node.Child = new ValueNode(property.GetValue(value, null));
                }
            }
            return node;
        }

        private Node ResolveQuery(BinaryExpression binaryExpression)
        {
            return new OperationNode
            {
                Left = ResolveQuery((dynamic)binaryExpression.Left),
                Operator = binaryExpression.NodeType,
                Right = ResolveQuery((dynamic)binaryExpression.Right)
            };
        }

        private Node ResolveQuery(MethodCallExpression callExpression)
        {
            SqlLikeType type;
            if (Enum.TryParse(callExpression.Method.Name, true, out type))
            {
                Expression objExp = callExpression.Object;
                var args = callExpression.Arguments;
                if (objExp != null)
                {
                    var member = callExpression.Object as MemberExpression;
                    var fieldValue = GetExpressionValue(args.First());

                    var node = new MemberNode(member);
                    return new LikeNode(type, node, fieldValue);
                }
                else
                {
                    if (args != null && args.Count > 1)
                    {
                        var values = GetExpressionValue(args.FirstOrDefault());
                        var member = args[1] as MemberExpression;
                        var node = new MemberNode(member);
                        return new InNode(node, values);
                    }
                }
                throw new NotSupportedException("不能解析该MethodCall_Lambda表达式");
            }
            else
            {
                var value = ResolveMethodCall(callExpression);
                return new ValueNode(value);
            }
        }

        private Node ResolveQuery(MemberExpression memberExpression, MemberExpression rootExpression = null)
        {
            rootExpression = rootExpression ?? memberExpression;
            if (memberExpression.Expression != null)
            {
                switch (memberExpression.Expression.NodeType)
                {
                    case ExpressionType.Parameter:
                        return new MemberNode(rootExpression);
                    case ExpressionType.MemberAccess:
                        return ResolveQuery(memberExpression.Expression as MemberExpression, rootExpression);
                    case ExpressionType.Call:
                    case ExpressionType.Constant:
                        return new ValueNode(GetExpressionValue(rootExpression));
                    default:
                        throw new ArgumentException("Expected member expression");
                }
            }
            else
            {
                return new ValueNode(GetExpressionValue(memberExpression));
            }
        }

        private void ResolveQuery(Expression expression)
        {
            throw new ArgumentException(string.Format("不支持该Lambda表达式 '{0}' ", expression.NodeType));
        }

        #endregion

        #region Helpers

        private object GetExpressionValue(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    return (expression as ConstantExpression).Value;
                case ExpressionType.Call:
                    return ResolveMethodCall(expression as MethodCallExpression);
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

        private object ResolveMethodCall(MethodCallExpression callExpression)
        {
            try
            {
                var arguments = callExpression.Arguments.Select(GetExpressionValue).ToArray();
                object obj = null;
                if (callExpression.Object != null) obj = GetExpressionValue(callExpression.Object);
                return callExpression.Method.Invoke(obj, arguments);
            }
            catch
            {
                throw new Exception("请不要使用静态方法,可以使用实例方法,或者使用临时变量");
            }
        }

        private object ResolveValue(PropertyInfo property, object obj)
        {
            return property.GetValue(obj, null);
        }

        private object ResolveValue(FieldInfo field, object obj)
        {
            return field.GetValue(obj);
        }
        #endregion
    }
}
