//-----------------------------------------------------------------------------
// <copyright file="WRMStats.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Karak
//   Date      : June 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage.Stats
{
    using WheelMUD.Core;

    /// <summary>Fate Stat.</summary>
    [ExportGameStat]
    public class FateStat : WRMStat
    {
        /// <summary>Initializes a new instance of the <see cref="FateStat"/> class.</summary>
        public FateStat() : base("Fate", "FATE", "ROG", 0, 0, 0)
        {
        }
    }

    /// <summary>Defense Stat.</summary>
    [ExportGameStat]
    public class DefenseStat : WRMStat
    {
        /// <summary>Initializes a new instance of the <see cref="DefenseStat"/> class.</summary>
        public DefenseStat() : base("Defense", "DEF", "((WAR+ROG)/2)+4", 0, 0, 0)
        {
        }
    }

    /// <summary>Armor Penalty Stat.</summary>
    [ExportGameStat]
    public class ArmorPenaltyStat : WRMStat
    {
        /// <summary>Initializes a new instance of the <see cref="ArmorPenaltyStat"/> class.</summary>
        public ArmorPenaltyStat() : base("Armor Penalty", "ARMORPENALTY", "ARMORPENALTY", 0, 0, 0)
        {
        }
    }

    /// <summary>Attack Stat.</summary>
    [ExportGameStat]
    public class AttackStat : WRMStat
    {
        /// <summary>Initializes a new instance of the <see cref="AttackStat"/> class.</summary>
        public AttackStat() : base("Attack", "ATK", "ATK", 0, 0, 0)
        {
        }
    }

    /// <summary>Damage Stat.</summary>
    [ExportGameStat]
    public class DamageStat : WRMStat
    {
        /// <summary>Initializes a new instance of the <see cref="DamageStat"/> class.</summary>
        public DamageStat() : base("Damage", "DAMAGE", "DAMAGE", 0, 0, 0)
        {
        }
    }

    /// <summary>Wield Maximum Stat.</summary>
    [ExportGameStat]
    public class WeaponWieldMaximumStat : WRMStat
    {
        /// <summary>Initializes a new instance of the <see cref="WeaponWieldMaximumStat"/> class.</summary>
        public WeaponWieldMaximumStat() : base("Weapon Wield Max", "WEAPONWIELDMAX", "WEAPONWIELDMAX", 1, 1, 2)
        {
        }
    }

    /// <summary>Hit Points Stat.</summary>
    [ExportGameStat]
    public class HitPointsStat : WRMStat
    {
        /// <summary>Initializes a new instance of the <see cref="HitPointsStat"/> class.</summary>
        public HitPointsStat() : base("Hit Points", "HP", "WARRIOR+6", 0, 0, 0)
        {
        }
    }

    /// <summary>Mana Stat.</summary>
    [ExportGameStat]
    public class ManaStat : WRMStat
    {
        /// <summary>Initializes a new instance of the <see cref="ManaStat"/> class.</summary>
        public ManaStat() : base("Mana", "MANA", "MAGE*2", 0, 0, 0)
        {
        }
    }

    /// <summary>Initiative Stat.</summary>
    [ExportGameStat]
    public class InitiativeStat : WRMStat
    {
        /// <summary>Initializes a new instance of the <see cref="InitiativeStat"/> class.</summary>
        public InitiativeStat() : base("Initiative", "INIT", "MAGE*2", 0, 0, 0)
        {
        }
    }

    /// <summary>Hunter Stat.</summary>
    [ExportGameStat]
    public class HunterStat : WRMStat
    {
        /// <summary>Initializes a new instance of the <see cref="HunterStat"/> class.</summary>
        public HunterStat() : base("Hunter", "HUNT", "ROGUE", 0, 0, 0)
        {
        }
    }

    /// <summary>Familiar Stat.</summary>
    [ExportGameStat]
    public class FamiliarStat : WRMStat
    {
        /// <summary>Initializes a new instance of the <see cref="FamiliarStat"/> class.</summary>
        public FamiliarStat() : base("Familiar", "FAMILIAR", "MAGE", 0, 0, 0)
        {
        }
    }
}