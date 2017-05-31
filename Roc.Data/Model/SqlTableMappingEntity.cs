using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data
{
    public class SqlTableMappingEntity
    {
        public Type Type { get; set; }

        public string Name { get; set; }

        public string AliasName { get; set; }

        public SqlTableMappingEntity()
        {

        }

        public SqlTableMappingEntity(Type type)
            : this(type, string.Empty)
        {

        }

        public SqlTableMappingEntity(Type type, string aliasName)
        {
            this.Type = type;
            this.Name = type.Name;
            this.AliasName = aliasName;
        }
    }
}
