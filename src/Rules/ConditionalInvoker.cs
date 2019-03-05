//-----------------------------------------------------------------------------
// <copyright file="ConditionalInvoker.cs" company="http://rulesengine.codeplex.com">
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

    public class ConditionalInvoker<T> : IRuleInvoker, IRegisterInvoker
    {
        private Func<T, bool> condition;
        private RulesEngine parent;
        private RulesEngine innerTrue;
        private RulesEngine innerFalse;

        public ConditionalInvoker(Expression<Func<T, bool>> conditionalExpression, RulesEngine parent)
        {
            condition = conditionalExpression.Compile();
            this.parent = parent;
            this.innerTrue = new RulesEngine();
            this.innerFalse = new RulesEngine();
        }

        public RulesEngine RulesRulesEngine
        {
            get { return parent; }
        }

        public RulesEngine IfTrueRulesEngine
        {
            get { return innerTrue; }
        }

        public RulesEngine IfFalseRulesEngine
        {
            get { return innerFalse; }
        }

        public Type ParameterType
        {
            get { return typeof(T); }
        }

        public void Invoke(object value, IValidationReport report, ValidationReportDepth depth)
        {
            if (condition.Invoke((T)value))
            {
                innerTrue.Validate(value, report, depth);
            }
            else
            {
                innerFalse.Validate(value, report, depth);
            }
        }

        public void RegisterInvoker(IRuleInvoker ruleInvoker)
        {
            innerTrue.RegisterInvoker(ruleInvoker);
        }
    }
}