//-----------------------------------------------------------------------------
// <copyright file="NotNullOrEmpty.cs" company="http://rulesengine.codeplex.com">
//   Copyright (c) athoma13. See RulesEngine_License.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Rule Engine
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Rules
{
    public class NotNullOrEmpty : IRule<string>
    {
        public string RuleKind
        {
            get { return "NotNullOrEmpty"; }
        }

        public ValidationResult Validate(string value)
        {
            return string.IsNullOrEmpty(value) ? ValidationResult.Fail() : ValidationResult.Success;
        }
    }
}