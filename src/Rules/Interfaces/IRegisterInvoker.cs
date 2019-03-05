//-----------------------------------------------------------------------------
// <copyright file="IRegisterInvoker.cs" company="http://rulesengine.codeplex.com">
//   Copyright (c) athoma13. See RulesEngine_License.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Rule Engine
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Rules
{
    public interface IRegisterInvoker
    {
        RulesEngine RulesRulesEngine { get; }

        void RegisterInvoker(IRuleInvoker ruleInvoker);
    }
}