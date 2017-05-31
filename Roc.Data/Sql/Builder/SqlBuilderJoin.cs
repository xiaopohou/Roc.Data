using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Roc.Data.Sql
{
    internal partial class SqlBuilder
    {
        public void BuildJoin(BinaryNode node)
        {
            BuildJoin((dynamic)node.Left, (dynamic)node.Right, node.Operator);
        }

        public void BuildJoin(Node node)
        {
            throw new ArgumentException("不能解析Join条件表达式", node.ToString());
        }

        public void BuildJoinTable(SqlJoinType type, Type tableType, string aliasName)
        {
            string tableName = this.GetTableName(tableType, aliasName);
            string joinString = string.Format("{0} JOIN {1} ON", type.ToString(), tableName);
            _joins.Add(joinString);
        }

        private void BuildJoin(Node leftNode, Node rightNode, ExpressionType op)
        {
            if (leftNode == null) return;
            if (rightNode == null) return;
            BuildJoin((dynamic)leftNode);
            CheckOperation(op);
            _joins.Add(_operations[op]);
            BuildJoin((dynamic)rightNode);
        }

        private void BuildJoin(MemberNode node, ValueNode valueNode, ExpressionType op)
        {
            string left = this.GetFieldName(node.TableName, node.FieldName);
            string pname = this.GetParameter("RIGHT");
            this.AddParameter(pname, valueNode.Value);
            this.BuildJoinSql(left, pname, _operations[op]);
        }

        private void BuildJoin(ValueNode valueNode, MemberNode node, ExpressionType op)
        {
            BuildJoin(node, valueNode, op);
        }

        private void BuildJoin(MemberNode leftMember, MemberNode rightMember, ExpressionType op)
        {
            string left = this.GetFieldName(leftMember.TableName, leftMember.FieldName);
            string right = this.GetFieldName(rightMember.TableName, rightMember.FieldName);
            this.BuildJoinSql(left, right, _operations[op]);
        }

        private void BuildJoinSql(string leftName, string rightName, string op)
        {
            string joinString = string.Format("{0} {1} {2}", leftName, op, rightName);
            _joins.Add(joinString);
        }
    }
}
