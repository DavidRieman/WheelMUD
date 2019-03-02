//-----------------------------------------------------------------------------
// <copyright file="NotNullRule.cs" company="http://rulesengine.codeplex.com">
//   Copyright (c) athoma13. See RulesEngine_License.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Rule Engine
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Rules
{
    public class NotNullRule<R> : IRule<R> where R : class
    {
        public string RuleKind
        {
            get { return "NotNullRule"; }
        }

        public ValidationResult Validate(R value)
        {
            return value != null ? ValidationResult.Success : ValidationResult.Fail();
        }
    }
}