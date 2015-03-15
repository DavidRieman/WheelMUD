//-----------------------------------------------------------------------------
// <copyright file="EnumerableCompositionInvoker.cs" company="http://rulesengine.codeplex.com">
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
    using System.Collections;
    using System.Linq.Expressions;

    public class EnumerableCompositionInvoker<T, R> : IRuleInvoker where R : IEnumerable
    {
        Func<T, R> _compiledExpression;
        CachedExpression _enumerableCompositionExpression;
        RulesEngine rulesRulesEngine;
        public EnumerableCompositionInvoker(RulesEngine rulesRulesEngine, Expression<Func<T, R>> enumerableCompositionExpression)
        {
            this.rulesRulesEngine = rulesRulesEngine;
            _compiledExpression = enumerableCompositionExpression.Compile();
            _enumerableCompositionExpression = rulesRulesEngine.ExpressionCache.Get(enumerableCompositionExpression);
        }
        public void Invoke(object value, IValidationReport report, ValidationReportDepth depth)
        {
            if (depth == ValidationReportDepth.FieldShortCircuit && report.HasError(_enumerableCompositionExpression, value))
            {
                return;
            }

            IEnumerable enumerableToValidate = _compiledExpression.Invoke((T)value);
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

        public Type ParameterType
        {
            get { return typeof(T); }
        }

    }
}
