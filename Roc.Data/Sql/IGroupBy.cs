using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Roc.Data
{
    public interface IGroupBy<T>
    {
        IGroupBy<T> SelectFunction<TKey>(SqlFunctionType type, params Expression<Func<T, TKey>>[] expressions);

        IHaving<T> Having(params Expression<Func<T, bool>>[] expressions);

        IHaving<T> Having<TKey>(SqlFunctionType type, Expression<Func<T, TKey>> expression, ExpressionType expType, object value);

        IHaving<T> Having(SqlFunctionType type, Expression<Func<T, bool>> expression);
    }
}
