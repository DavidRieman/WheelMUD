//-----------------------------------------------------------------------------
// <copyright file="RuleResult.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 2/19/2011
//   Purpose   : Contains information about the result of a rule.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Rules
{
    using System.Collections.Generic;

    /// <summary>Contains information about the result of a rule.</summary>
    public class RuleResult
    {
        /// <summary>Initializes a new instance of the <see cref="RuleResult"/> class. Creates a successful result.</summary>
        /// <param name="ruleName">Unique name of the rule creating this result.</param>
        /// <param name="property">Property to which this result should be attached.</param>
        public RuleResult(string ruleName, IPropertyInfo property)
        {
            this.RuleName = ruleName;
            this.PrimaryProperty = property;
            this.Success = true;
        }

        /// <summary>Initializes a new instance of the <see cref="RuleResult"/> class. Creates a failure result.</summary>
        /// <param name="ruleName">Unique name of the rule creating this result.</param>
        /// <param name="property">Property to which this result should be attached.</param>
        /// <param name="description">Human-readable description of why the rule failed.</param>
        public RuleResult(string ruleName, IPropertyInfo property, string description)
        {
            this.RuleName = ruleName;
            this.PrimaryProperty = property;
            this.Description = description;
            this.Success = string.IsNullOrEmpty(description);

            if (!this.Success)
            {
                this.Severity = RuleSeverity.Error;
            }
        }

        /// <summary>Gets the unique name of the rule that created this result.</summary>
        public string RuleName { get; private set; }

        /// <summary>Gets a value indicating whether the  rule was successful.</summary>
        public bool Success { get; private set; }

        /// <summary>Gets a human-readable description of why the rule failed.</summary>
        public string Description { get; private set; }

        /// <summary>Gets or sets the severity of a failed rule.</summary>
        public RuleSeverity Severity { get; set; }

        /// <summary>Gets or sets a value indicating whether rule processing should immediately stop (applies to sync rules only).</summary>
        public bool StopProcessing { get; set; }

        /// <summary>Gets the primary property for this result.</summary>
        public IPropertyInfo PrimaryProperty { get; private set; }

        /// <summary>
        /// Gets or sets a list of properties that were affected by the rule, so appropriate PropertyChanged 
        /// events can be raised for UI notification.
        /// </summary>
        public List<IPropertyInfo> Properties { get; set; }

        /// <summary>
        /// Gets or sets a dictionary of new property values used to update the business object's properties
        /// after the rule is complete.
        /// </summary>
        public Dictionary<IPropertyInfo, object> OutputPropertyValues { get; set; }
    }
}