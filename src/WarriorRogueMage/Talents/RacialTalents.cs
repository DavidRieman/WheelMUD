//-----------------------------------------------------------------------------
// <copyright file="RacialTalents.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage
{
    /// <summary>Beserker Talent.</summary>
    [ExportTalent]
    public class BeserkerTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="BeserkerTalent"/> class.</summary>
        public BeserkerTalent() : base(
            "Berserker",
            "This talent allows a character to go berserk during combat. Going berserk adds +2 to the Warrior attribute and all damage caused.",
            TalentType.Racial)
        {
        }
    }

    /// <summary>Exceptional Attribute Talent.</summary>
    [ExportTalent]
    public class ExceptionalAttributeTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="ExceptionalAttributeTalent"/> class.</summary>
        /// <param name="attribute">The attribute.</param>
        public ExceptionalAttributeTalent(string attribute) : base(
            "Exceptional Attribute",
            "This talent allows the player to roll two six sided dice instead of one when making a check using the relevant attribute. The highest result counts.",
            TalentType.Racial)
        {
            this.ExceptionalAttribute = attribute;
        }

        /// <summary>Initializes a new instance of the <see cref="ExceptionalAttributeTalent"/> class.</summary>
        public ExceptionalAttributeTalent() : this(null)
        {
        }

        /// <summary>Gets the attribute which is exceptional.</summary>
        public string ExceptionalAttribute { get; private set; }
    }

    /// <summary>Natural Armor Talent.</summary>
    [ExportTalent]
    public class NaturalArmorTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="NaturalArmorTalent"/> class.</summary>
        /// <param name="defenseBonus">The defense bonus.</param>
        public NaturalArmorTalent(int defenseBonus) : base(
            "Natural Armor",
            "Natural armor usually is in the form of scales or thick fur that protects the character from damage. The Defense granted by natural armor works as long as no other armor is worn.",
            TalentType.Racial)
        {
            this.DefenseBonus = defenseBonus;
        }

        /// <summary>Initializes a new instance of the <see cref="NaturalArmorTalent"/> class.</summary>
        public NaturalArmorTalent() : this(2)
        {
        }

        /// <summary>Gets the defense bonus.</summary>
        public int DefenseBonus { get; private set; }
    }

    /// <summary>No Talent For Magic Talent.</summary>
    [ExportTalent]
    public class NoTalentForMagicTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="NoTalentForMagicTalent"/> class.</summary>
        public NoTalentForMagicTalent() : base(
            "No Talent For Magic",
            "A character with this talent has a hard time grasping the concepts of magic or has a natural resistance to channeling mana. When making a casting check, the character rolls two d6 and takes the lowest result. In addition to that, all base mana costs for spells are doubled. The armor penalty for armor worn remains the same.",
            TalentType.Racial)
        {
        }
    }

    /// <summary>Outcast Talent.</summary>
    [ExportTalent]
    public class OutcastTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="OutcastTalent"/> class.</summary>
        public OutcastTalent() : base(
            "Outcast",
            "A character with this racial talent is considered an outcast in most societies. Every test related to social interaction with a member of a different race is modified by -3.",
            TalentType.Racial)
        {
        }
    }

    /// <summary>Tinkerer Talent.</summary>
    [ExportTalent]
    public class TinkererTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="TinkererTalent"/> class.</summary>
        public TinkererTalent() : base(
            "Tinkerer",
            "This talent grants a +2 bonus on all checks related to repairing, dismantling or using technical equipment like mechanical traps, firearms, war golems, clockwork, etc.",
            TalentType.Racial)
        {
        }
    }

    /// <summary>Weak Talent.</summary>
    [ExportTalent]
    public class WeakTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="WeakTalent"/> class.</summary>
        public WeakTalent() : base(
            "Weak",
            "A character with this talent starts play with hit points equal to 3 + the Warrior attribute, and when leveling up only gains 1d6-2 hit points (minimum 1 point).",
            TalentType.Racial)
        {
        }
    }
}