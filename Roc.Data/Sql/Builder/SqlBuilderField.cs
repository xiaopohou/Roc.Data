using Roc.Data.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Roc.Data.Sql
{
    internal partial class SqlBuilder
    {
        private SqlPartType partType;
        private SqlOrderByType sortType;
        private SqlFunctionType functionType;
        private string topString;

        public SqlPartType PartType { get { return partType; } set { partType = value; } }

        public SqlOrderByType SortType { get { return sortType; } set { sortType = value; } }

        public SqlFunctionType FunctionType { get { return functionType; } set { functionType = value; } }

        public void BuildField(FieldNode node)
        {
            foreach (var item in node.Nodes)
            {
                this.BuildField(item.TableName, item.FieldName, item.AliasName);
            }
        }

        public void BuildTopField(double top, bool percent)
        {
            string topstring = "TOP";
            if (percent)
            {
                top = top < 0 ? 0 : top;
                top = top > 100 ? 100 : top;
            }
            else
            {
                top = top < 1 ? 1 : (int)top;
            }
            topString = string.Format("{0} ({1})", topstring, top);
            if (percent) topString += " PERCENT";
            //selection = string.Format(" {0} {1}", selection, this.GetTableName(_tableType));
            string first = _selections.FirstOrDefault();
            if (!string.IsNullOrEmpty(first) && first.Contains(topstring)) _selections.Remove(first);
            //_selections.Insert(0, selection);
        }

        public void BuildUpdateField<T>(FieldNode node, T obj, bool flag)
        {
            foreach (var item in node.Nodes)
            {
                if (flag)
                {
                    if (Utils.IsIncrementColumn(item.TableName, item.FieldName)) continue;
                }
                object value = Utils.GetPropertyValue(item.FieldName, obj);
                this.BuildUpdateFiled(item.TableName, item.FieldName, value);
            }
        }

        public void BuildInsertField<T>(FieldNode node, IEnumerable<T> list)
        {
            foreach (var item in node.Nodes)
            {
                string name = this.GetFieldName(item.FieldName);
                _selections.Add(name);
            }
            List<string> fields = new List<string>();
            foreach (var obj in list)
            {
                foreach (var item in node.Nodes)
                {
                    string p = this.GetParameter(item.FieldName);
                    object value = Utils.GetPropertyValue(item.FieldName, obj);
                    this.AddParameter(p, value);
                    fields.Add(p);
                }
                _parameters.Add(string.Format("({0})", string.Join(",", fields)));
                fields.Clear();
            }
        }

        public void BuildInsertQuery<T, TResult>(FieldNode node, IQuery<TResult> sqlQuery)
        {
            string sql = sqlQuery.GetSql();

            foreach (var item in node.Nodes)
            {
                string name = this.GetFieldName(item.FieldName);
                _selections.Add(name);
            }

            var parameters = sqlQuery.GetParameters();
            foreach (var param in parameters)
            {
                this.AddParameter(param.Key, param.Value);
            }
            _insertWithQuery = true;
            _parameters.Add(sql);
        }

        public void BuildWhere<T>(FieldNode node, T obj, bool andOr)
        {
            string op = _operations[ExpressionType.Equal];
            foreach (var item in node.Nodes)
            {
                if (andOr) this.And();
                else this.Or();
                object value = Utils.GetPropertyValue(item.FieldName, obj);
                this.AddCondition(item.TableName, item.FieldName, op, value);
            }
        }

        public void BuildWhereBetween<T>(FieldNode node, object begin, object end)
        {
            if (node.HasNode())
            {
                var member = node.Nodes.FirstOrDefault();
                string name = this.GetFieldName(member.TableName, member.FieldName);
                string p1 = this.GetParameter(member.FieldName, "begin");
                this.AddParameter(p1, begin);
                string p2 = this.GetParameter(member.FieldName, "end");
                this.AddParameter(p2, end);
                string condition = string.Format("{0} BETWEEN {1} AND {2}", name, p1, p2);
                this.AppendCondition(condition);
            }
        }

        #region private method
        private void BuildUpdateFiled(string tableName, string fieldName, object value)
        {
            string name = this.GetFieldName(tableName, fieldName);
            string p = this.GetParameter(fieldName);
            string selection = string.Format("{0}={1}", name, p);
            _selections.Add(selection);
            this.AddParameter(p, value);
        }

        private void BuildField(string tableName, string fieldName, string aliasName)
        {
            string name = string.Empty;
            switch (partType)
            {
                case SqlPartType.Section:
                    name = this.GetFieldName(tableName, fieldName, aliasName);
                    _selections.Add(name);
                    break;
                case SqlPartType.OrderBy:
                    name = this.GetFieldName(tableName, fieldName);
                    string sort = string.Format("{0} {1}", name, sortType.ToString());
                    _sorts.Add(sort);
                    break;
                case SqlPartType.GroupBy:
                    name = this.GetFieldName(tableName, fieldName);
                    _groupings.Add(name);
                    break;
                case SqlPartType.Having:
                    name = this.GetFieldName(tableName, fieldName);
                    _havings.Add(name);
                    break;
                case SqlPartType.Function:
                    this.BuildFunctionField(tableName, fieldName, aliasName);
                    break;
            }
        }

        private void BuildFunctionField(string tableName, string fieldName, string aliasName)
        {
            if (functionType == SqlFunctionType.NONE) return;
            string name = this.GetFieldName(tableName, fieldName);
            aliasName = string.IsNullOrEmpty(aliasName) ? fieldName : aliasName;
            aliasName = this.GetFieldName(aliasName);
            string selection = string.Format("{0}({1}) AS {2}", functionType.ToString(), name, aliasName);
            _selections.Add(selection);
        }
        #endregion
    }
}
