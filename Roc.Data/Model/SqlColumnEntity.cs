using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data
{
    public class SqlColumnEntity
    {
        /// <summary>
        /// 类型 排序用 1=自己加的配置 2=特性
        /// </summary>
        internal int Type { get; set; }
        /// <summary>
        /// 是否是主键
        /// </summary>
        public bool Key { get; set; }
        /// <summary>
        /// 是否是自增
        /// </summary>
        public bool Increment { get; set; }
        /// <summary>
        /// 是否忽略该字段
        /// </summary>
        public bool Ignore { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public ActionType ActionType { get; set; }
        /// <summary>
        /// 实体属性名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数据库对应字段名称
        /// </summary>
        public string FieldName { get; set; }

        public SqlColumnEntity()
        {
            this.Type = 1;
        }

        public SqlColumnEntity(string name) : this(name, name) { }

        public SqlColumnEntity(string name, string fieldName)
        {
            this.Name = name;
            this.FieldName = fieldName;
            this.ActionType = ActionType.ReadOrWrite;
            this.Type = 1;
        }
    }
}
