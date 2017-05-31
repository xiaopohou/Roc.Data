using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Roc.Data
{
    public interface IExecute<T>
    {
        IWhere<T> Where<TKey>(Expression<Func<T, TKey>> expression);
        IWhere<T> Where(Expression<Func<T, bool>> expression);

        IExecute<T> Insert(T obj, bool ignoreKey = true, bool increment = false);
        IExecute<T> Insert(object obj, bool ignoreKey = true, bool increment = false);
        IExecute<T> Insert<TKey>(T obj, Expression<Func<T, TKey>> fields, bool increment = false);

        IExecute<T> Insert(IEnumerable<T> list, bool ignoreKey = true);
        IExecute<T> Insert<TKey>(IEnumerable<T> list, Expression<Func<T, TKey>> fields);
        IExecute<T> InsertWithQuery<TResult>(IQuery<TResult> sql, bool ignoreKey = true);
        IExecute<T> InsertWithQuery<TKey, TResult>(Expression<Func<T, TKey>> fields, IQuery<TResult> sql);

        IExecute<T> Update<TSource>(TSource obj, bool ignoreKey = true);
        IExecute<T> Update<TKey, TSource>(TSource obj, Expression<Func<T, TKey>> fields);

        IExecute<T> Delete();
        IExecute<T> Delete(Expression<Func<T, bool>> where);
        void Truncate();

        IQuery<TResult> Join<TResult>(Expression<Func<T, TResult, bool>> expression, SqlJoinType type = SqlJoinType.LEFT, string aliasName = "");

        string GetSql();

        Dictionary<string, object> GetParameters();

        int Execute();
    }
}
