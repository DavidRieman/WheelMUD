//-----------------------------------------------------------------------------
// <copyright file="SetupClass.cs" company="http://rulesengine.codeplex.com">
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

    public class SetupClass<T, R> : IMustPassRule<SetupClass<T, R>, T, R>, ISetupClass
    {
        private Expression<Func<T, R>> expression;
        private ForClass<T> parent;
        private RulesEngine rulesRulesEngine;

        internal SetupClass(RulesEngine rulesRulesEngine, ForClass<T> parent, Expression<Func<T, R>> expression)
        {
            this.expression = expression;
            this.parent = parent;
            this.rulesRulesEngine = rulesRulesEngine;
        }

        public Expression<Func<T, R>> Expression
        {
            get
            {
                return expression;
            }
        }

        /// <summary>Gets RulesEngine.</summary>
        public RulesEngine RulesRulesEngine
        {
            get { return this.rulesRulesEngine; }
        }

        public SetupClass<T, R> MustPassRule(IRule<R> rule)
        {
            this.rulesRulesEngine.RegisterRule(rule, expression, expression);
            return this;
        }

        public SetupClass<T, R> MustPassRule(IRule<T> rule)
        {
            this.rulesRulesEngine.RegisterRule(rule, Utilities.ReturnSelf<T>(), expression);
            return this;
        }

        public SetupClass<T, R1> Setup<R1>(Expression<Func<T, R1>> expression)
        {
            return new SetupClass<T, R1>(this.rulesRulesEngine, parent, expression);
        }

        public ForClassElseEndIf<T, ForClass<T>> If(Expression<Func<T, bool>> condition)
        {
            return parent.If(condition);
        }
        
        public ForClass<T> EndSetup()
        {
            return parent;
        }

        public SetupClass<T, R> GetSelf()
        {
            return this;
        }
    }
}