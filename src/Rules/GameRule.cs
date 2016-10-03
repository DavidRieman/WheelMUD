//-----------------------------------------------------------------------------
// <copyright file="GameRule.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 5/6/2009 6:50:22 PM
//   Purpose   : Representation of a game rule.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Rules
{
    using System;
    using WheelMUD.Interfaces;

    /// <summary>This is the code representation of a rule in a gaming system.</summary>
    public class GameRule : IRule
    {
        /// <summary>Initializes a new instance of the <see cref="GameRule"/> class.</summary>
        public GameRule()
        {
            //RuleFailureCondition = RuleConditions.None;
            //RuleSuccessCondition = RuleConditions.None;
        }
        
        ///// <summary>Gets or sets the name.</summary>
        ///// <value>The name of this game rule.</value>
        //public string Name { get; set; }

        ///// <summary>Gets or sets the source formula.</summary>
        ///// <value>The source formula.</value>
        //public string Formula { get; set; }

        ///// <summary>Gets or sets the action that this rule will perform.</summary>
        //public RuleAction ActionToPerform { get; set; }

        ///// <summary>Gets or sets the rule success condition.</summary>
        ///// <value>The rule success condition.</value>
        //public RuleConditions RuleSuccessCondition { get; set; }

        ///// <summary>Gets or sets the rule success critical formula.</summary>
        ///// <value>The rule success critical formula.</value>
        //public string RuleSuccessCritialFormula { get; set; }

        ///// <summary>Gets or sets the rule failure condition.</summary>
        ///// <value>The rule failure condition.</value>
        //public RuleConditions RuleFailureCondition { get; set; }

        ///// <summary>Gets or sets the rule failure critical formula.</summary>
        ///// <value>The rule failure critical formula.</value>
        //public string RuleFailureCritialFormula { get; set; }
        
        /// <summary>Gets the kind of the rule.</summary>
        /// <value>The kind of the rule.</value>
        public virtual string RuleKind
        {
            get { return "Game Rule"; }
        }

        /// <summary>Executes the specified player thing.</summary>
        /// <param name="playerThing">The player thing.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        public virtual void Execute(IThing playerThing, string value1, int value2)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>Basic game rule using generics.</summary>
    /// <typeparam name="T">First type.</typeparam>
    /// <typeparam name="R">Second type.</typeparam>
    public class GameRule<T, R> : IRule<T>
    {
        /// <summary>Gets the kind of the rule.</summary>
        /// <value>The kind of the rule.</value>
        public virtual string RuleKind
        {
            get { return "Game Rule"; }
        }

        /// <summary>Validates the specified value.</summary>
        /// <param name="value">The value.</param>
        /// <returns>A validation result enum.</returns>
        public virtual ValidationResult Validate(T value)
        {
            throw new NotImplementedException();
        }

        /// <summary>Executes this instance.</summary>
        public virtual void Execute()
        {
            throw new NotImplementedException();
        }

        /// <summary>Executes the specified player thing.</summary>
        /// <param name="playerThing">The player thing.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        public virtual void Execute(IThing playerThing, string value1, string value2)
        {
            throw new NotImplementedException();
        }
    }
}