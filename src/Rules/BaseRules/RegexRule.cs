//-----------------------------------------------------------------------------
// <copyright file="RegexRule.cs" company="http://rulesengine.codeplex.com">
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
    using System.Text.RegularExpressions;

    public class RegexRule : IRule<string>
    {
        private Regex regex;

        public RegexRule(Regex regex)
        {
            if (regex == null)
            {
                throw new ArgumentNullException("regex");
            }

            this.regex = regex;
        }

        public string RuleKind
        {
            get { return "RegexRule"; }
        }

        public ValidationResult Validate(string value)
        {
            // NOTE: Yes, null string will pass RegexRules. Use the NotNullRule in combination to invalidate nulls.
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (this.regex.IsMatch(value))
            {
                return ValidationResult.Success;
            }

            return ValidationResult.Fail(this.regex);
        }
    }
}