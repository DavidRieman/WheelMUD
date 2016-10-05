//-----------------------------------------------------------------------------
// <copyright file="CharacterSheet.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A script to view a character sheet.
//   @@@ TODO: Implement beyond 'Attributes.cs' functionality.
//   Created: November 2006 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Effects;
    using WheelMUD.Interfaces;

    /// <summary>A command to list the player's character sheet.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("character sheet", CommandCategory.Inform)]
    [ActionAlias("charactersheet", CommandCategory.Inform)]
    [ActionAlias("char sheet", CommandCategory.Inform)]
    [ActionAlias("charsheet", CommandCategory.Inform)]
    [ActionAlias("character", CommandCategory.Inform)]
    [ActionAlias("score", CommandCategory.Inform)]
    [ActionAlias("sco", CommandCategory.Inform)]
    [ActionDescription("See your stats.")]
    [ActionSecurity(SecurityRole.player)]
    public class Stats : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            Thing t = sender.Thing;

            var statEffects = t.Behaviors.OfType<StatEffect>();
            var sb = new StringBuilder();

            var health = t.Stats["HP"];
            var healthMod = statEffects.Where(e => e.Stat.Abbreviation == "HP").Sum(e => e.ValueMod);
            var mana = t.Stats["MANA"];
            var damage = t.Stats["DAMAGE"];
            var init = t.Stats["INIT"];
            var attack = t.Stats["ATK"];
            var defense = t.Stats["DEF"];
            var armorPenalty = t.Stats["ARMORPENALTY"];
            var wieldMax = t.Stats["WEAPONWIELDMAX"];
            var hunt = t.Stats["HUNT"];
            var familiar = t.Stats["FAMILIAR"];
            var fate = t.Stats["FATE"];

            var healthLine = string.Format("{0,-13} {1,5:####0}/{2,-5:####0} ({3})", health.Name, health.Value, health.MaxValue, healthMod).PadRight(31);
            var manaLine = string.Format("{0,-12} {1,5:####0}/{2,-5:####0}", mana.Name, mana.Value, mana.MaxValue).PadRight(31);
            var damageLine = string.Format("{0,-13} {1,5:##0}/{2,-5:##0}", damage.Name, damage.Value, damage.MaxValue).PadRight(31);
            var initLine = string.Format("{0,-16} {1,-6}", init.Name, init.Value).PadRight(31);
            var attackLine = string.Format("{0,-15} {1,3:##0}/{2,-3:##0}", attack.Name, attack.Value, attack.MaxValue).PadRight(31);
            var defenseLine = string.Format("{0,-14} {1,3:##0}/{2,-3:##0}", defense.Name, defense.Value, defense.MaxValue).PadRight(31);
            var armorPenaltyLine = string.Format("{0,-16} {1,2}", armorPenalty.Name, armorPenalty.Value).PadRight(31);
            var wieldMaxLine = string.Format("{0,-16} {1,1}/{2,-1}", wieldMax.Name, wieldMax.Value, wieldMax.MaxValue).PadRight(31);
            var huntLine = string.Format("{0,-16} {1,2}({2,2})", hunt.Name, hunt.Value, hunt.MaxValue).PadRight(31);
            var familiarLine = string.Format("{0,-16} {1,1:0}{2,-1:(0)}", familiar.Name, familiar.Value, familiar.MaxValue).PadRight(31);
            var fateLine = string.Format("{0,-16} {1,2}({2,2})", fate.Name, fate.Value, fate.MaxValue).PadRight(31);

            sb.AppendFormat(t.Name + ", " + t.Title + "\r\n");
            sb.AppendFormat("   +-------------------------------------------------------------------+\r\n");
            sb.AppendFormat("   | {0, -19} Level {1, -6}  Reputation {2, -6}  Kudos {3, -6} |\r\n", t.Name, 1, 0, 0);
            sb.AppendFormat("   +----------------+----------------+----------------+----------------+\r\n");

            sb.AppendFormat("   | {0} | {1} |\r\n", healthLine, manaLine);
            sb.AppendFormat("   | {0} | {1} |\r\n", damageLine, initLine);
            sb.AppendFormat("   | {0} | {1} |\r\n", attackLine, defenseLine);
            sb.AppendFormat("   | {0} | {1} |\r\n", armorPenaltyLine, wieldMaxLine);
            sb.AppendFormat("   | {0} | {1} |\r\n", huntLine, familiarLine);
            sb.AppendFormat("   | {0} | {1} |\r\n", fateLine, string.Empty.PadRight(31));

            sb.AppendFormat("   +-------------------------------------------------------------------+\r\n");

            sender.Write(sb.ToString().Trim());
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            string commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            return null;
        }
    }
}