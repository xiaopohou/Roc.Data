using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data
{
    /// <summary>
    /// 字段忽略 查询不忽略（修改，新增，删除 忽略）
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {

    }
}
