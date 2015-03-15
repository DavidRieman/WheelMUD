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
        Expression<Func<T, R>> _expression;
        ForClassElseEndIf<T, ENDIF> _parent;
        RulesEngine rulesRulesEngine;

        internal SetupClassElseEndIf(RulesEngine rulesRulesEngine, ForClassElseEndIf<T, ENDIF> parent, Expression<Func<T, R>> expression)
        {
            _expression = expression;
            _parent = parent;
            this.rulesRulesEngine = rulesRulesEngine;
        }

        public SetupClassElseEndIf<T, R, ENDIF> MustPassRule(IRule<R> rule)
        {
            this.rulesRulesEngine.RegisterRule(rule, _expression, _expression);
            return this;
        }

        public SetupClassElseEndIf<T, R, ENDIF> MustPassRule(IRule<T> rule)
        {
            this.rulesRulesEngine.RegisterRule(rule, Utilities.ReturnSelf<T>(), _expression);
            return this;
        }


        public SetupClassElseEndIf<T, R1, ENDIF> Setup<R1>(Expression<Func<T, R1>> expression)
        {
            return new SetupClassElseEndIf<T, R1, ENDIF>(this.rulesRulesEngine, _parent, expression);
        }

        public ForClassEndIf<T, ENDIF> Else()
        {
            return _parent.Else();
        }

        public ForClassElseEndIf<T, ForClassElseEndIf<T, ENDIF>> If(Expression<Func<T, bool>> condition)
        {
            return _parent.If(condition);
        }

        public ENDIF EndIf()
        {
            return _parent.EndIf();
        }

        public Expression<Func<T, R>> Expression
        {
            get
            {
                return _expression;
            }
        }

        /// <summary>
        /// Gets RulesEngine
        /// </summary>
        public RulesEngine RulesRulesEngine
        {
            get { return this.rulesRulesEngine; }
        }


        public SetupClassElseEndIf<T, R, ENDIF> GetSelf()
        {
            return this;
        }
    }
}
