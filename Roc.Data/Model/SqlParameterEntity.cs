using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Roc.Data
{
    public class SqlParameterEntity
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public ParameterDirection Direction { get; set; }
        public DbType? DbType { get; set; }
        public int? Size { get; set; }
        public IDbDataParameter AttachedParam { get; set; }

        public SqlParameterEntity()
        {

        }

        public SqlParameterEntity(string name, object value, ParameterDirection direction = ParameterDirection.Input, DbType? type = null, int? size = null)
        {
            this.Name = name;
            this.Value = value ?? DBNull.Value;
            this.Direction = direction;
            this.DbType = type;

            if (type == null && value != null)
            {
                this.DbType = Utils.LookupDbType(value.GetType());
            }
            string s = value as string;
            if (s != null)
            {
                if (s.Length <= Common.DefaultLength)
                    this.Size = Common.DefaultLength;
            }
            if (size != null) this.Size = size.Value;
        }
    }
}
