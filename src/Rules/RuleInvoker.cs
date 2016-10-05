//-----------------------------------------------------------------------------
// <copyright file="RuleInvoker.cs" company="http://rulesengine.codeplex.com">
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

    public class RuleInvoker<T, R> : IRuleInvoker
    {
        private IRule<R> rule;
        private CachedExpression expressionToBlame;
        private Func<T, R> compiledExpression;

        public RuleInvoker(IRule<R> rule, Expression<Func<T, R>> expressionToInvoke, CachedExpression expressionToBlame)
        {
            this.rule = rule;
            this.compiledExpression = expressionToInvoke.Compile();
            this.expressionToBlame = expressionToBlame;
        }

        public Type ParameterType
        {
            get { return typeof(T); }
        }

        public void Invoke(object value, IValidationReport report, ValidationReportDepth depth)
        {
            // If validating an Expression that has already failed a rule, then skip.
            if (depth == ValidationReportDepth.FieldShortCircuit && report.HasError(expressionToBlame, value))
            {
                return;
            }

            var result = rule.Validate(compiledExpression.Invoke((T)value));
            if (!result.IsValid)
            {
                report.AddError(new ValidationError(rule, expressionToBlame, result.Arguments, value));
            }
        }
    }
}