using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace Roc.Data.Sql
{
    public abstract class ExpressionVisitor<T>
    {
        protected ExpressionVisitor()
        {

        }

        protected virtual T Visit(Expression exp)
        {
            if (exp == null) return default(T);
            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return this.VisitUnary((UnaryExpression)exp);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return this.VisitBinary((BinaryExpression)exp);
                case ExpressionType.TypeIs:
                    return this.VisitTypeIs((TypeBinaryExpression)exp);
                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression)exp);
                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)exp);
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)exp);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);
                case ExpressionType.New:
                    return this.VisitNew((NewExpression)exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray((NewArrayExpression)exp);
                case ExpressionType.Invoke:
                    return this.VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.ListInit:
                    return this.VisitListInit((ListInitExpression)exp);
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }

        protected virtual T VisitBinding(MemberBinding binding)
        {
            return default(T);
        }

        protected virtual T VisitElementInitializer(ElementInit initializer)
        {
            return default(T);
        }

        protected virtual T VisitUnary(UnaryExpression u)
        {
            //var operand = this.Visit(u.Operand);
            //if (operand != u.Operand)
            //{
            //     Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method);
            //}
            //return u;
            return default(T);
        }

        protected virtual T VisitBinary(BinaryExpression b)
        {
            //Expression left = this.Visit(b.Left);
            //Expression right = this.Visit(b.Right);
            //Expression conversion = this.Visit(b.Conversion);
            //if (left != b.Left || right != b.Right || conversion != b.Conversion)
            //{
            //    if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
            //        return Expression.Coalesce(left, right, conversion as LambdaExpression);
            //    else
            //        return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
            //}
            //return b;
            return default(T);
        }

        protected virtual T VisitTypeIs(TypeBinaryExpression b)
        {
            return default(T);
        }

        protected virtual T VisitConstant(ConstantExpression c)
        {
            return default(T);
        }

        protected virtual T VisitConditional(ConditionalExpression c)
        {
            //Expression test = this.Visit(c.Test);
            //Expression ifTrue = this.Visit(c.IfTrue);
            //Expression ifFalse = this.Visit(c.IfFalse);
            //if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
            //{
            //    return Expression.Condition(test, ifTrue, ifFalse);
            //}
            //return c;
            return default(T);
        }

        protected virtual T VisitParameter(ParameterExpression p)
        {
            return default(T);
        }

        protected virtual T VisitMemberAccess(MemberExpression m)
        {
            //Expression exp = this.Visit(m.Expression);
            //if (exp != m.Expression)
            //{
            //    return Expression.MakeMemberAccess(exp, m.Member);
            //}
            //return m;
            return default(T);
        }

        protected virtual T VisitMethodCall(MethodCallExpression m)
        {
            //Expression obj = this.Visit(m.Object);
            //IEnumerable<Expression> args = this.VisitExpressionList(m.Arguments);
            //if (obj != m.Object || args != m.Arguments)
            //{
            //    return Expression.Call(obj, m.Method, args);
            //}
            //return m;
            return default(T);
        }

        protected virtual T VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            //List<Expression> list = null;
            //for (int i = 0, n = original.Count; i < n; i++)
            //{
            //    Expression p = this.Visit(original[i]);
            //    if (list != null)
            //    {
            //        list.Add(p);
            //    }
            //    else if (p != original[i])
            //    {
            //        list = new List<Expression>(n);
            //        for (int j = 0; j < i; j++)
            //        {
            //            list.Add(original[j]);
            //        }
            //        list.Add(p);
            //    }
            //}
            //if (list != null)
            //{
            //    return list.AsReadOnly();
            //}
            //return original;
            return default(T);
        }

        protected virtual T VisitMemberAssignment(MemberAssignment assignment)
        {
            //Expression e = this.Visit(assignment.Expression);
            //if (e != assignment.Expression)
            //{
            //    return Expression.Bind(assignment.Member, e);
            //}
            //return assignment;
            return default(T);
        }

        protected virtual T VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            //IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);
            //if (bindings != binding.Bindings)
            //{
            //    return Expression.MemberBind(binding.Member, bindings);
            //}
            //return binding;
            return default(T);
        }

        protected virtual T VisitMemberListBinding(MemberListBinding binding)
        {
            //IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);
            //if (initializers != binding.Initializers)
            //{
            //    return Expression.ListBind(binding.Member, initializers);
            //}
            //return binding;
            return default(T);
        }

        protected virtual T VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            //List<MemberBinding> list = null;
            //for (int i = 0, n = original.Count; i < n; i++)
            //{
            //    MemberBinding b = this.VisitBinding(original[i]);
            //    if (list != null)
            //    {
            //        list.Add(b);
            //    }
            //    else if (b != original[i])
            //    {
            //        list = new List<MemberBinding>(n);
            //        for (int j = 0; j < i; j++)
            //        {
            //            list.Add(original[j]);
            //        }
            //        list.Add(b);
            //    }
            //}
            //if (list != null)
            //    return list;
            //return original;
            return default(T);
        }

        protected virtual T VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            //List<ElementInit> list = null;
            //for (int i = 0, n = original.Count; i < n; i++)
            //{
            //    ElementInit init = this.VisitElementInitializer(original[i]);
            //    if (list != null)
            //    {
            //        list.Add(init);
            //    }
            //    else if (init != original[i])
            //    {
            //        list = new List<ElementInit>(n);
            //        for (int j = 0; j < i; j++)
            //        {
            //            list.Add(original[j]);
            //        }
            //        list.Add(init);
            //    }
            //}
            //if (list != null)
            //    return list;
            //return original;
            return default(T);
        }

        protected virtual T VisitLambda(LambdaExpression lambda)
        {
            //Expression body = this.Visit(lambda.Body);
            //if (body != lambda.Body)
            //{
            //    return Expression.Lambda(lambda.Type, body, lambda.Parameters);
            //}
            //return lambda;
            return default(T);
        }

        protected virtual T VisitNew(NewExpression nex)
        {
            //IEnumerable<Expression> args = this.VisitExpressionList(nex.Arguments);
            //if (args != nex.Arguments)
            //{
            //    if (nex.Members != null)
            //        return Expression.New(nex.Constructor, args, nex.Members);
            //    else
            //        return Expression.New(nex.Constructor, args);
            //}
            //return nex;
            return default(T);
        }

        protected virtual T VisitMemberInit(MemberInitExpression init)
        {
            //NewExpression n = this.VisitNew(init.NewExpression);
            //IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
            //if (n != init.NewExpression || bindings != init.Bindings)
            //{
            //    return Expression.MemberInit(n, bindings);
            //}
            //return init;
            return default(T);
        }

        protected virtual T VisitListInit(ListInitExpression init)
        {
            //NewExpression n = this.VisitNew(init.NewExpression);
            //IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);
            //if (n != init.NewExpression || initializers != init.Initializers)
            //{
            //    return Expression.ListInit(n, initializers);
            //}
            //return init;
            return default(T);
        }

        protected virtual T VisitNewArray(NewArrayExpression na)
        {
            //IEnumerable<Expression> exprs = this.VisitExpressionList(na.Expressions);
            //if (exprs != na.Expressions)
            //{
            //    if (na.NodeType == ExpressionType.NewArrayInit)
            //    {
            //        return Expression.NewArrayInit(na.Type.GetElementType(), exprs);
            //    }
            //    else
            //    {
            //        return Expression.NewArrayBounds(na.Type.GetElementType(), exprs);
            //    }
            //}
            //return na;
            return default(T);
        }

        protected virtual T VisitInvocation(InvocationExpression iv)
        {
            //IEnumerable<Expression> args = this.VisitExpressionList(iv.Arguments);
            //Expression expr = this.Visit(iv.Expression);
            //if (args != iv.Arguments || expr != iv.Expression)
            //{
            //    return Expression.Invoke(expr, args);
            //}
            //return iv;
            return default(T);
        }
    }
}
