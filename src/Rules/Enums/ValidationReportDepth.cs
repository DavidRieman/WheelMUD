//-----------------------------------------------------------------------------
// <copyright file="ValidationReportDepth.cs" company="http://rulesengine.codeplex.com">
//   Copyright (c) athoma13. See RulesEngine_License.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Rule Engine
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Rules
{
    public enum ValidationReportDepth
    {
        /// <summary>Only report the first encountered error for an Expression.</summary>
        FieldShortCircuit,

        /// <summary>Stops reporting at the first error encountered.</summary>
        ShortCircuit,

        /// <summary>Test all Validation Rules.</summary>
        All
    }
}