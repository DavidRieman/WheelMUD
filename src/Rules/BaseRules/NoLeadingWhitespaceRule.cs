//-----------------------------------------------------------------------------
// <copyright file="NoLeadingWhitespaceRule.cs" company="http://rulesengine.codeplex.com">
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