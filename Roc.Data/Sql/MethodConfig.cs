using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Roc.Data
{
    public class MethodConfig
    {
        private static Dictionary<string, Func<Expression, IEnumerable<Expression>, MethodCallExpression, Node>> dic;

        static MethodConfig()
        {
            dic = new Dictionary<string, Func<Expression, IEnumerable<Expression>, MethodCallExpression, Node>>();
            AddMethod(typeof(string), "IsNullOrEmpty", IsNullOrEmpty);
        }

        public static void AddMethod(Type type, string methodName, Func<Expression, IEnumerable<Expression>, MethodCallExpression, Node> func)
        {
            string name = string.Format("{0}.{1}", type.FullName, methodName);
            bool flag = dic.ContainsKey(name);
            if (flag) dic.Remove(name);
            dic.Add(name, func);
        }

        public static Func<Expression, IEnumerable<Expression>, MethodCallExpression, Node> GetMethodHandler(Type type, string methodName)
        {
            string name = string.Format("{0}.{1}", type.FullName, methodName);
            Func<Expression, IEnumerable<Expression>, MethodCallExpression, Node> func;
            if (dic.TryGetValue(name, out func)) return func;
            return null;
        }

        private static Node IsNullOrEmpty(Expression obj, IEnumerable<Expression> args, MethodCallExpression m)
        {
            var member = ExpressionHelper.GetMemberExpression(args.FirstOrDefault());
            if (member != null)
            {
                var node = new MethodNode(new MemberNode(member), "ISNULL({0},'')=''");
                return node;
            }
            return null;
        }
    }
}
