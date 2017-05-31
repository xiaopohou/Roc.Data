using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Roc.Data
{
    public interface IWhere<T>
    {
        IGroupBy<T> GroupBy<TKey>(params Expression<Func<T, TKey>>[] expressions);

        IWhere<T> And();
        IWhere<T> And(Expression<Func<T, bool>> expression);
        IWhere<T> And<TKey>(Expression<Func<T, TKey>> expression);
        IWhere<T> Or();
        IWhere<T> Or(Expression<Func<T, bool>> expression);
        IWhere<T> Or<TKey>(Expression<Func<T, TKey>> expression);

        IWhere<T> In<TKey>(Expression<Func<T, TKey>> expression, IEnumerable<object> values, bool andOr = true);
        IWhere<T> In<TKey, TResult>(Expression<Func<T, TKey>> expression, IQuery<TResult> sql, bool andOr = true);

        IWhere<T> NotIn<TKey>(Expression<Func<T, TKey>> expression, IEnumerable<object> values, bool andOr = true);
        IWhere<T> NotIn<TKey, TResult>(Expression<Func<T, TKey>> expression, IQuery<TResult> sql, bool andOr = true);

        IWhere<T> Between<TKey>(Expression<Func<T, TKey>> expression, object between, object and);

        IWhere<T> Begin();
        IWhere<T> End();
    }
}
