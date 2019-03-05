//-----------------------------------------------------------------------------
// <copyright file="ForClassElseEndIf.cs" company="http://rulesengine.codeplex.com">
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

    public class ForClassElseEndIf<T, ENDIF> : IMustPassRule<ForClassElseEndIf<T, ENDIF>, T, T>
    {
        private RulesEngine rulesRulesEngine;
        private ENDIF parent;
        private ConditionalInvoker<T> conditionalInvoker;

        internal ForClassElseEndIf(RulesEngine rulesRulesEngine, ConditionalInvoker<T> conditionalInvoker, ENDIF parent)
        {
            this.rulesRulesEngine = rulesRulesEngine;
            this.parent = parent;
            this.conditionalInvoker = conditionalInvoker;
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

        public SetupClassElseEndIf<T, R, ENDIF> Setup<R>(Expression<Func<T, R>> expression)
        {
            return new SetupClassElseEndIf<T, R, ENDIF>(this.rulesRulesEngine, this, expression);
        }

        public ForClassElseEndIf<T, ENDIF> MustPassRule(IRule<T> rule)
        {
            Expression<Func<T, T>> expression = t => t;
            this.rulesRulesEngine.RegisterRule<T, T>(rule, expression, expression);
            return this;
        }

        public ForClassElseEndIf<T, ForClassElseEndIf<T, ENDIF>> If(Expression<Func<T, bool>> conditionalExpression)
        {
            var invoker = this.rulesRulesEngine.RegisterCondition(conditionalExpression);
            return new ForClassElseEndIf<T, ForClassElseEndIf<T, ENDIF>>(invoker.IfTrueRulesEngine, invoker, this);
        }

        public ForClassEndIf<T, ENDIF> Else()
        {
            return new ForClassEndIf<T, ENDIF>(conditionalInvoker.IfFalseRulesEngine, conditionalInvoker, parent);
        }

        public ENDIF EndIf()
        {
            return parent;
        }

        public ForClassElseEndIf<T, ENDIF> GetSelf()
        {
            return this;
        }
    }
}