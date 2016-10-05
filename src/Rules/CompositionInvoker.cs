//-----------------------------------------------------------------------------
// <copyright file="CompositionInvoker.cs" company="http://rulesengine.codeplex.com">
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

    public class CompositionInvoker<T, R> : IRuleInvoker
    {
        private Func<T, R> compiledExpression;
        private CachedExpression cachedExpression;
        private RulesEngine rulesRulesEngine;

        public CompositionInvoker(RulesEngine rulesRulesEngine, Expression<Func<T, R>> compositionExpression)
        {
            this.rulesRulesEngine = rulesRulesEngine;
            this.compiledExpression = compositionExpression.Compile();
            this.cachedExpression = rulesRulesEngine.ExpressionCache.Get(compositionExpression);
        }

        public Type ParameterType
        {
            get { return typeof(T); }
        }

        public void Invoke(object value, IValidationReport report, ValidationReportDepth depth)
        {
            if (depth == ValidationReportDepth.FieldShortCircuit && report.HasError(cachedExpression, value))
            {
                return;
            }

            R objToValidate = compiledExpression.Invoke((T)value);
            if (objToValidate != null)
            {
                this.rulesRulesEngine.Validate(objToValidate, report, depth);
            }
        }
    }
}
