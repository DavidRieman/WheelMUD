//-----------------------------------------------------------------------------
// <copyright file="GenericRule.cs" company="http://rulesengine.codeplex.com">
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

    public class GenericRule<R> : IRule<R>
    {
        Func<R, bool> _rule;

        public ValidationResult Validate(R value)
        {
            if (_rule(value))
                return ValidationResult.Success;

            return ValidationResult.Fail();
        }

        public GenericRule(Func<R, bool> rule)
        {
            _rule = rule;
        }

        public string RuleKind
        {
            get { return "GenericRule"; }
        }
    }
}
