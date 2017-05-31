using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Roc.Data.Sql
{
    class FieldExpressionVisitor : ExpressionVisitor<Node>
    {
        private SqlBuilder builder;
        private Type resolveType;

        public FieldExpressionVisitor(SqlBuilder builder)
        {
            this.builder = builder;
        }

        public void ResolveField<T, TKey>(SqlPartType type, Expression<Func<T, TKey>> expression)
        {
            resolveType = typeof(T);
            builder.PartType = type;

            var node = this.Visit(expression.Body);
            builder.BuildField((dynamic)node);
        }

        public void ResolveUpdateField<T, TKey, TSource>(Expression<Func<T, TKey>> expression, TSource obj)
        {
            SetBuilder<T>(SqlTextType.Update);
            var node = this.Visit(expression.Body);
            builder.BuildUpdateField((dynamic)node, obj, false);
        }

        public void ResolveWhereField<T, TKey, TSource>(Expression<Func<T, TKey>> expression, TSource obj, bool andOr)
        {
            SetBuilder<T>(SqlTextType.None, SqlPartType.Where);
            var node = this.Visit(expression.Body);
            builder.BuildWhere((dynamic)node, obj, andOr);
        }

        public void ResolveWhereField<T, TKey>(Expression<Func<T, TKey>> expression, object begin, object end)
        {
            SetBuilder<T>(SqlTextType.None, SqlPartType.Where);
            var node = this.Visit(expression.Body);
            builder.BuildWhereBetween<T>((dynamic)node, begin, end);
        }

        public void ResolveUpdateField<T, TSource>(TSource obj, bool ignoreKey)
        {
            SetBuilder<T>(SqlTextType.Update);
            var ps = Utils.GetPropertyInfos<T>(ignoreKey, true);
            if (ps != null)
            {
                var nodes = ps.Select(m => new MemberNode(resolveType, m.Name));
                builder.BuildUpdateField(new FieldNode(nodes), obj, true);
            }
        }

        public void ResolveInsertField<T, TSource>(IEnumerable<TSource> list, bool ignoreKey)
        {
            if (list == null || list.Count() < 1) return;
            SetBuilder<T>(SqlTextType.Insert);
            var ps = Utils.GetPropertyInfos<T>(ignoreKey, true);
            if (ps != null)
            {
                var nodes = ps.Select(m => new MemberNode(resolveType, m.Name));
                builder.BuildInsertField(new FieldNode(nodes), list);
            }
        }

        public void ResolveInsertField<T>(object obj, bool ignoreKey)
        {
            var list = Utils.GetObjectList(obj);
            if (list == null || list.Count() < 1)
            {
                list = new List<object>();
                list.Add(obj);
            }
            this.ResolveInsertField<T, object>(list, ignoreKey);
        }

        public void ResolveInsertField<T, TKey, TSource>(Expression<Func<T, TKey>> expression, IEnumerable<TSource> list)
        {
            if (list == null || list.Count() < 1) return;
            SetBuilder<T>(SqlTextType.Insert);
            var node = this.Visit(expression.Body);
            builder.BuildInsertField((dynamic)node, list);
        }

        public void ResolveInsertField<T, TResult>(IQuery<TResult> sql, bool ignoreKey = true)
        {
            SetBuilder<T>(SqlTextType.Insert);
            var ps = Utils.GetPropertyInfos<T>(ignoreKey, true);
            if (ps != null)
            {
                var nodes = ps.Select(m => new MemberNode(resolveType, m.Name));
                builder.BuildInsertQuery<T, TResult>(new FieldNode(nodes), sql);
            }
        }

        public void ResolveInsertField<T, TKey, TResult>(IQuery<TResult> sql, Expression<Func<T, TKey>> expression)
        {
            SetBuilder<T>(SqlTextType.Insert);
            var node = this.Visit(expression.Body);
            builder.BuildInsertQuery<T, TResult>((dynamic)node, sql);
        }

        private void SetBuilder<T>(SqlTextType type, SqlPartType partType = SqlPartType.None)
        {
            this.resolveType = typeof(T);
            if (type != SqlTextType.None)
                builder.SqlType = type;
            if (partType != SqlPartType.None)
                builder.PartType = partType;
        }

        protected override Node VisitMemberAccess(MemberExpression m)
        {
            MemberNode node = new MemberNode(m);
            return new FieldNode(node);
        }

        protected override Node VisitNew(NewExpression node)
        {
            var args = node.Arguments;
            int count = args != null ? args.Count : 0;
            var members = node.Members;
            FieldNode fileNode = new FieldNode();
            for (int i = 0; i < count; i++)
            {
                var exp = ExpressionHelper.GetMemberExpression(args[i]);
                if (exp != null)
                {
                    MemberNode member = new MemberNode(exp);
                    member.AliasName = members[i].Name;
                    fileNode.AddNode(member);
                }
            }
            return fileNode;
        }

        protected override Node VisitUnary(UnaryExpression u)
        {
            return this.Visit(u.Operand);
        }
    }
}
