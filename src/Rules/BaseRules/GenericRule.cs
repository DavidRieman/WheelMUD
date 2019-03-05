//-----------------------------------------------------------------------------
// <copyright file="GenericRule.cs" company="http://rulesengine.codeplex.com">
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

    public class GenericRule<R> : IRule<R>
    {
        private Func<R, bool> rule;

        public GenericRule(Func<R, bool> rule)
        {
            this.rule = rule;
        }

        public string RuleKind
        {
            get { return "GenericRule"; }
        }

        public ValidationResult Validate(R value)
        {
            return rule(value) ? ValidationResult.Success : ValidationResult.Fail();
        }
    }
}