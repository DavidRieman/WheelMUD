//-----------------------------------------------------------------------------
// <copyright file="ExpressionComparer.cs" company="http://rulesengine.codeplex.com">
//   Copyright (c) athoma13. See RulesEngine_License.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: athoma13
//   Date      : Fri Sep 30 2011
//   Purpose   : Rule Engine
// </summary>
// <history>
//   Sat Jan 28 2012 by Fastalanasa - Added to WheelMUD.Rules
// </history>
//-----------------------------------------------------------------------------

namespace WheelMUD.Rules
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    public class ExpressionComparer : IEqualityComparer<Expression>
    {
        public bool Compare(Expression node1, Expression node2)
        {
            object state = CreateCompareState(node1, node2);
            return Compare(node1, node2, state);
        }

        public bool Equals(Expression x, Expression y)
        {
            return Compare(x, y);
        }

        public int GetHashCode(Expression obj)
        {
            object state = CreateHashState(obj);
            return GetHashCode(obj, state);
        }

        protected bool CompareMany<T>(IEnumerable<T> nodes1, IEnumerable<T> nodes2, object state, Func<T, T, object, bool> compareDelegate)
            where T : class
        {
            if (AreBothNull(nodes1, nodes2)) return true;
            if (AreEitherNull(nodes1, nodes2)) return false;

            var en1 = nodes1.GetEnumerator();
            var en2 = nodes2.GetEnumerator();
            try
            {
                while (true)
                {
                    var moved = false;
                    var exp1 = (moved |= en1.MoveNext()) ? en1.Current : null;
                    var exp2 = (moved |= en2.MoveNext()) ? en2.Current : null;

                    if (!moved) return true;
                    if (!compareDelegate(exp1, exp2, state)) return false;
                }
            }
            finally
            {
                en1.Dispose();
                en2.Dispose();
            }
        }

        protected bool AreBothNull<T>(T arg1, T arg2)
            where T : class
        {
            return arg1 == null && arg2 == null;
        }

        protected bool AreEitherNull<T>(T arg1, T arg2)
            where T : class
        {
            return arg1 == null || arg2 == null;
        }

        protected virtual bool CompareType(Type node1, Type node2, object state)
        {
            return node1 == node2;
        }

        protected virtual bool CompareMemberInfo(MemberInfo node1, MemberInfo node2, object state)
        {
            return node1 == node2;
        }

        protected virtual bool CompareObject(object node1, object node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return node1.Equals(node2);
        }

        protected virtual bool CompareBinary(BinaryExpression node1, BinaryExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareMemberInfo(node1.Method, node2.Method, state)
                && Compare(node1.Left, node2.Left, state)
                && CompareLambda(node1.Conversion, node2.Conversion, state)
                && Compare(node1.Right, node2.Right, state);
        }

        protected virtual bool CompareBlock(BlockExpression node1, BlockExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareType(node1.Type, node2.Type, state)
                && CompareMany(node1.Variables, node2.Variables, state, CompareParameter)
                && CompareMany(node1.Expressions, node2.Expressions, state, Compare);
        }

        protected virtual bool CompareCatchBlock(CatchBlock node1, CatchBlock node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareType(node1.Test, node2.Test, state)
                && Compare(node1.Body, node2.Body, state)
                && CompareParameter(node1.Variable, node2.Variable, state)
                && Compare(node1.Filter, node2.Filter, state);
        }

        protected virtual bool CompareConditional(ConditionalExpression node1, ConditionalExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return Compare(node1.Test, node2.Test, state)
                && Compare(node1.IfTrue, node2.IfTrue, state)
                && Compare(node1.IfFalse, node2.IfFalse, state);
        }

        protected virtual bool CompareConstant(ConstantExpression node1, ConstantExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareType(node1.Type, node2.Type, state)
                && CompareObject(node1.Value, node2.Value, state);
        }

        protected virtual bool CompareDebugInfo(DebugInfoExpression node1, DebugInfoExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return node1.Document.FileName == node2.Document.FileName
                && node1.EndColumn == node2.EndColumn
                && node1.EndLine == node2.EndLine
                && node1.StartColumn == node2.StartColumn
                && node1.StartLine == node2.StartLine;
        }

        protected virtual bool CompareDefault(DefaultExpression node1, DefaultExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareType(node1.Type, node2.Type, state);
        }

        protected virtual bool CompareDynamic(DynamicExpression node1, DynamicExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareType(node1.Type, node2.Type, state)
                && CompareType(node1.DelegateType, node2.DelegateType, state)
                && CompareMany(node1.Arguments, node2.Arguments, state, Compare);
        }

        protected virtual bool CompareElementInit(ElementInit node1, ElementInit node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareMemberInfo(node1.AddMethod, node2.AddMethod, state)
                && CompareMany(node1.Arguments, node2.Arguments, state, Compare);
        }

        protected virtual bool CompareGoto(GotoExpression node1, GotoExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return node1.Kind == node2.Kind
                && CompareType(node1.Type, node2.Type, state)
                && CompareLabelTarget(node1.Target, node2.Target, state)
                && Compare(node1.Value, node2.Value, state);
        }

        protected virtual bool CompareIndex(IndexExpression node1, IndexExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareType(node1.Type, node2.Type, state)
                && CompareMemberInfo(node1.Indexer, node2.Indexer, state)
                && Compare(node1.Object, node2.Object, state)
                && CompareMany(node1.Arguments, node2.Arguments, state, Compare);
        }

        protected virtual bool CompareInvocation(InvocationExpression node1, InvocationExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareType(node1.Type, node2.Type, state)
                && Compare(node1.Expression, node2.Expression, state)
                && CompareMany(node1.Arguments, node2.Arguments, state, Compare);
        }

        protected virtual bool CompareLabel(LabelExpression node1, LabelExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareType(node1.Type, node2.Type, state)
                && CompareLabelTarget(node1.Target, node2.Target, state)
                && Compare(node1.DefaultValue, node2.DefaultValue, state);
        }

        protected virtual bool CompareLabelTarget(LabelTarget node1, LabelTarget node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return node1.Name == node2.Name
                && CompareType(node1.Type, node2.Type, state);
        }

        protected virtual bool CompareLambda(LambdaExpression node1, LambdaExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareType(node1.Type, node2.Type, state)
                && CompareType(node1.ReturnType, node2.ReturnType, state)
                && CompareMany(node1.Parameters, node2.Parameters, state, CompareParameter)
                && Compare(node1.Body, node2.Body, state);
        }

        protected virtual bool CompareListInit(ListInitExpression node1, ListInitExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareType(node1.Type, node2.Type, state)
                && CompareNew(node1.NewExpression, node2.NewExpression, state)
                && CompareMany(node1.Initializers, node2.Initializers, state, CompareElementInit);
        }

        protected virtual bool CompareLoop(LoopExpression node1, LoopExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareType(node1.Type, node2.Type, state)
                && CompareLabelTarget(node1.BreakLabel, node2.BreakLabel, state)
                && CompareLabelTarget(node1.ContinueLabel, node2.ContinueLabel, state)
                && Compare(node1.Body, node2.Body, state);
        }

        protected virtual bool CompareMember(MemberExpression node1, MemberExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareType(node1.Type, node2.Type, state)
                && CompareMemberInfo(node1.Member, node2.Member, state)
                && Compare(node1.Expression, node2.Expression, state);
        }

        protected virtual bool CompareMemberAssignment(MemberAssignment node1, MemberAssignment node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareMemberInfo(node1.Member, node2.Member, state)
                && Compare(node1.Expression, node2.Expression, state);
        }

        protected virtual bool CompareMemberBinding(MemberBinding node1, MemberBinding node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareMemberInfo(node1.Member, node2.Member, state);
        }

        protected virtual bool CompareMemberInit(MemberInitExpression node1, MemberInitExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareType(node1.Type, node2.Type, state)
                && CompareNew(node1.NewExpression, node2.NewExpression, state);
        }

        protected virtual bool CompareMemberListBinding(MemberListBinding node1, MemberListBinding node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareMemberInfo(node1.Member, node2.Member, state)
                && CompareMany(node1.Initializers, node2.Initializers, state, CompareElementInit);
        }

        protected virtual bool CompareMemberMemberBinding(MemberMemberBinding node1, MemberMemberBinding node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareMemberInfo(node1.Member, node2.Member, state)
                && CompareMany(node1.Bindings, node2.Bindings, state, CompareMemberBinding);
        }

        protected virtual bool CompareMethodCall(MethodCallExpression node1, MethodCallExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareType(node1.Type, node2.Type, state)
                && CompareMemberInfo(node1.Method, node2.Method, state)
                && Compare(node1.Object, node2.Object, state)
                && CompareMany(node1.Arguments, node2.Arguments, state, Compare);
        }

        protected virtual bool CompareNew(NewExpression node1, NewExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareMemberInfo(node1.Constructor, node2.Constructor, state)
                && CompareMany(node1.Arguments, node2.Arguments, state, Compare);
        }

        protected virtual bool CompareNewArray(NewArrayExpression node1, NewArrayExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareType(node1.Type, node2.Type, state)
                && CompareMany(node1.Expressions, node2.Expressions, state, Compare);
        }

        protected virtual bool CompareParameter(ParameterExpression node1, ParameterExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareType(node1.Type, node2.Type, state) && node1.IsByRef == node2.IsByRef && node1.Name == node2.Name;
        }

        protected virtual bool CompareRuntimeVariables(RuntimeVariablesExpression node1, RuntimeVariablesExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareType(node1.Type, node2.Type, state) && CompareMany(node1.Variables, node2.Variables, state, CompareParameter);
        }

        protected virtual bool CompareSwitch(SwitchExpression node1, SwitchExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return Compare(node1.SwitchValue, node2.SwitchValue, state)
                && CompareMany(node1.Cases, node2.Cases, state, CompareSwitchCase)
                && CompareMemberInfo(node1.Comparison, node2.Comparison, state)
                && Compare(node1.DefaultBody, node2.DefaultBody, state);
        }

        protected virtual bool CompareSwitchCase(SwitchCase node1, SwitchCase node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return Compare(node1.Body, node2.Body, state)
                && CompareMany(node1.TestValues, node2.TestValues, state, Compare);
        }

        protected virtual bool CompareTry(TryExpression node1, TryExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return Compare(node1.Body, node2.Body, state)
                && Compare(node1.Fault, node2.Fault, state)
                && Compare(node1.Finally, node2.Finally, state)
                && CompareMany(node1.Handlers, node2.Handlers, state, CompareCatchBlock);
        }

        protected virtual bool CompareTypeBinary(TypeBinaryExpression node1, TypeBinaryExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return Compare(node1.Expression, node2.Expression, state) && CompareType(node1.TypeOperand, node2.TypeOperand, state);
        }

        protected virtual bool CompareUnary(UnaryExpression node1, UnaryExpression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;
            if (AreEitherNull(node1, node2)) return false;

            return CompareMemberInfo(node1.Method, node2.Method, state) && Compare(node1.Operand, node2.Operand, state);
        }

        protected virtual bool Compare(Expression node1, Expression node2, object state)
        {
            if (AreBothNull(node1, node2)) return true;

            // Let specific method decide if (AreEitherNull(node1, node2)) return false;
            if (node1 is BinaryExpression && node2 is BinaryExpression) return CompareBinary((BinaryExpression)node1, (BinaryExpression)node2, state);
            if (node1 is BlockExpression && node2 is BlockExpression) return CompareBlock((BlockExpression)node1, (BlockExpression)node2, state);
            if (node1 is ConditionalExpression && node2 is ConditionalExpression) return CompareConditional((ConditionalExpression)node1, (ConditionalExpression)node2, state);
            if (node1 is ConstantExpression && node2 is ConstantExpression) return CompareConstant((ConstantExpression)node1, (ConstantExpression)node2, state);
            if (node1 is DebugInfoExpression && node2 is DebugInfoExpression) return CompareDebugInfo((DebugInfoExpression)node1, (DebugInfoExpression)node2, state);
            if (node1 is DefaultExpression && node2 is DefaultExpression) return CompareDefault((DefaultExpression)node1, (DefaultExpression)node2, state);
            if (node1 is DynamicExpression && node2 is DynamicExpression) return CompareDynamic((DynamicExpression)node1, (DynamicExpression)node2, state);
            if (node1 is GotoExpression && node2 is GotoExpression) return CompareGoto((GotoExpression)node1, (GotoExpression)node2, state);
            if (node1 is IndexExpression && node2 is IndexExpression) return CompareIndex((IndexExpression)node1, (IndexExpression)node2, state);
            if (node1 is InvocationExpression && node2 is InvocationExpression) return CompareInvocation((InvocationExpression)node1, (InvocationExpression)node2, state);
            if (node1 is LabelExpression && node2 is LabelExpression) return CompareLabel((LabelExpression)node1, (LabelExpression)node2, state);
            if (node1 is LambdaExpression && node2 is LambdaExpression) return CompareLambda((LambdaExpression)node1, (LambdaExpression)node2, state);
            if (node1 is ListInitExpression && node2 is ListInitExpression) return CompareListInit((ListInitExpression)node1, (ListInitExpression)node2, state);
            if (node1 is LoopExpression && node2 is LoopExpression) return CompareLoop((LoopExpression)node1, (LoopExpression)node2, state);
            if (node1 is MemberExpression && node2 is MemberExpression) return CompareMember((MemberExpression)node1, (MemberExpression)node2, state);
            if (node1 is MemberInitExpression && node2 is MemberInitExpression) return CompareMemberInit((MemberInitExpression)node1, (MemberInitExpression)node2, state);
            if (node1 is MethodCallExpression && node2 is MethodCallExpression) return CompareMethodCall((MethodCallExpression)node1, (MethodCallExpression)node2, state);
            if (node1 is NewExpression && node2 is NewExpression) return CompareNew((NewExpression)node1, (NewExpression)node2, state);
            if (node1 is NewArrayExpression && node2 is NewArrayExpression) return CompareNewArray((NewArrayExpression)node1, (NewArrayExpression)node2, state);
            if (node1 is ParameterExpression && node2 is ParameterExpression) return CompareParameter((ParameterExpression)node1, (ParameterExpression)node2, state);
            if (node1 is RuntimeVariablesExpression && node2 is RuntimeVariablesExpression) return CompareRuntimeVariables((RuntimeVariablesExpression)node1, (RuntimeVariablesExpression)node2, state);
            if (node1 is SwitchExpression && node2 is SwitchExpression) return CompareSwitch((SwitchExpression)node1, (SwitchExpression)node2, state);
            if (node1 is TryExpression && node2 is TryExpression) return CompareTry((TryExpression)node1, (TryExpression)node2, state);
            if (node1 is TypeBinaryExpression && node2 is TypeBinaryExpression) return CompareTypeBinary((TypeBinaryExpression)node1, (TypeBinaryExpression)node2, state);
            if (node1 is UnaryExpression && node2 is UnaryExpression) return CompareUnary((UnaryExpression)node1, (UnaryExpression)node2, state);

            return false;
        }

        protected int CombineHash(int hashcode, params int[] otherHashes)
        {
            return Utilities.CombineHash(hashcode, otherHashes);
        }

        protected int GetHashCode<T>(IEnumerable<T> nodes, object state, Func<T, object, int> getHashCodeDelegate)
            where T : class
        {
            if (nodes == null) return 0;
            int result = 0;
            foreach (T node in nodes)
            {
                result = CombineHash(result, getHashCodeDelegate(node, state));
            }

            return result;
        }

        protected virtual int GetHashCode(Type node, object state)
        {
            if (node == null) return 0;

            return node.GetHashCode();
        }

        protected virtual int GetHashCode(MemberInfo node, object state)
        {
            if (node == null) return 0;

            return node.GetHashCode();
        }

        protected virtual int GetHashCode(object node, object state)
        {
            if (node == null) return 0;

            return node.GetHashCode();
        }

        protected virtual int GetHashCode(BinaryExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Method, state), GetHashCode(node.Left, state), GetHashCode(node.Conversion, state), GetHashCode(node.Right, state));
        }

        protected virtual int GetHashCode(BlockExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Type, state), GetHashCode(node.Variables, state, GetHashCode), GetHashCode(node.Expressions, state, GetHashCode));
        }

        protected virtual int GetHashCode(CatchBlock node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Test, state), GetHashCode(node.Body, state), GetHashCode(node.Variable, state), GetHashCode(node.Filter, state));
        }

        protected virtual int GetHashCode(ConditionalExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Test, state), GetHashCode(node.IfTrue, state), GetHashCode(node.IfFalse, state));
        }

        protected virtual int GetHashCode(ConstantExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Type, state), GetHashCode(node.Value, state));
        }

        protected virtual int GetHashCode(DebugInfoExpression node, object state)
        {
            if (node == null) return 0;

            var hash = node.Document != null ? (node.Document.FileName ?? string.Empty).GetHashCode() : 0;
            return CombineHash(hash, node.EndColumn, node.EndLine, node.StartColumn, node.StartLine);
        }

        protected virtual int GetHashCode(DefaultExpression node, object state)
        {
            if (node == null) return 0;
            return GetHashCode(node.Type, state);
        }

        protected virtual int GetHashCode(DynamicExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Type, state), GetHashCode(node.DelegateType, state), GetHashCode(node.Arguments, state, GetHashCode));
        }

        protected virtual int GetHashCode(ElementInit node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.AddMethod, state), GetHashCode(node.Arguments, state, GetHashCode));
        }

        protected virtual int GetHashCode(GotoExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(node.Kind.GetHashCode(), GetHashCode(node.Type, state), GetHashCode(node.Target, state), GetHashCode(node.Value, state));
        }

        protected virtual int GetHashCode(IndexExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Type, state), GetHashCode(node.Indexer, state), GetHashCode(node.Object, state), GetHashCode(node.Arguments, state, GetHashCode));
        }

        protected virtual int GetHashCode(InvocationExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Type, state), GetHashCode(node.Expression, state), GetHashCode(node.Arguments, state, GetHashCode));
        }

        protected virtual int GetHashCode(LabelExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Type, state), GetHashCode(node.Target, state), GetHashCode(node.DefaultValue, state));
        }

        protected virtual int GetHashCode(LabelTarget node, object state)
        {
            if (node == null) return 0;

            return CombineHash((node.Name ?? string.Empty).GetHashCode(), GetHashCode(node.Type, state));
        }

        protected virtual int GetHashCode(LambdaExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Type, state), GetHashCode(node.ReturnType, state), GetHashCode(node.Parameters, state, GetHashCode), GetHashCode(node.Body, state));
        }

        protected virtual int GetHashCode(ListInitExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Type, state), GetHashCode(node.NewExpression, state), GetHashCode(node.Initializers, state, GetHashCode));
        }

        protected virtual int GetHashCode(LoopExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Type, state), GetHashCode(node.BreakLabel, state), GetHashCode(node.ContinueLabel, state), GetHashCode(node.Body, state));
        }

        protected virtual int GetHashCode(MemberExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Type, state), GetHashCode(node.Member, state), GetHashCode(node.Expression, state));
        }

        protected virtual int GetHashCode(MemberAssignment node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Member, state), GetHashCode(node.Expression, state));
        }

        protected virtual int GetHashCode(MemberBinding node, object state)
        {
            if (node == null) return 0;

            return GetHashCode(node.Member, state);
        }

        protected virtual int GetHashCode(MemberInitExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Type, state), GetHashCode(node.NewExpression, state));
        }

        protected virtual int GetHashCode(MemberListBinding node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Member, state), GetHashCode(node.Initializers, state, GetHashCode));
        }

        protected virtual int GetHashCode(MemberMemberBinding node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Member, state), GetHashCode(node.Bindings, state, GetHashCode));
        }

        protected virtual int GetHashCode(MethodCallExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Type, state), GetHashCode(node.Method, state), GetHashCode(node.Object, state), GetHashCode(node.Arguments, state, GetHashCode));
        }

        protected virtual int GetHashCode(NewExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Constructor, state), GetHashCode(node.Arguments, state, GetHashCode));
        }

        protected virtual int GetHashCode(NewArrayExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Type, state), GetHashCode(node.Expressions, state, GetHashCode));
        }

        protected virtual int GetHashCode(ParameterExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Type, state), node.IsByRef.GetHashCode(), (node.Name ?? string.Empty).GetHashCode());
        }

        protected virtual int GetHashCode(RuntimeVariablesExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Type, state), GetHashCode(node.Variables, state, GetHashCode));
        }

        protected virtual int GetHashCode(SwitchExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.SwitchValue, state), GetHashCode(node.Cases, state, GetHashCode), GetHashCode(node.Comparison, state), GetHashCode(node.DefaultBody, state));
        }

        protected virtual int GetHashCode(SwitchCase node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Body, state), GetHashCode(node.TestValues, state, GetHashCode));
        }

        protected virtual int GetHashCode(TryExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Body, state), GetHashCode(node.Fault, state), GetHashCode(node.Finally, state), GetHashCode(node.Handlers, state, GetHashCode));
        }

        protected virtual int GetHashCode(TypeBinaryExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Expression, state), GetHashCode(node.TypeOperand, state));
        }

        protected virtual int GetHashCode(UnaryExpression node, object state)
        {
            if (node == null) return 0;

            return CombineHash(GetHashCode(node.Method, state), GetHashCode(node.Operand, state));
        }

        protected virtual int GetHashCode(Expression node, object state)
        {
            if (node == null) return 0;
            if (node is BinaryExpression) return GetHashCode((BinaryExpression)node, state);
            if (node is BlockExpression) return GetHashCode((BlockExpression)node, state);
            if (node is ConditionalExpression) return GetHashCode((ConditionalExpression)node, state);
            if (node is ConstantExpression) return GetHashCode((ConstantExpression)node, state);
            if (node is DebugInfoExpression) return GetHashCode((DebugInfoExpression)node, state);
            if (node is DefaultExpression) return GetHashCode((DefaultExpression)node, state);
            if (node is DynamicExpression) return GetHashCode((DynamicExpression)node, state);
            if (node is GotoExpression) return GetHashCode((GotoExpression)node, state);
            if (node is IndexExpression) return GetHashCode((IndexExpression)node, state);
            if (node is InvocationExpression) return GetHashCode((InvocationExpression)node, state);
            if (node is LabelExpression) return GetHashCode((LabelExpression)node, state);
            if (node is LambdaExpression) return GetHashCode((LambdaExpression)node, state);
            if (node is ListInitExpression) return GetHashCode((ListInitExpression)node, state);
            if (node is LoopExpression) return GetHashCode((LoopExpression)node, state);
            if (node is MemberExpression) return GetHashCode((MemberExpression)node, state);
            if (node is MemberInitExpression) return GetHashCode((MemberInitExpression)node, state);
            if (node is MethodCallExpression) return GetHashCode((MethodCallExpression)node, state);
            if (node is NewExpression) return GetHashCode((NewExpression)node, state);
            if (node is NewArrayExpression) return GetHashCode((NewArrayExpression)node, state);
            if (node is ParameterExpression) return GetHashCode((ParameterExpression)node, state);
            if (node is RuntimeVariablesExpression) return GetHashCode((RuntimeVariablesExpression)node, state);
            if (node is SwitchExpression) return GetHashCode((SwitchExpression)node, state);
            if (node is TryExpression) return GetHashCode((TryExpression)node, state);
            if (node is TypeBinaryExpression) return GetHashCode((TypeBinaryExpression)node, state);
            if (node is UnaryExpression) return GetHashCode((UnaryExpression)node, state);
            return 0;
        }

        protected virtual object CreateCompareState(Expression node1, Expression node2)
        {
            return null;
        }

        protected virtual object CreateHashState(Expression node)
        {
            return null;
        }
    }
}