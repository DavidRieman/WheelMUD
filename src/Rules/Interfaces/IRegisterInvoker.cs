//-----------------------------------------------------------------------------
// <copyright file="IRegisterInvoker.cs" company="http://rulesengine.codeplex.com">
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
    public interface IRegisterInvoker
    {
        RulesEngine RulesRulesEngine { get; }

        void RegisterInvoker(IRuleInvoker ruleInvoker);
    }
}