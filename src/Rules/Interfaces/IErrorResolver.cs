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
    public interface IErrorResolver
    {
        string GetErrorMessage(ValidationError validationError);
    }

    public class ValidationError
    {
        private IRule _rule;
        private CachedExpression _cachedExpression;
        private object[] _validationArguments;
        private object _value;

        /// <summary>
        /// Gets Rule
        /// </summary>
        public IRule Rule
        {
            get { return _rule; }
        }

        /// <summary>
        /// Gets Expression
        /// </summary>
        public CachedExpression CachedExpression
        {
            get { return _cachedExpression; }
        }

        /// <summary>
        /// Gets ValidationArguments
        /// </summary>
        public object[] ValidationArguments
        {
            get { return _validationArguments; }
        }

        /// <summary>
        /// Gets Value
        /// </summary>
        public object Value
        {
            get { return _value; }
        }

        public ValidationError(IRule rule, CachedExpression cachedExpression, object[] validationArguments, object value)
        {
            if (validationArguments == null) throw new System.ArgumentNullException("validationArguments");
            if (value == null) throw new System.ArgumentNullException("value");
            if (cachedExpression == null) throw new System.ArgumentNullException("cachedExpression");
            if (rule == null) throw new System.ArgumentNullException("rule");

            _rule = rule;
            _cachedExpression = cachedExpression;
            _validationArguments = validationArguments;
            _value = value;
        }
    }
}
