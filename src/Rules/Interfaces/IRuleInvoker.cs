//-----------------------------------------------------------------------------
// <copyright file="IRuleInvoker.cs" company="http://rulesengine.codeplex.com">
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

    public interface IRuleInvoker
    {
        Type ParameterType { get; }

        void Invoke(object value, IValidationReport report, ValidationReportDepth depth);
    }
}