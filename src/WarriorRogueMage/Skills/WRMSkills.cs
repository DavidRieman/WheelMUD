//-----------------------------------------------------------------------------
// <copyright file="WRMSkills.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Karak
//   Date      : June 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage.Skills
{
    using WheelMUD.Core;

    /// <summary>Acrobatics Skill.</summary>
    [ExportGameSkill]
    public class SkillAcrobatics : WRMSkill
    {
        /// <summary>Initializes a new instance of the <see cref="SkillAcrobatics"/> class.</summary>
        public SkillAcrobatics() : base(
            "Acrobatics",
            "ROG",
            "Training in activities like dancing, contortion, climbing, tightrope, walking, tumbling.")
        {
        }
    }

    /// <summary>Alchemy Skill.</summary>
    [ExportGameSkill]
    public class SkillAlchemy : WRMSkill
    {
        /// <summary>Initializes a new instance of the <see cref="SkillAlchemy"/> class.</summary>
        public SkillAlchemy() : base(
            "Alchemy", 
            "MAG", 
            "Training in creating and identifying potions and salves.")
        {
        }
    }

    /// <summary>Athletics Skill.</summary>
    [ExportGameSkill]
    public class SkillAthletics : WRMSkill
    {
        /// <summary>Initializes a new instance of the <see cref="SkillAthletics"/> class.</summary>
        public SkillAthletics() : base(
            "Athletics", 
            "WAR", 
            "Training in swimming, running, and jumping.")
        {
        }
    }

    /// <summary>Awareness Skill.</summary>
    [ExportGameSkill]
    public class SkillAwareness : WRMSkill
    {
        /// <summary>Initializes a new instance of the <see cref="SkillAwareness"/> class.</summary>
        public SkillAwareness() : base(
            "Awareness", 
            "MAG", 
            "This skill is a measure of a character's awareness of his surroundings.")
        {
        }
    }

    /// <summary>Axes Skill.</summary>
    [ExportGameSkill]
    public class SkillAxes : WRMSkill
    {
        /// <summary>Initializes a new instance of the <see cref="SkillAxes"/> class.</summary>
        public SkillAxes() : base(
            "Axes", 
            "WAR", 
            "Training with axes.")
        {
        }
    }

    /// <summary>Blunt Skill.</summary>
    [ExportGameSkill]
    public class SkillBlunt : WRMSkill
    {
        /// <summary>Initializes a new instance of the <see cref="SkillBlunt"/> class.</summary>
        public SkillBlunt() : base(
            "Blunt", 
            "WAR", 
            "Training in all blunt weapons including maces and staves.")
        {
        }
    }

    /// <summary>Bows Skill.</summary>
    [ExportGameSkill]
    public class SkillBows : WRMSkill
    {
        /// <summary>Initializes a new instance of the <see cref="SkillBows"/> class.</summary>
        public SkillBows() : base("Bows", "ROG", "Skill for using bows and crossbows.")
        {
        }
    }

    /// <summary>Daggers Skill.</summary>
    [ExportGameSkill]
    public class SkillDaggers : WRMSkill
    {
        /// <summary>Initializes a new instance of the <see cref="SkillDaggers"/> class.</summary>
        public SkillDaggers() : base("Daggers", "ROG", "Training with daggers and knives.")
        {
        }
    }

    /// <summary>Driving Skill.</summary>
    [ExportGameSkill]
    public class SkillDriving : WRMSkill
    {
        /// <summary>Initializes a new instance of the <see cref="SkillDriving"/> class.</summary>
        public SkillDriving() : base("Driving", "WAR", "Training with driving vehicles.")
        {
        }
    }

    /// <summary>Firearms Skill.</summary>
    [ExportGameSkill]
    public class SkillFirearms : WRMSkill
    {
        /// <summary>Initializes a new instance of the <see cref="SkillFirearms"/> class.</summary>
        public SkillFirearms() : base("Firearms", "ROG", "Training in the usage of exotic firearms.")
        {
        }
    }

    /// <summary>Herbalism Skill.</summary>
    [ExportGameSkill]
    public class SkillHerbalism : WRMSkill
    {
        /// <summary>Initializes a new instance of the <see cref="SkillHerbalism"/> class.</summary>
        public SkillHerbalism() : base(
            "Herbalism",
            "MAG",
            "Knowledge of plants, herbs, and their medical uses. Can be used to heal critically wounded characters.")
        {
        }
    }

    /// <summary>Lore Skill.</summary>
    [ExportGameSkill]
    public class SkillLore : WRMSkill
    {
        /// <summary>Initializes a new instance of the <see cref="SkillLore"/> class.</summary>
        public SkillLore() : base("Lore", "MAG", "General knowledge.")
        {
        }
    }

    /// <summary>Polearms Skill.</summary>
    [ExportGameSkill]
    public class SkillPolearms : WRMSkill
    {
        /// <summary>Initializes a new instance of the <see cref="SkillPolearms"/> class.</summary>
        public SkillPolearms() : base("Polearms", "WAR", "Training with polearms, spears, and lances.")
        {
        }
    }

    /// <summary>Riding Skill.</summary>
    [ExportGameSkill]
    public class SkillRiding : WRMSkill
    {
        /// <summary>Initializes a new instance of the <see cref="SkillRiding"/> class.</summary>
        public SkillRiding() : base("Riding", "WAR", "Training in riding on horses and other common mounts.")
        {
        }
    }

    /// <summary>Swords Skill.</summary>
    [ExportGameSkill]
    public class SkillSwords : WRMSkill
    {
        /// <summary>Initializes a new instance of the <see cref="SkillSwords"/> class.</summary>
        public SkillSwords() : base("Swords", "WAR", "Training with all kinds of swords, including two-handed ones.")
        {
        }
    }

    /// <summary>Thaumaturgy Skill.</summary>
    [ExportGameSkill]
    public class SkillThaumaturgy : WRMSkill
    {
        /// <summary>Initializes a new instance of the <see cref="SkillThaumaturgy"/> class.</summary>
        public SkillThaumaturgy() : base("Thaumaturgy", "MAG", "Skilled with arcane spells and rituals.")
        {
        }
    }

    /// <summary>Thievery Skill.</summary>
    [ExportGameSkill]
    public class SkillThievery : WRMSkill
    {
        /// <summary>Initializes a new instance of the <see cref="SkillThievery"/> class.</summary>
        public SkillThievery() : base("Thievery", "ROG", "Training in the roguish arts like picking locks and picking pockets.")
        {
        }
    }

    /// <summary>Thrown Skill.</summary>
    [ExportGameSkill]
    public class SkillThrown : WRMSkill
    {
        /// <summary>Initializes a new instance of the <see cref="SkillThrown"/> class.</summary>
        public SkillThrown() : base("Thrown", "ROG", "Proficiency with thrown weapons like shuriken, kunais, and throwing daggers.")
        {
        }
    }

    /// <summary>Unarmed Skill.</summary>
    [ExportGameSkill]
    public class SkillUnarmed : WRMSkill
    {
        /// <summary>Initializes a new instance of the <see cref="SkillUnarmed"/> class.</summary>
        public SkillUnarmed() : base("Unarmed", "WAR", "Training with unarmed fighting.")
        {
        }
    }
}