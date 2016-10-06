//-----------------------------------------------------------------------------
// <copyright file="IErrorResolver.cs" company="http://rulesengine.codeplex.com">
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

    public interface IErrorResolver
    {
        string GetErrorMessage(ValidationError validationError);
    }

    public class ValidationError
    {
        private IRule rule;
        private CachedExpression cachedExpression;
        private object[] validationArguments;
        private object value;

        public ValidationError(IRule rule, CachedExpression cachedExpression, object[] validationArguments, object value)
        {
            if (validationArguments == null)
            {
                throw new ArgumentNullException("validationArguments");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (cachedExpression == null)
            {
                throw new ArgumentNullException("cachedExpression");
            }

            if (rule == null)
            {
                throw new ArgumentNullException("rule");
            }

            this.rule = rule;
            this.cachedExpression = cachedExpression;
            this.validationArguments = validationArguments;
            this.value = value;
        }

        /// <summary>Gets the Rule.</summary>
        public IRule Rule
        {
            get { return rule; }
        }

        /// <summary>Gets the cached expression.</summary>
        public CachedExpression CachedExpression
        {
            get { return cachedExpression; }
        }

        /// <summary>Gets ValidationArguments.</summary>
        public object[] ValidationArguments
        {
            get { return validationArguments; }
        }

        /// <summary>Gets the value.</summary>
        public object Value
        {
            get { return value; }
        }
    }
}