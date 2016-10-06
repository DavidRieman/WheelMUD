//-----------------------------------------------------------------------------
// <copyright file="ExpressionComparerIgnoreLambdaParameterNames.cs" company="http://rulesengine.codeplex.com">
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
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class ExpressionComparerIgnoreLambdaParameterNames : ExpressionComparer
    {
        protected override object CreateCompareState(Expression node1, Expression node2)
        {
            return new List<ParameterExpression>[] { new List<ParameterExpression>(), new List<ParameterExpression>() };
        }

        protected override bool CompareParameter(ParameterExpression node1, ParameterExpression node2, object state)
        {
            if (this.AreBothNull(node1, node2))
            {
                return true;
            }

            if (this.AreEitherNull(node1, node2))
            {
                return false;
            }

            var index1 = this.GetCompareState(state, 0).IndexOf(node1);
            var index2 = this.GetCompareState(state, 1).IndexOf(node2);

            if (index1 == -1 && index2 == -1)
            {
                return base.CompareParameter(node1, node2, state);
            }
            else
            {
                // The Type of the parameter is irrelevant here. The CompareLambda has already ensured parameter types.
                // Only make sure that the parameters are found at identical positions in their respective lists.
                return index1 == index2;
            }
        }

        protected override bool CompareLambda(LambdaExpression node1, LambdaExpression node2, object state)
        {
            if (this.AreBothNull(node1, node2))
            {
                return true;
            }

            if (this.AreEitherNull(node1, node2))
            {
                return false;
            }

            // Those parameters will be checked later on to make sure both expressions are equivalent.
            this.GetCompareState(state, 0).AddRange(node1.Parameters);
            this.GetCompareState(state, 1).AddRange(node2.Parameters);

            return base.CompareLambda(node1, node2, state);
        }

        protected override object CreateHashState(Expression node)
        {
            return new List<ParameterExpression>();
        }

        protected override int GetHashCode(LambdaExpression node, object state)
        {
            if (node != null)
            {
                this.GetHashCodeState(state).AddRange(node.Parameters);
            }

            return base.GetHashCode(node, state);
        }

        protected override int GetHashCode(ParameterExpression node, object state)
        {
            var index = this.GetHashCodeState(state).IndexOf(node);
            if (index == -1)
            {
                // Name is relevant...
                return base.GetHashCode(node, state);
            }
            else
            {
                // Index of the parameter is relevant...
                return this.CombineHash(this.GetHashCode(node.Type, state), node.IsByRef.GetHashCode(), index);
            }
        }

        private List<ParameterExpression> GetHashCodeState(object state)
        {
            return (List<ParameterExpression>)state;
        }

        private List<ParameterExpression> GetCompareState(object state, int index)
        {
            return ((List<ParameterExpression>[])state)[index];
        }
    }
}