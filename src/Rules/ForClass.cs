//-----------------------------------------------------------------------------
// <copyright file="ForClass.cs" company="http://rulesengine.codeplex.com">
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

    public class ForClass<T> : IMustPassRule<ForClass<T>, T, T>
    {
        private RulesEngine rulesRulesEngine;

        internal ForClass(RulesEngine rulesRulesEngine)
        {
            this.rulesRulesEngine = rulesRulesEngine;
        }

        public Expression<Func<T, T>> Expression
        {
            get { return Utilities.ReturnSelf<T>(); }
        }

        /// <summary>Gets RulesEngine.</summary>
        public RulesEngine RulesRulesEngine
        {
            get { return this.rulesRulesEngine; }
        }

        public SetupClass<T, R> Setup<R>(Expression<Func<T, R>> expression)
        {
            return new SetupClass<T, R>(this.rulesRulesEngine, this, expression);
        }
        
        public ForClass<T> MustPassRule(IRule<T> rule)
        {
            Expression<Func<T, T>> expression = t => t;
            this.rulesRulesEngine.RegisterRule<T, T>(rule, expression, expression);
            return this;
        }

        public ForClassElseEndIf<T, ForClass<T>> If(Expression<Func<T, bool>> conditionalExpression)
        {
            var invoker = this.rulesRulesEngine.RegisterCondition(conditionalExpression);
            return new ForClassElseEndIf<T, ForClass<T>>(invoker.IfTrueRulesEngine, invoker, this);
        }

        public ForClass<T> GetSelf()
        {
            return this;
        }
    }
}