using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Roc.Data
{
    public interface IQuery<T>
    {
        IGroupBy<T> GroupBy<TKey>(params Expression<Func<T, TKey>>[] expressions);

        IQuery<T> Select(params Expression<Func<T, object>>[] expressions);

        IQuery<T> Select<TResult>(params Expression<Func<T, TResult>>[] expressions);

        IQuery<T> SelectDistinct(params Expression<Func<T, object>>[] expressions);

        IQuery<T> SelectDistinct<TResult>(params Expression<Func<T, TResult>>[] expressions);

        IWhere<T> Where(Expression<Func<T, bool>> expression);

        IQuery<T> As(string aliasName);

        IQuery<T> Top(double top, bool percent = false);

        IQuery<TResult> Join<TResult>(Expression<Func<T, TResult, bool>> expression, SqlJoinType type = SqlJoinType.LEFT, string aliasName = "");

        IOrderBy<T> OrderBy<TKey>(params Expression<Func<T, TKey>>[] expressions);

        IOrderBy<T> OrderByDescending<TKey>(params Expression<Func<T, TKey>>[] expressions);

        IQuery<T> QueryPage(int pageNumber, int pageSize = 10);

        string GetSql();

        Dictionary<string, object> GetParameters();
    }
}
