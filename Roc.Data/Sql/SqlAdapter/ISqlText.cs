using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data
{
    internal interface ISqlText
    {
        bool SupportParameter { get; }

        string Query(SqlTextEntity entity);
        string QueryPage(SqlTextEntity entity);
        string Insert(SqlTextEntity entity, bool key = false);
        string Update(SqlTextEntity entity);
        string Delete(SqlTextEntity entity);
        string Truncate(string tableName);

        string TableName(string tableName);
        string FieldName(string filedName);
        string FieldName(string tableName, string fieldName);
        string ParameterName(string parameterId);
    }
}
