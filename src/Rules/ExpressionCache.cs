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
        private class ExpressionKey : IEquatable<ExpressionKey>
        {
            private readonly IEqualityComparer<Expression> _comparer;
            private readonly Expression _expression;
            private readonly int _hashCode;

            /// <summary>
            /// Gets Expression
            /// </summary>
            public Expression Expression
            {
                get { return _expression; }
            }

            public ExpressionKey(IEqualityComparer<Expression> comparer, Expression expression)
            {
                _comparer = comparer;
                _expression = expression;
                _hashCode = comparer.GetHashCode(expression);

            }

            public bool Equals(ExpressionKey other)
            {
                return (_hashCode == other._hashCode && _comparer.Equals(_expression, other._expression));
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }
        }
        private readonly Dictionary<ExpressionKey, CachedExpression> _cache;
        private readonly IEqualityComparer<Expression> _comparer;

        public ExpressionCache() :
            this(new ExpressionComparer())
        {
        }
        public ExpressionCache(IEqualityComparer<Expression> comparer)
        {
            if (comparer == null) throw new System.ArgumentNullException("comparer");
            _comparer = comparer;
            _cache = new Dictionary<ExpressionKey, CachedExpression>();
        }

        public CachedExpression Get(Expression expression)
        {
            if (expression == null) throw new System.ArgumentNullException("expression");
            CachedExpression result;
            var key = new ExpressionKey(_comparer, expression);
            if (_cache.TryGetValue(key, out result))
            {
                return result;
            }
            else
            {
                result = new CachedExpression(expression);
                _cache.Add(key, result);
                return result;
            }
        }
    }
}
