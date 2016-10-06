//-----------------------------------------------------------------------------
// <copyright file="IMustPassRule.cs" company="http://rulesengine.codeplex.com">
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