//-----------------------------------------------------------------------------
// <copyright file="ForClassEndIf.cs" company="http://rulesengine.codeplex.com">
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

    public class ForClassEndIf<T, ENDIF> : IMustPassRule<ForClassEndIf<T, ENDIF>, T, T>
    {
        private RulesEngine rulesRulesEngine;
        private ENDIF parent;

        internal ForClassEndIf(RulesEngine rulesRulesEngine, ConditionalInvoker<T> conditionalInvoker, ENDIF parent)
        {
            // TODO: conditionalInvoker is not used. These rules don't actually seem to be used anyway, but this seems 
            //       like a strong indication that at least this class is broken.
            this.rulesRulesEngine = rulesRulesEngine;
            this.parent = parent;
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

        public SetupClassEndIf<T, R, ENDIF> Setup<R>(Expression<Func<T, R>> expression)
        {
            return new SetupClassEndIf<T, R, ENDIF>(this.rulesRulesEngine, this, expression);
        }

        public ForClassEndIf<T, ENDIF> MustPassRule(IRule<T> rule)
        {
            Expression<Func<T, T>> expression = t => t;
            this.rulesRulesEngine.RegisterRule<T, T>(rule, expression, expression);
            return this;
        }

        public ForClassElseEndIf<T, ForClassEndIf<T, ENDIF>> If(Expression<Func<T, bool>> conditionalExpression)
        {
            var invoker = this.rulesRulesEngine.RegisterCondition(conditionalExpression);
            return new ForClassElseEndIf<T, ForClassEndIf<T, ENDIF>>(invoker.IfTrueRulesEngine, invoker, this);
        }

        public ENDIF EndIf()
        {
            return parent;
        }

        public ForClassEndIf<T, ENDIF> GetSelf()
        {
            return this;
        }
    }
}