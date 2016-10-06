//-----------------------------------------------------------------------------
// <copyright file="ValidationReport.cs" company="http://rulesengine.codeplex.com">
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
    using System.Collections.Generic;
    using System.Linq;

    public class ValidationReport : IValidationReport
    {
        private bool hasErrors = false;
        private Dictionary<CachedExpressionObjectPair, List<ValidationError>> hasExpressionError = new Dictionary<CachedExpressionObjectPair, List<ValidationError>>();

        public bool HasErrors
        {
            get { return hasErrors; }
        }

        public virtual void AddError(ValidationError validationError)
        {
            this.hasErrors = true;
            CachedExpressionObjectPair key = new CachedExpressionObjectPair(validationError.CachedExpression, validationError.Value);
            List<ValidationError> validationErrors;

            if (hasExpressionError.TryGetValue(key, out validationErrors))
            {
                validationErrors.Add(validationError);
            }
            else
            {
                hasExpressionError.Add(key, new ValidationError[] { validationError }.ToList());
            }
        }

        public bool HasError(CachedExpression cachedExpression, object value)
        {
            return hasExpressionError.ContainsKey(new CachedExpressionObjectPair(cachedExpression, value));
        }

        public bool HasError(CachedExpression cachedExpression, object value, out ValidationError[] validationErrors)
        {
            List<ValidationError> result;
            if (hasExpressionError.TryGetValue(new CachedExpressionObjectPair(cachedExpression, value), out result))
            {
                validationErrors = result.ToArray();
                return true;
            }

            validationErrors = null;
            return false;
        }
    }
}