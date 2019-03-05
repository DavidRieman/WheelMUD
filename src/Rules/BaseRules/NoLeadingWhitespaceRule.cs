//-----------------------------------------------------------------------------
// <copyright file="NoLeadingWhitespaceRule.cs" company="http://rulesengine.codeplex.com">
//   Copyright (c) athoma13. See RulesEngine_License.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Rule Engine
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Rules
{
    public class NoLeadingWhitespaceRule : IRule<string>
    {
        public string RuleKind
        {
            get { return "NoLeadingWhitespaceRule"; }
        }

        public ValidationResult Validate(string value)
        {
            if (value == null || value.Length == 0)
            {
                return ValidationResult.Success;
            }
            else
            {
                if (!char.IsWhiteSpace(value[0]))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return ValidationResult.Fail();
                }
            }
        }
    }
}