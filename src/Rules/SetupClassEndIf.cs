//-----------------------------------------------------------------------------
// <copyright file="SetupClassEndIf.cs" company="http://rulesengine.codeplex.com">
//   Copyright (c) athoma13. See RulesEngine_License.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Rule Engine
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Rules
{
    using System;
    using System.Linq.Expressions;

    public class SetupClassEndIf<T, R, ENDIF> : IMustPassRule<SetupClassEndIf<T, R, ENDIF>, T, R>
    {
        private Expression<Func<T, R>> expression;
        private ForClassEndIf<T, ENDIF> parent;
        private RulesEngine rulesRulesEngine;

        internal SetupClassEndIf(RulesEngine rulesRulesEngine, ForClassEndIf<T, ENDIF> parent, Expression<Func<T, R>> expression)
        {
            this.expression = expression;
            this.parent = parent;
            this.rulesRulesEngine = rulesRulesEngine;
        }

        public Expression<Func<T, R>> Expression
        {
            get { return this.expression; }
        }

        /// <summary>Gets RulesEngine.</summary>
        public RulesEngine RulesRulesEngine
        {
            get { return this.rulesRulesEngine; }
        }

        public SetupClassEndIf<T, R, ENDIF> MustPassRule(IRule<R> rule)
        {
            this.rulesRulesEngine.RegisterRule(rule, expression, expression);
            return this;
        }

        public SetupClassEndIf<T, R, ENDIF> MustPassRule(IRule<T> rule)
        {
            this.rulesRulesEngine.RegisterRule(rule, Utilities.ReturnSelf<T>(), expression);
            return this;
        }
        
        public SetupClassEndIf<T, R1, ENDIF> Setup<R1>(Expression<Func<T, R1>> expression)
        {
            return new SetupClassEndIf<T, R1, ENDIF>(this.rulesRulesEngine, parent, expression);
        }

        public ForClassElseEndIf<T, ForClassEndIf<T, ENDIF>> If(Expression<Func<T, bool>> condition)
        {
            return parent.If(condition);
        }
        
        public ENDIF EndIf()
        {
            return parent.EndIf();
        }

        public SetupClassEndIf<T, R, ENDIF> GetSelf()
        {
            return this;
        }
    }
}