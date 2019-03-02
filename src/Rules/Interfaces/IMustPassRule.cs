//-----------------------------------------------------------------------------
// <copyright file="IMustPassRule.cs" company="http://rulesengine.codeplex.com">
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
    using System.Linq.Expressions;

    public interface IMustPassRule<M, T, R>
    {
        Expression<Func<T, R>> Expression { get; }

        RulesEngine RulesRulesEngine { get; }

        M MustPassRule(IRule<R> rule);

        M MustPassRule(IRule<T> rule);

        M GetSelf();
    }
}