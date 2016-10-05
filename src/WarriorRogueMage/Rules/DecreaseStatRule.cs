//-----------------------------------------------------------------------------
// <copyright file="DecreaseStatRule.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 2/12/2012
//   Purpose   : Rule to decrease the value of a stat.
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage.Rules
{
    using System;
    using WheelMUD.Core;
    using WheelMUD.Interfaces;
    using WheelMUD.Rules;

    /// <summary>Rule to decrease the value of a stat.</summary>
    public class DecreaseStatRule : GameRule
    {
        private static Thing parentThing;
        private static GameStat privateStat;
        private static int value;

        /// <summary>Gets the kind of the rule.</summary>
        /// <value>The kind of the rule.</value>
        public override string RuleKind
        {
            get { return "DecreaseStatRule"; }
        }

        /// <summary>Executes the rule on the specified player <see cref="Thing"/>.</summary>
        /// <param name="playerThing">The player thing.</param>
        /// <param name="statName">Name of the attribute.</param>
        /// <param name="valueToAdd">The value to add.</param>
        public override void Execute(IThing playerThing, string statName, int valueToAdd)
        {
            value = valueToAdd;
            parentThing = (Thing)playerThing;

            FindStat(statName);
            DecreaseStatValue();
        }

        private static void FindStat(string statNameToFind)
        {
            var statToFind = parentThing.FindGameStat(statNameToFind);
            var stat = (WRMStat)Convert.ChangeType(statToFind, typeof(WRMStat));
            privateStat = (GameStat)Convert.ChangeType(parentThing.FindGameStat(stat.Name), typeof(GameStat));
        }

        private static void DecreaseStatValue()
        {
            var stat = (WRMStat)Convert.ChangeType(privateStat, typeof(WRMStat));
            stat.Decrease(value, parentThing);
        }
    }
}