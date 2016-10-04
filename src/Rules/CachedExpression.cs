//-----------------------------------------------------------------------------
// <copyright file="CachedExpression.cs" company="http://rulesengine.codeplex.com">
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
    using System.Linq.Expressions;

    /// <summary>Caches a rule expression.</summary>
    public sealed class CachedExpression
    {
        private readonly Expression cachedExpression;

        internal CachedExpression(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            this.cachedExpression = expression;
        }

        /// <summary>Gets the cached expression.</summary>
        public Expression Expression
        {
            get { return this.cachedExpression; }
        }

        /// <summary>Returns a <see cref="String"/> that represents this instance.</summary>
        /// <returns>A <see cref="String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return this.cachedExpression.ToString();
        }
    }
}