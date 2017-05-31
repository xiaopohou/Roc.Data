using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Roc.Data
{
    public interface IHaving<T>
    {
        IOrderBy<T> OrderBy<TKey>(params Expression<Func<T, TKey>>[] expressions);

        IOrderBy<T> OrderByDescending<TKey>(params Expression<Func<T, TKey>>[] expressions);
    }
}
