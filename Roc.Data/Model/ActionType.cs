using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data
{
    public enum ActionType
    {
        /// <summary>
        /// 可读可写
        /// </summary>
        ReadOrWrite = 1,
        /// <summary>
        /// 可读
        /// </summary>
        Read = 2,
        /// <summary>
        /// 可写
        /// </summary>
        Write = 3,
        /// <summary>
        /// 不能操作
        /// </summary>
        Undo = 4
    }
}
