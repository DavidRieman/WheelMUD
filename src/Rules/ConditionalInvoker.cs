//-----------------------------------------------------------------------------
// <copyright file="ConditionalInvoker.cs" company="http://rulesengine.codeplex.com">
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

    public class ConditionalInvoker<T> : IRuleInvoker, IRegisterInvoker
    {
        Func<T, bool> _condition;
        RulesEngine _parent;
        RulesEngine _innerTrue;
        RulesEngine _innerFalse;

        public ConditionalInvoker(Expression<Func<T, bool>> conditionalExpression, RulesEngine parent)
        {
            _condition = conditionalExpression.Compile();
            _parent = parent;
            _innerTrue = new RulesEngine();
            _innerFalse = new RulesEngine();
        }

        public void Invoke(object value, IValidationReport report, ValidationReportDepth depth)
        {
            if (_condition.Invoke((T)value))
            {
                _innerTrue.Validate(value, report, depth);
            }
            else
            {
                _innerFalse.Validate(value, report, depth);
            }
        }

        public void RegisterInvoker(IRuleInvoker ruleInvoker)
        {
            _innerTrue.RegisterInvoker(ruleInvoker);
        }

        public RulesEngine RulesRulesEngine
        {
            get { return _parent; }
        }

        public RulesEngine IfTrueRulesEngine
        {
            get { return _innerTrue; }
        }
        public RulesEngine IfFalseRulesEngine
        {
            get { return _innerFalse; }
        }

        public Type ParameterType
        {
            get { return typeof(T); }
        }
    }
}
