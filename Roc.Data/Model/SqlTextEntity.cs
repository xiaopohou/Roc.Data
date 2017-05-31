using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data
{
    public class SqlTextEntity
    {
        /// <summary>
        /// 页面数 从 1 开始
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// 页面大小
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 查询列
        /// </summary>
        public string Selection { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// From 后面的 东西
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// 查询条件
        /// </summary>
        public string Conditions { get; set; }
        /// <summary>
        /// 排序条件
        /// </summary>
        public string OrderBy { get; set; }
        /// <summary>
        /// 分组条件
        /// </summary>
        public string Grouping { get; set; }
        /// <summary>
        /// 分组后条件
        /// </summary>
        public string Having { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public string Parameter { get; set; }

        public SqlTextEntity()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
        }
    }
}
