//-----------------------------------------------------------------------------
// <copyright file="RollDieRule.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 2/24/2012 9:38:57 PM
//   Purpose   : Rule that requires rolling a set of dice.
// </summary>
//-----------------------------------------------------------------------------

/* Disabling: Generates unused variable warnings-as-errors which show that this rule is not in use,
   as there is not actual execution behavior here.
namespace WarriorRogueMage.Rules
{
    using WheelMUD.Core;
    using WheelMUD.Interfaces;
    using WheelMUD.Rules;

    /// <summary>Rule that requires rolling a set of dice.</summary>
    public class RollDieRule : GameRule
    {
        private static Thing parentThing;
        private string rollDiceFormula;

        /// <summary>Initializes a new instance of the <see cref="RollDieRule"/> class.</summary>
        /// <param name="diceFormula">The dice formula.</param>
        public RollDieRule(string diceFormula)
        {
            this.rollDiceFormula = diceFormula;
        }

        /// <summary>Gets the kind of the rule.</summary>
        /// <value>The kind of the rule.</value>
        public override string RuleKind
        {
            get
            {
                return "RollDieRule";
            }
        }

        /// <summary>Executes the rule.</summary>
        /// <param name="playerThing">The player thing.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        public override void Execute(IThing playerThing, string value1, int value2)
        {
            parentThing = (Thing)playerThing;
        }
    }
}*/