using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Roc.Data.Sql;

namespace Roc.Data
{
    public class SqlLam<T> : SqlLamBase, IQuery<T>, IExecute<T>, IWhere<T>, IGroupBy<T>, IHaving<T>, IOrderBy<T>
    {
        private bool pre;
        private object source;

        #region 构造函数
        public SqlLam()
            : this(ProviderType.SQLServer2005, true)
        {

        }

        public SqlLam(string aliasName)
            : this(ProviderType.SQLServer2005, true)
        {
            this.As(aliasName);
        }

        public SqlLam(string aliasName, ProviderType type)
            : this(type, true)
        {
            this.As(aliasName);
        }

        public SqlLam(ProviderType type, bool flag)
        {
            this.Type = type;
            if (flag)
            {
                base.Builder = new SqlBuilder(typeof(T), type);
                base.FieldVisitor = new FieldExpressionVisitor(Builder);
                base.QueryVisitor = new QueryExpressionVisitor(Builder);
            }
        }
        #endregion

        public IQuery<T> As(string aliasName)
        {
            Builder.AddTableName(typeof(T), aliasName);
            return this;
        }

        public void Clear()
        {
            Builder.Clear();
        }

        public int Execute()
        {
            using (var db = new DbClient(this.Type))
            {
                return db.ExecuteNonQuery(this.GetSql(), this.GetParameters());
            }
        }

        public IQuery<T> QueryPage(int pageNumber, int pageSize = 10)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            Builder.PageNumber = pageNumber;
            Builder.PageSize = pageSize;
            Builder.SqlType = SqlTextType.QueryPage;
            return this;
        }

        #region Insert Update Delete

        public IWhere<T> Where<TKey>(Expression<Func<T, TKey>> expression)
        {
            FieldVisitor.ResolveWhereField(expression, source, true);
            return this;
        }

        public IWhere<T> And<TKey>(Expression<Func<T, TKey>> expression)
        {
            FieldVisitor.ResolveWhereField(expression, source, true);
            return this;
        }

        public IWhere<T> Or<TKey>(Expression<Func<T, TKey>> expression)
        {
            FieldVisitor.ResolveWhereField(expression, source, false);
            return this;
        }

        #region insert

        public IExecute<T> Insert(T obj, bool ignoreKey = true, bool increment = false)
        {
            Builder.InsertKey = increment;
            return this.Insert(new List<T>() { obj }, ignoreKey);
        }

        public IExecute<T> Insert(object obj, bool ignoreKey = true, bool increment = false)
        {
            Builder.InsertKey = increment;
            FieldVisitor.ResolveInsertField<T>(obj, ignoreKey);
            return this;
        }

        public IExecute<T> Insert<TKey>(T obj, Expression<Func<T, TKey>> fields, bool increment = false)
        {
            Builder.InsertKey = increment;
            return this.Insert<TKey>(new List<T>() { obj }, fields);
        }

        public IExecute<T> Insert(IEnumerable<T> list, bool ignoreKey = true)
        {
            FieldVisitor.ResolveInsertField<T, T>(list, ignoreKey);
            return this;
        }

        public IExecute<T> Insert<TKey>(IEnumerable<T> list, Expression<Func<T, TKey>> fields)
        {
            FieldVisitor.ResolveInsertField(fields, list);
            return this;
        }

        public IExecute<T> InsertWithQuery<TResult>(IQuery<TResult> sql, bool ignoreKey = true)
        {
            FieldVisitor.ResolveInsertField<T, TResult>(sql, ignoreKey);
            return this;
        }

        public IExecute<T> InsertWithQuery<TKey, TResult>(Expression<Func<T, TKey>> fields, IQuery<TResult> sql)
        {
            FieldVisitor.ResolveInsertField<T, TKey, TResult>(sql, fields);
            return this;
        }

        #endregion

        #region update
        public IExecute<T> Update<TSource>(TSource obj, bool ignoreKey = true)
        {
            this.source = obj;
            FieldVisitor.ResolveUpdateField<T, TSource>(obj, ignoreKey);
            return this;
        }

        public IExecute<T> Update<TKey, TSource>(TSource obj, Expression<Func<T, TKey>> fields)
        {
            this.source = obj;
            FieldVisitor.ResolveUpdateField(fields, obj);
            return this;
        }
        #endregion

        #region delete
        public IExecute<T> Delete()
        {
            return this.Delete(null);
        }

        public IExecute<T> Delete(Expression<Func<T, bool>> expression)
        {
            Builder.SqlType = SqlTextType.Delete;
            Builder.AddTableName();
            return ResolveWhereExpression(true, expression);
        }

        public void Truncate()
        {
            Builder.SqlType = SqlTextType.Truncate;
        }
        #endregion

        #endregion

        #region Where 条件

        public IWhere<T> Where(Expression<Func<T, bool>> expressions)
        {
            return this.And(expressions);
        }

        public IWhere<T> And()
        {
            Builder.And();
            return this;
        }

        public IWhere<T> And(Expression<Func<T, bool>> expression)
        {
            return ResolveWhereExpression(true, expression);
        }

        public IWhere<T> Or()
        {
            Builder.Or();
            return this;
        }

        public IWhere<T> Or(Expression<Func<T, bool>> expression)
        {
            return ResolveWhereExpression(false, expression);
        }

        public IWhere<T> In<TKey>(Expression<Func<T, TKey>> expression, IEnumerable<object> values, bool andOr = true)
        {
            QueryVisitor.ResolveWhereIn(SqlInType.IN, expression, values, andOr);
            return this;
        }

        public IWhere<T> NotIn<TKey>(Expression<Func<T, TKey>> expression, IEnumerable<object> values, bool andOr = true)
        {
            QueryVisitor.ResolveWhereIn(SqlInType.NOTIN, expression, values, andOr);
            return this;
        }

        public IWhere<T> In<TKey, TResult>(Expression<Func<T, TKey>> expression, IQuery<TResult> sql, bool andOr = true)
        {
            QueryVisitor.ResolveWhereIn(SqlInType.IN, expression, sql, andOr);
            return this;
        }

        public IWhere<T> NotIn<TKey, TResult>(Expression<Func<T, TKey>> expression, IQuery<TResult> sql, bool andOr = true)
        {
            QueryVisitor.ResolveWhereIn(SqlInType.NOTIN, expression, sql, andOr);
            return this;
        }

        public IWhere<T> Between<TKey>(Expression<Func<T, TKey>> expression, object between, object and)
        {
            Builder.And();
            FieldVisitor.ResolveWhereField<T, TKey>(expression, between, and);
            return this;
        }

        public IWhere<T> Begin()
        {
            pre = true;
            return this;
        }

        public IWhere<T> End()
        {
            Builder.EndExpression();
            return this;
        }

        #endregion

        #region 查询列

        public IQuery<T> Select(params Expression<Func<T, object>>[] expressions)
        {
            return this.Select<object>(expressions);
        }

        public IQuery<T> Select<TResult>(params Expression<Func<T, TResult>>[] expressions)
        {
            return ResolveFieldExpression(SqlPartType.Section, expressions);
        }

        public IQuery<T> SelectDistinct(params Expression<Func<T, object>>[] expressions)
        {
            return this.SelectDistinct<object>(expressions);
        }

        public IQuery<T> SelectDistinct<TResult>(params Expression<Func<T, TResult>>[] expressions)
        {
            Builder.Distinct = true;
            return ResolveFieldExpression(SqlPartType.Section, expressions);
        }

        public IQuery<T> Top(double top, bool percent = false)
        {
            Builder.BuildTopField(top, percent);
            return this;
        }

        #endregion

        #region 分组

        public IGroupBy<T> GroupBy<TKey>(params Expression<Func<T, TKey>>[] expressions)
        {
            return ResolveFieldExpression(SqlPartType.GroupBy, expressions);
        }

        public IGroupBy<T> SelectFunction<TKey>(SqlFunctionType type, params Expression<Func<T, TKey>>[] expressions)
        {
            Builder.FunctionType = type;
            return ResolveFieldExpression(SqlPartType.Function, expressions);
        }

        #endregion

        #region Having

        public IHaving<T> Having(params Expression<Func<T, bool>>[] expressions)
        {
            foreach (var item in expressions)
            {
                QueryVisitor.ResolveHaving(SqlFunctionType.NONE, item);
            }
            return this;
        }

        public IHaving<T> Having<TKey>(SqlFunctionType type, Expression<Func<T, TKey>> expression, ExpressionType expType, object value)
        {
            QueryVisitor.ResolveHaving(type, expression, expType, value);
            return this;
        }

        public IHaving<T> Having(SqlFunctionType type, Expression<Func<T, bool>> expression)
        {
            QueryVisitor.ResolveHaving(type, expression);
            return this;
        }

        #endregion

        #region 排序

        public IOrderBy<T> OrderBy<TKey>(params Expression<Func<T, TKey>>[] expressions)
        {
            return ResolveFieldExpression(SqlPartType.OrderBy, expressions);
        }

        public IOrderBy<T> OrderByDescending<TKey>(params Expression<Func<T, TKey>>[] expressions)
        {
            Builder.SortType = SqlOrderByType.DESC;
            return ResolveFieldExpression(SqlPartType.OrderBy, expressions);
        }
        #endregion

        #region Join

        public IQuery<TResult> Join<TResult>(Expression<Func<T, TResult, bool>> expression, SqlJoinType type = SqlJoinType.LEFT, string aliasName = "")
        {
            var join = new SqlLam<TResult>(this.Type, false);
            join.Builder = this.Builder;
            join.FieldVisitor = this.FieldVisitor;
            join.QueryVisitor = this.QueryVisitor;
            QueryVisitor.ResolveJoin(expression, aliasName, type);
            return join;
        }

        #endregion

        #region private method
        private SqlLam<T> ResolveFieldExpression<TKey>(SqlPartType type, Expression<Func<T, TKey>> expression)
        {
            FieldVisitor.ResolveField<T, TKey>(type, expression);
            return this;
        }

        private SqlLam<T> ResolveFieldExpression<TKey>(SqlPartType type, params Expression<Func<T, TKey>>[] expressions)
        {
            foreach (var item in expressions)
            {
                FieldVisitor.ResolveField<T, TKey>(type, item);
            }
            return this;
        }

        private SqlLam<T> ResolveWhereExpression(bool and, Expression<Func<T, bool>> expression)
        {
            QueryVisitor.ResolveWhere<T>(and, pre, expression);
            pre = false;
            return this;
        }
        #endregion
    }

    public abstract class SqlLamBase
    {
        private ProviderType type;

        public SqlLamBase()
        {

        }

        internal SqlBuilder Builder { get; set; }

        internal FieldExpressionVisitor FieldVisitor { get; set; }

        internal QueryExpressionVisitor QueryVisitor { get; set; }

        public ProviderType Type { get { return type; } set { type = value; } }

        public string GetSql()
        {
            return Builder.GetSql();
        }

        public Dictionary<string, object> GetParameters()
        {
            return Builder.GetParameters();
        }
    }
}
