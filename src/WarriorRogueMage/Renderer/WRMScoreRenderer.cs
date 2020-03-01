//-----------------------------------------------------------------------------
// <copyright file="WRMScoreRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Linq;
    using System.Text;
    using WheelMUD.Effects;

    /// <summary>The WRM game system score renderer.</summary>
    /// <remarks>Overrides the default WheelMUD score renderer, to present game-system-specific details.</remarks>
    [RendererExports.Score(100)]
    public class DefaultScoreRenderer : RendererDefinitions.Score
    {
        public override string Render(Thing player)
        {
            var stats = player.Stats;
            var statEffects = player.Behaviors.OfType<StatEffect>();
            var sb = new StringBuilder();

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
            var nameAndTitle = string.IsNullOrWhiteSpace(player.Title) ? player.Name : $"{player.Name}, {player.Title}";
            var pipe = "<%green%>|<%n%>";

            sb.AppendLine($"<%green%>+-------------------------------------------------------------------+");
            sb.AppendLine($"{pipe} {nameAndTitle,-19} Level {"?",-6}  Reputation {"?",-6}  Kudos {"?",-6} {pipe}");
            sb.AppendLine($"<%green%>+----------------+----------------+----------------+----------------+");
            sb.AppendLine($"{pipe} {healthLine} {pipe} {manaLine} {pipe}");
            sb.AppendLine($"{pipe} {damageLine} {pipe} {initLine} {pipe}");
            sb.AppendLine($"{pipe} {attackLine} {pipe} {defenseLine} {pipe}");
            sb.AppendLine($"{pipe} {armorPenaltyLine} {pipe} {wieldMaxLine} {pipe}");
            sb.AppendLine($"{pipe} {huntLine} {pipe} {familiarLine} {pipe}");
            sb.AppendLine($"{pipe} {fateLine} {pipe} {string.Empty.PadRight(31)} {pipe}");
            sb.AppendLine($"<%green%>+---------------------------------^---------------------------------+<%n%>");

            return sb.ToString();
        }
    }
}
