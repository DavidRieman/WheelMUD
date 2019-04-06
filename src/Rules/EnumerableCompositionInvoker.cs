//-----------------------------------------------------------------------------
// <copyright file="EnumerableCompositionInvoker.cs" company="http://rulesengine.codeplex.com">
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
    using System.Collections;
    using System.Linq.Expressions;

    public class EnumerableCompositionInvoker<T, R> : IRuleInvoker where R : IEnumerable
    {
        private Func<T, R> compiledExpression;
        private CachedExpression enumerableCompositionExpression;
        private RulesEngine rulesRulesEngine;

        public EnumerableCompositionInvoker(RulesEngine rulesRulesEngine, Expression<Func<T, R>> enumerableCompositionExpression)
        {
            this.rulesRulesEngine = rulesRulesEngine;
            compiledExpression = enumerableCompositionExpression.Compile();
            this.enumerableCompositionExpression = rulesRulesEngine.ExpressionCache.Get(enumerableCompositionExpression);
        }

        public Type ParameterType
        {
            get { return typeof(T); }
        }

        public void Invoke(object value, IValidationReport report, ValidationReportDepth depth)
        {
            if (depth == ValidationReportDepth.FieldShortCircuit && report.HasError(enumerableCompositionExpression, value))
            {
                return;
            }

            IEnumerable enumerableToValidate = compiledExpression.Invoke((T)value);
            if (enumerableToValidate != null)
            {
                foreach (object objToValidate in enumerableToValidate)
                {
                    this.rulesRulesEngine.Validate(objToValidate, report, depth);
                    if (report.HasErrors && (depth == ValidationReportDepth.ShortCircuit))
                    {
                        return;
                    }
                }
            }
        }
    }
}