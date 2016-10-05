//-----------------------------------------------------------------------------
// <copyright file="OfTypeRule.cs" company="http://rulesengine.codeplex.com">
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

    public class OfTypeRule<R> : IRule<R>
    {
        private Type type;

        public OfTypeRule(Type type)
        {
            this.type = type;
        }

        public string RuleKind
        {
            get { return "OfTypeRule"; }
        }

        public ValidationResult Validate(R value)
        {
            if (value != null && type.IsAssignableFrom(value.GetType()))
            {
                return ValidationResult.Success;
            }

            return ValidationResult.Fail(type);
        }
    }
}