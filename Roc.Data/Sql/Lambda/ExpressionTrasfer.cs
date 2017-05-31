using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Roc.Data.Sql
{
    internal class ExpressionTrasfer //: ExpressionVisitor
    {
        private SqlBuilder builder;
        private Type resolveType;

        public ExpressionTrasfer(SqlBuilder builder)
        {
            this.builder = builder;
        }

        public void ResolveField<T, TKey>(SqlPartType type, Expression<Func<T, TKey>> expression)
        {
            this.resolveType = typeof(T);
            builder.PartType = type;
            builder.TableType = resolveType;
            var nodes = ResolveField(expression.Body);
            builder.BuildField(nodes);
        }

        public void ResolveField<T, TKey, TSource>(Expression<Func<T, TKey>> expression, TSource obj)
        {
            this.resolveType = typeof(T);
            builder.SqlType = SqlTextType.Update;
            builder.TableType = resolveType;
            var nodes = ResolveField(expression.Body);
            builder.BuildUpdateField(nodes, obj);
        }

        public void ResolveWhereField<T, TKey, TSource>(Expression<Func<T, TKey>> expression, TSource obj, bool andOr)
        {
            this.resolveType = typeof(T);
            builder.PartType = SqlPartType.Where;
            builder.TableType = resolveType;
            var nodes = ResolveField(expression.Body);
            builder.BuildWhere(nodes, obj, andOr);
        }

        public void ResolveField<T, TSource>(TSource obj, bool ignoreKey)
        {
            this.resolveType = typeof(T);
            builder.SqlType = SqlTextType.Update;
            builder.TableType = resolveType;
            var ps = Utils.GetPropertyInfos<T>(ignoreKey, true);
            if (ps != null)
            {
                string tableName = typeof(TSource).Name;
                var nodes = ps.Select(m => new MemberNode(tableName, m.Name));
                builder.BuildUpdateField(nodes, obj);
            }
        }

        private List<MemberNode> ResolveField(Expression node)
        {
            List<MemberNode> list = new List<MemberNode>();
            switch (node.NodeType)
            {
                case ExpressionType.MemberAccess:
                    list.Add(new MemberNode(node as MemberExpression, resolveType));
                    break;
                case ExpressionType.New:
                    list.AddRange(ResolveNewExpression(node as NewExpression));
                    break;
            }
            return list;
        }

        private List<MemberNode> ResolveNewExpression(NewExpression node)
        {
            var args = node.Arguments;
            int count = args != null ? args.Count : 0;
            var members = node.Members;
            List<MemberNode> list = new List<MemberNode>();
            for (int i = 0; i < count; i++)
            {
                var exp = ExpressionHelper.GetMemberExpression(args[i]);
                if (exp != null)
                {
                    MemberNode member = new MemberNode(exp, resolveType);
                    member.AliasName = members[i].Name;
                    list.Add(member);
                }
            }
            return list;
        }
    }
}
