//-----------------------------------------------------------------------------
// <copyright file="PropertyValidationReport.cs" company="http://rulesengine.codeplex.com">
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

    public class PropertyValidationReport : ValidationReport
    {
        private readonly ExpressionCache expressionCache;

        public PropertyValidationReport()
        {
            this.expressionCache = RulesEngine.DefaultExpressionCache;
        }

        public PropertyValidationReport(ExpressionCache expressionCache)
        {
            if (expressionCache == null)
            {
                throw new ArgumentNullException("expressionCache");
            }

            this.expressionCache = expressionCache;
        }

        public string GetErrorMessage(IErrorResolver resolver, string propertyName, object value)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException("resolver");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            var exp = this.CreatePropertyExpression(propertyName, value);

            ValidationError[] validationErrors;
            if (this.HasError(exp, value, out validationErrors))
            {
                return resolver.GetErrorMessage(validationErrors[0]);
            }

            return null;
        }

        private CachedExpression CreatePropertyExpression(string propertyName, object value)
        {
            ParameterExpression paramExp = Expression.Parameter(value.GetType());
            Expression body = Expression.Property(paramExp, propertyName);
            var lambda = Expression.Lambda(body, paramExp);
            return this.expressionCache.Get(lambda);
        }
    }
}