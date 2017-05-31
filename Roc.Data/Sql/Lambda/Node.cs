using Roc.Data.Sql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Roc.Data
{
    /// <summary>
    /// 默认都是 internal
    /// </summary>
    public abstract class Node
    {

    }

    public class FieldNode : Node
    {
        private List<MemberNode> nodes = new List<MemberNode>();

        public IEnumerable<MemberNode> Nodes { get { return nodes; } }

        public FieldNode() { }

        public FieldNode(MemberNode node)
        {
            AddNode(node);
        }

        public FieldNode(IEnumerable<MemberNode> ns)
        {
            AddNodes(ns);
        }

        public void AddNode(MemberNode node)
        {
            this.nodes.Add(node);
        }

        public void AddNodes(IEnumerable<MemberNode> nodes)
        {
            this.nodes.AddRange(nodes);
        }

        public bool HasNode()
        {
            if (this.nodes != null && this.nodes.Count > 0) return true;
            return false;
        }
    }

    public class MemberNode : Node
    {
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public string AliasName { get; set; }

        public MemberNode() { }

        public MemberNode(Type type, string fname)
        {
            this.TableName = type.Name;
            this.FieldName = fname;
        }

        public MemberNode(string tname, string fname)
        {
            this.TableName = tname;
            this.FieldName = fname;
        }

        public MemberNode(string tname, string fname, string aname)
        {
            this.TableName = tname;
            this.FieldName = fname;
            this.AliasName = aname;
        }

        public MemberNode(MemberExpression member)
        {
            this.Init(member);
        }

        public MemberNode(MemberExpression member, Type resolveType)
        {
            var type = member.Member.DeclaringType;
            if (type == resolveType || resolveType.IsSubclassOf(type))
            {
                this.Init(member);
            }
        }

        private void Init(MemberExpression member)
        {
            this.TableName = ExpressionHelper.GetTableName(member);
            this.FieldName = ExpressionHelper.GetColumnName(member);
        }
    }

    public class ValueNode : Node
    {
        public object Value { get; set; }

        public ValueNode() { }

        public ValueNode(int obj)
        {
            this.Value = obj;
        }

        public ValueNode(object obj)
        {
            this.Value = obj;
        }
    }

    public class UnaryNode : Node
    {
        public ExpressionType Operator { get; set; }
        public Node Child { get; set; }
    }

    public class BinaryNode : Node
    {
        public ExpressionType Operator { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
    }

    public class LikeNode : Node
    {
        public SqlLikeType Method { get; set; }
        public MemberNode MemberNode { get; set; }
        public object Value { get; set; }

        public LikeNode()
        {

        }

        public LikeNode(SqlLikeType type, MemberNode node, object value)
        {
            this.Method = type;
            this.MemberNode = node;
            this.Value = value;
        }
    }

    public class InNode : Node
    {
        public MemberNode Node { get; set; }

        public SqlInType Type { get; set; }

        public IEnumerable<object> Values { get; set; }

        public InNode()
        {
            this.Type = SqlInType.IN;
        }

        public InNode(SqlInType type, MemberNode node, IEnumerable<object> values)
        {
            this.Type = type;
            this.Node = node;
            this.Values = values;
        }

        public InNode(MemberNode node, object value)
        {
            this.Node = node;
            this.Type = SqlInType.IN;
            this.Values = Utils.GetObjectList(value);
        }
    }

    public class MethodNode : Node
    {
        public MethodNode() { }

        public MethodNode(MemberNode node, string sql)
            : this(node, sql, null)
        {

        }
        public MethodNode(MemberNode node, string sql, object value)
        {
            this.Member = node;
            this.Sql = sql;
            this.Value = value;
        }
        /// <summary>
        /// 操作的字段
        /// </summary>
        public MemberNode Member { get; set; }
        /// <summary>
        /// 操作的部分sql语句，支持格式化 第一个参数为 字段 ，第二个参数为 参数的Key 如
        /// Sql ="{0} = {1}"; //最终结果为 字段A = @字段A_qqww34  
        /// @字段A_qqww34 的值为 Value 的值
        /// </summary>
        public string Sql { get; set; }
        /// <summary>
        /// 字段的值 如果没有值 则为null
        /// </summary>
        public object Value { get; set; }
    }
}
