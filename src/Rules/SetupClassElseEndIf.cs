//-----------------------------------------------------------------------------
// <copyright file="SetupClassElseEndIf.cs" company="http://rulesengine.codeplex.com">
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

    public class SetupClassElseEndIf<T, R, ENDIF> : IMustPassRule<SetupClassElseEndIf<T, R, ENDIF>, T, R>, ISetupClass
    {
        private Expression<Func<T, R>> expression;
        private ForClassElseEndIf<T, ENDIF> parent;
        private RulesEngine rulesRulesEngine;

        internal SetupClassElseEndIf(RulesEngine rulesRulesEngine, ForClassElseEndIf<T, ENDIF> parent, Expression<Func<T, R>> expression)
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

        public SetupClassElseEndIf<T, R, ENDIF> MustPassRule(IRule<R> rule)
        {
            this.rulesRulesEngine.RegisterRule(rule, expression, expression);
            return this;
        }

        public SetupClassElseEndIf<T, R, ENDIF> MustPassRule(IRule<T> rule)
        {
            this.rulesRulesEngine.RegisterRule(rule, Utilities.ReturnSelf<T>(), expression);
            return this;
        }
        
        public SetupClassElseEndIf<T, R1, ENDIF> Setup<R1>(Expression<Func<T, R1>> expression)
        {
            return new SetupClassElseEndIf<T, R1, ENDIF>(this.rulesRulesEngine, parent, expression);
        }

        public ForClassEndIf<T, ENDIF> Else()
        {
            return parent.Else();
        }

        public ForClassElseEndIf<T, ForClassElseEndIf<T, ENDIF>> If(Expression<Func<T, bool>> condition)
        {
            return parent.If(condition);
        }

        public ENDIF EndIf()
        {
            return parent.EndIf();
        }

        public SetupClassElseEndIf<T, R, ENDIF> GetSelf()
        {
            return this;
        }
    }
}