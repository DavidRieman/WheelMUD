//-----------------------------------------------------------------------------
// <copyright file="ExpressionCache.cs" company="http://rulesengine.codeplex.com">
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

    public sealed class ExpressionCache
    {
        private readonly Dictionary<ExpressionKey, CachedExpression> ccache;
        private readonly IEqualityComparer<Expression> comparer;

        public ExpressionCache() : this(new ExpressionComparer())
        {
        }

        public ExpressionCache(IEqualityComparer<Expression> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            this.comparer = comparer;
            ccache = new Dictionary<ExpressionKey, CachedExpression>();
        }

        public CachedExpression Get(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            CachedExpression result;
            var key = new ExpressionKey(comparer, expression);
            if (ccache.TryGetValue(key, out result))
            {
                return result;
            }
            else
            {
                result = new CachedExpression(expression);
                ccache.Add(key, result);
                return result;
            }
        }

        private class ExpressionKey : IEquatable<ExpressionKey>
        {
            private readonly IEqualityComparer<Expression> comparer;
            private readonly Expression expression;
            private readonly int hashCode;

            public ExpressionKey(IEqualityComparer<Expression> comparer, Expression expression)
            {
                this.comparer = comparer;
                this.expression = expression;
                this.hashCode = comparer.GetHashCode(expression);
            }

            /// <summary>Gets the Expression.</summary>
            public Expression Expression
            {
                get { return expression; }
            }

            public bool Equals(ExpressionKey other)
            {
                return hashCode == other.hashCode && comparer.Equals(expression, other.expression);
            }

            public override int GetHashCode()
            {
                return hashCode;
            }
        }
    }
}