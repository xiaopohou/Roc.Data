using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Roc.Data.Sql
{
    class QueryExpressionVisitor : ExpressionVisitor<Node>
    {
        private SqlBuilder builder;

        public QueryExpressionVisitor(SqlBuilder builder)
        {
            this.builder = builder;
        }

        public void ResolveWhere<T>(bool and, bool pre, Expression<Func<T, bool>> expression)
        {
            if (expression == null) return;
            builder.PartType = SqlPartType.Where;
            if (pre)
            {
                builder.BeginExpression();
            }
            else
            {
                if (and) builder.And();
                else builder.Or();
            }
            var node = this.Visit((dynamic)expression.Body);
            builder.BuildWhere(node);
        }

        public void ResolveWhereIn<T, TKey>(SqlInType type, Expression<Func<T, TKey>> expression, IEnumerable<object> values, bool andOr)
        {
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
                builder.AddTableName(tableType, aliasName);
                builder.BuildJoinTable(type, tableType, aliasName);

                var node = this.Visit((dynamic)body);
                builder.BuildJoin(node);
            }
            else throw new ArgumentException("不能解析Join条件表达式", "expression");
        }

        public void ResolveHaving<T>(SqlFunctionType type, Expression<Func<T, bool>> expression)
        {
            builder.And();
            builder.FunctionType = type;
            builder.PartType = SqlPartType.Having;

            var node = this.Visit((dynamic)expression.Body);
            builder.BuildWhere(node);
        }

        public void ResolveHaving<T, TKey>(SqlFunctionType type, Expression<Func<T, TKey>> expression, ExpressionType expType, object value)
        {
            builder.FunctionType = type;
            builder.PartType = SqlPartType.Having;

            var member = ExpressionHelper.GetMemberExpression(expression.Body);
            if (member != null)
            {
                BinaryNode node = new BinaryNode();
                node.Left = new MemberNode(member);
                node.Right = new ValueNode(value);
                node.Operator = expType;
                builder.BuildWhere(node);
            }
        }

        #region protected
        protected override Node VisitUnary(UnaryExpression u)
        {
            UnaryNode node = new UnaryNode();
            node.Operator = u.NodeType;
            node.Child = this.Visit(u.Operand);
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

        protected override Node VisitBinary(BinaryExpression b)
        {
            BinaryNode node = new BinaryNode();
            node.Left = this.Visit(b.Left);
            node.Operator = b.NodeType;
            node.Right = this.Visit(b.Right);
            return node;
        }

        protected override Node VisitConstant(ConstantExpression c)
        {
            return new ValueNode(c.Value);
        }

        protected override Node VisitMemberAccess(MemberExpression m)
        {
            return this.VisitMemberAccess(m, null);
        }

        protected override Node VisitMethodCall(MethodCallExpression m)
        {
            SqlLikeType type;
            if (Enum.TryParse(m.Method.Name, true, out type))
            {
                Expression objExp = m.Object;
                var args = m.Arguments;
                if (objExp != null)
                {
                    var member = m.Object as MemberExpression;
                    var fieldValue = ExpressionHelper.GetExpressionValue(args.FirstOrDefault());
                    var node = new MemberNode(member);
                    return new LikeNode(type, node, fieldValue);
                }
                else
                {
                    if (args != null && args.Count > 1)
                    {
                        var values = ExpressionHelper.GetExpressionValue(args.FirstOrDefault());
                        var member = args[1] as MemberExpression;
                        var node = new MemberNode(member);
                        return new InNode(node, values);
                    }
                }
                throw new NotSupportedException("不能解析该MethodCall_Lambda表达式");
            }
            else
            {
                var handler = MethodConfig.GetMethodHandler(m.Method.DeclaringType, m.Method.Name);
                if (handler != null)
                {
                    return handler(m.Object, m.Arguments, m);
                }
                else
                {
                    var value = ExpressionHelper.GetMethodCallValue(m);
                    return new ValueNode(value);
                }
            }
        }
        #endregion

        #region private method

        private Node VisitMemberAccess(MemberExpression m, MemberExpression root = null)
        {
            Node node;
            root = root ?? m;
            if (m.Expression != null)
            {
                switch (m.Expression.NodeType)
                {
                    case ExpressionType.Parameter:
                        node = new MemberNode(root);
                        break;
                    case ExpressionType.MemberAccess:
                        node = VisitMemberAccess(m.Expression as MemberExpression, root);
                        break;
                    case ExpressionType.Call:
                    case ExpressionType.Constant:
                        node = new ValueNode(ExpressionHelper.GetExpressionValue(root));
                        break;
                    default:
                        throw new ArgumentException("Expected member expression");
                }
            }
            else node = new ValueNode(ExpressionHelper.GetExpressionValue(m));
            return node;
        }

        #endregion
    }
}
