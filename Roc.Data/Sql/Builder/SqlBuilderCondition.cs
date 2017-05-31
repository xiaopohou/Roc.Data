using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Roc.Data.Sql;
using System.Text.RegularExpressions;

namespace Roc.Data.Sql
{
    internal partial class SqlBuilder
    {
        #region  private method
        void AddCondition(object obj)
        {
            string fieldName = "1";
            bool flag = Convert.ToBoolean(obj);
            ExpressionType type = flag ? ExpressionType.Equal : ExpressionType.NotEqual;
            this.AddCondition(fieldName, _operations[type], fieldName);
        }

        void AddCondition(string fieldName, string op, object fieldValue)
        {
            if (partType == SqlPartType.Having)
            {
                if (functionType != SqlFunctionType.NONE)
                    fieldName = string.Format("{0}({1})", functionType.ToString(), fieldName);
            }
            string condition = string.Format("{0} {1} {2}", fieldName, op, fieldValue);
            this.AppendCondition(condition);
        }

        void AddCondition(string tableName, string fieldName, string op, object fieldValue)
        {
            string key = this.GetFieldName(tableName, fieldName);
            string value = this.GetParameter(fieldName);
            this.AddParameter(value, fieldValue);
            this.AddCondition(key, op, value);
        }

        #endregion

        public void BuildWhere(Node node)
        {
            if (node == null) return;
            BuildWhere((dynamic)node);
        }

        public void BuildWhere(ValueNode node)
        {
            this.AddCondition(node.Value);
        }

        public void BuildWhere(LikeNode node)
        {
            object v = node.Value;
            string sv = node.Value.ToString();
            switch (node.Method)
            {
                case SqlLikeType.StartsWith:
                    v = sv + LikeSymbol;
                    break;
                case SqlLikeType.EndsWith:
                    v = LikeSymbol + sv;
                    break;
                case SqlLikeType.Contains:
                    v = LikeSymbol + sv + LikeSymbol;
                    break;
            }
            AddCondition(node.MemberNode.TableName, node.MemberNode.FieldName, _methods[node.Method], v);
        }

        public void BuildWhere(InNode node)
        {
            var values = node.Values;
            if (values != null && values.Count() > 0)
            {
                int index = 0;
                var paramIds = values.Select(m =>
                {
                    string p = this.GetParameter(node.Node.FieldName, ++index);
                    this.AddParameter(p, m);
                    return p;
                });
                string fieldName = this.GetFieldName(node.Node.TableName, node.Node.FieldName);
                string op = node.Type == SqlInType.IN ? InSymbol : NotInSymbol;
                var condition = string.Format("{0} {1} ({2})", fieldName, op, string.Join(",", paramIds));
                this.AppendCondition(condition);
            }
        }

        public void BuildWhereIn<T>(MemberNode node, SqlInType type, IQuery<T> sql)
        {
            string sqlString = sql.GetSql();
            var parameters = sql.GetParameters();
            foreach (var param in parameters)
            {
                //var innerParamKey = "Inner" + param.Key;
                //innerQuery = Regex.Replace(innerQuery, param.Key, innerParamKey);
                AddParameter(param.Key, param.Value);
            }
            string fieldName = this.GetFieldName(node.TableName, node.FieldName);
            string op = type == SqlInType.IN ? InSymbol : NotInSymbol;
            var condition = string.Format("{0} {1} ({2})", fieldName, op, sqlString);
            this.AppendCondition(condition);
        }

        public void BuildWhere(BinaryNode node)
        {
            BuildWhere((dynamic)node.Left, (dynamic)node.Right, node.Operator);
        }

        public void BuildWhere(MemberNode memberNode)
        {
            AddCondition(memberNode.TableName, memberNode.FieldName, _operations[ExpressionType.Equal], true);
        }

        public void BuildWhere(UnaryNode node)
        {
            if (node.Operator == ExpressionType.Not)
            {
                if (node.Child is InNode)
                {
                    (node.Child as InNode).Type = SqlInType.NOTIN;
                }
                else this.Not();
            }
            BuildWhere(node.Child);
        }

        public void BuildWhere(MethodNode node)
        {
            string sql = node.Sql;
            if (string.IsNullOrEmpty(sql)) return;

            string fieldName = this.GetFieldName(node.Member.TableName, node.Member.FieldName);
            string p = this.GetParameter(node.Member.FieldName);
            object value = node.Value;
            if (value != null)
            {
                this.AddParameter(p, value);
            }
            string condition = string.Format(sql, fieldName, p);
            this.AppendCondition(condition);
        }

        #region InNode
        void BuildWhere(InNode node, ValueNode valueNode, ExpressionType op)
        {
            bool flag = Convert.ToBoolean(valueNode.Value);
            node.Type = flag ? SqlInType.IN : SqlInType.NOTIN;
            BuildWhere(node);
        }

        void BuildWhere(ValueNode valueNode, InNode node, ExpressionType op)
        {
            this.BuildWhere(node, valueNode, op);
        }

        void BuildWhere(InNode node, UnaryNode n, ExpressionType op)
        {
            BuildWhere(node);
        }

        void BuildWhere(UnaryNode n, InNode node, ExpressionType op)
        {
            BuildWhere(node);
        }

        void BuildWhere(InNode node, MemberNode n, ExpressionType op)
        {
            BuildWhere(node);
        }

        void BuildWhere(MemberNode n, InNode node, ExpressionType op)
        {
            BuildWhere(node);
        }

        #endregion

        #region MemberNode
        void BuildWhere(MemberNode memberNode, ValueNode valueNode, ExpressionType op)
        {
            if (valueNode.Value == null)
            {
                this.AddCondition(GetFieldName(memberNode.TableName, memberNode.FieldName), _noperations[op], string.Empty);
            }
            else
            {
                AddCondition(memberNode.TableName, memberNode.FieldName, _operations[op], valueNode.Value);
            }
        }

        void BuildWhere(ValueNode valueNode, MemberNode memberNode, ExpressionType op)
        {
            BuildWhere(memberNode, valueNode, op);
        }

        void BuildWhere(MemberNode leftMember, MemberNode rightMember, ExpressionType op)
        {
            this.AddCondition(GetFieldName(leftMember.TableName, leftMember.FieldName), _operations[op], GetFieldName(rightMember.TableName, rightMember.FieldName));
        }

        #endregion

        #region SingleOperationNode
        void BuildWhere(UnaryNode leftMember, Node rightMember, ExpressionType op)
        {
            if (leftMember.Operator == ExpressionType.Not)
                BuildWhere(leftMember as Node, rightMember, op);
            else
                BuildWhere((dynamic)leftMember.Child, (dynamic)rightMember, op);
        }

        void BuildWhere(Node leftMember, UnaryNode rightMember, ExpressionType op)
        {
            BuildWhere(rightMember, leftMember, op);
        }
        #endregion

        #region ValueNode
        void BuildWhere(ValueNode left, ValueNode right, ExpressionType op)
        {
            this.AddCondition(GetFormatValue(left.Value), _operations[op], GetFormatValue(right.Value));
        }
        #endregion

        void BuildWhere(Node leftNode, Node rightNode, ExpressionType op)
        {
            this.BeginExpression();
            BuildWhere((dynamic)leftNode);
            this.CheckOperation(op);
            this.AppendCondition(string.Format(" {0} ", _operations[op]));
            BuildWhere((dynamic)rightNode);
            this.EndExpression();
        }

        void CheckOperation(ExpressionType op)
        {
            switch (op)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    break;
                default:
                    throw new ArgumentException(string.Format("无法识别的二元表达式操作符-'{0}'", op.ToString()));
            }
        }

        string GetFormatValue(object obj)
        {
            if (obj == null) return NullSymbol;
            Type type = obj.GetType();
            if (type.IsValueType)
            {
                if (Type.GetTypeCode(type) < TypeCode.DateTime) return obj.ToString();
            }
            return string.Format("'{0}'", obj.ToString());
        }
    }
}
