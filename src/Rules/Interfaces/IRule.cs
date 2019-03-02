//-----------------------------------------------------------------------------
// <copyright file="IRule.cs" company="http://rulesengine.codeplex.com">
//   Copyright (c) athoma13. See RulesEngine_License.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Rule Engine
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Rules
{
    public interface IRule
    {
        string RuleKind { get; }
    }

    public interface IRule<R> : IRule
    {
        ValidationResult Validate(R value);
    }
}
