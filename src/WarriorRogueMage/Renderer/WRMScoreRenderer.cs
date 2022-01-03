//-----------------------------------------------------------------------------
// <copyright file="WRMScoreRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Linq;
using WheelMUD.Effects;
using WheelMUD.Server;

namespace WheelMUD.Core
{
    /// <summary>The WRM game system score renderer.</summary>
    /// <remarks>Overrides the default WheelMUD score renderer, to present game-system-specific details.</remarks>
    [RendererExports.Score(100)]
    public class DefaultScoreRenderer : RendererDefinitions.Score
    {
        public override OutputBuilder Render(TerminalOptions terminalOptions, Thing player)
        {
            var stats = player.Stats;
            var statEffects = player.Behaviors.OfType<StatEffect>();
            var output = new OutputBuilder();

            var health = stats["HP"];
            var healthMod = statEffects.Where(e => e.Stat.Abbreviation == "HP").Sum(e => e.ValueMod);
            var mana = stats["MANA"];
            var damage = stats["DAMAGE"];
            var init = stats["INIT"];
            var attack = stats["ATK"];
            var defense = stats["DEF"];
            var armorPenalty = stats["ARMORPENALTY"];
            var wieldMax = stats["WEAPONWIELDMAX"];
            var hunt = stats["HUNT"];
            var familiar = stats["FAMILIAR"];
            var fate = stats["FATE"];

            var healthLine = $"{health.Name,-13} {health.Value,5:####0}/{health.MaxValue,-5:####0} ({healthMod})".PadRight(31);
            var manaLine = $"{mana.Name,-12} {mana.Value,5:####0}/{mana.MaxValue,-5:####0}".PadRight(31);
            var damageLine = $"{damage.Name,-13} {damage.Value,5:##0}/{damage.MaxValue,-5:##0}".PadRight(31);
            var initLine = $"{init.Name,-16} {init.Value,-6}".PadRight(31);
            var attackLine = $"{attack.Name,-15} {attack.Value,3:##0}/{attack.MaxValue,-3:##0}".PadRight(31);
            var defenseLine = $"{defense.Name,-14} {defense.Value,3:##0}/{defense.MaxValue,-3:##0}".PadRight(31);
            var armorPenaltyLine = $"{armorPenalty.Name,-16} {armorPenalty.Value,2}".PadRight(31);
            var wieldMaxLine = $"{wieldMax.Name,-16} {wieldMax.Value,1}/{wieldMax.MaxValue,-1}".PadRight(31);
            var huntLine = $"{hunt.Name,-16} {hunt.Value,2}({hunt.MaxValue,2})".PadRight(31);
            var familiarLine = $"{familiar.Name,-16} {familiar.Value,1:0}{familiar.MaxValue,-1:(0)}".PadRight(31);
            var fateLine = $"{fate.Name,-16} {fate.Value,2}({fate.MaxValue,2})".PadRight(31);
            var nameAndTitle = string.IsNullOrWhiteSpace(player.Title) ? player.Name : $"{player.Name}, {player.Title}";
            var pipe = "<%green%>|<%n%>";

            output.AppendLine("<%green%>+-------------------------------------------------------------------+");
            output.AppendLine($"{pipe} {nameAndTitle,-19} Level {"?",-6}  Reputation {"?",-6}  Kudos {"?",-6} {pipe}");
            output.AppendLine("<%green%>+----------------+----------------+----------------+----------------+");
            output.AppendLine($"{pipe} {healthLine} {pipe} {manaLine} {pipe}");
            output.AppendLine($"{pipe} {damageLine} {pipe} {initLine} {pipe}");
            output.AppendLine($"{pipe} {attackLine} {pipe} {defenseLine} {pipe}");
            output.AppendLine($"{pipe} {armorPenaltyLine} {pipe} {wieldMaxLine} {pipe}");
            output.AppendLine($"{pipe} {huntLine} {pipe} {familiarLine} {pipe}");
            output.AppendLine($"{pipe} {fateLine} {pipe} {string.Empty.PadRight(31)} {pipe}");
            output.AppendLine("<%green%>+---------------------------------^---------------------------------+<%n%>");

            return output;
        }
    }
}
