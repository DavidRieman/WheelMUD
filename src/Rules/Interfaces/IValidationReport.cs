//-----------------------------------------------------------------------------
// <copyright file="IValidationReport.cs" company="http://rulesengine.codeplex.com">
//   Copyright (c) athoma13. See RulesEngine_License.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Rule Engine
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Rules
{
    public interface IValidationReport
    {
        bool HasErrors { get; }

        bool HasError(CachedExpression expression, object value, out ValidationError[] validationErrors);

        bool HasError(CachedExpression expression, object value);

        void AddError(ValidationError validationError);
    }
}