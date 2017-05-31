using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data
{
    public class SqlTableEntity
    {
        /// <summary>
        /// 类型 排序用 1=自己加的配置 2=特性
        /// </summary>
        internal int Type { get; set; }
        /// <summary>
        /// 实体名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数据库表名
        /// </summary>
        public string TableName { get; set; }

        internal List<SqlColumnEntity> Columns { get; set; }

        #region 构造函数
        public SqlTableEntity() { }

        public SqlTableEntity(string name, string tableName)
        {
            this.Name = name;
            this.TableName = tableName;
            this.Type = 1;
        }

        public SqlTableEntity(Type type)
            : this(type, type.Name, null)
        {

        }

        public SqlTableEntity(Type type, string tableName)
            : this(type, tableName, null)
        {

        }

        public SqlTableEntity(Type type, string tableName, List<SqlColumnEntity> columns)
        {
            this.Name = type.Name;
            this.TableName = tableName;
            this.Columns = columns;
            this.Type = 1;
        }
        #endregion

        public void AddColumn(SqlColumnEntity column)
        {
            if (column == null) return;

            if (this.Columns == null) this.Columns = new List<SqlColumnEntity>();
            var columns = this.Columns;
            if (columns.Count > 0)
            {
                var c = columns.Find(m => m.Type == column.Type && m.Name.Equals(column.Name, StringComparison.CurrentCultureIgnoreCase));
                if (c != null) columns.Remove(c);
            }
            columns.Add(column);
            this.Columns = columns.OrderBy(m => m.Type).ToList();
        }

        public SqlColumnEntity GetColumn(string columnName)
        {
            var columns = this.Columns;
            if (columns != null && columns.Count > 0)
            {
                return columns.FirstOrDefault(m => m.Name.Equals(columnName, StringComparison.CurrentCultureIgnoreCase));
            }
            return null;
        }
    }
}
