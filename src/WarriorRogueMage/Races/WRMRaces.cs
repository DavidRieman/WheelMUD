//-----------------------------------------------------------------------------
// <copyright file="WRMRaces.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage
{
    using WheelMUD.Core;

    /// <summary>Class that represents the Human race.</summary>
    [ExportGameRace]
    public class HumanRace : WRMRace
    {
        /// <summary>Initializes a new instance of the <see cref="HumanRace"/> class.</summary>
        public HumanRace() : base(
            "Human",
            "Humans are you standard bipedal humanoid. They are born without any natural abilities, but they make up this shortcoming by being the most versatile race in this realm. They can pretty much learn anything.")
        {
        }
    }

    /// <summary>Class that represents the Elven race.</summary>
    [ExportGameRace]
    public class ElfRace : WRMRace
    {
        /// <summary>Initializes a new instance of the <see cref="ElfRace"/> class.</summary>
        public ElfRace() : base(
            "Elf",
            "Elves are usually slender, beautiful humanoids with slightly elongated limbs and pointed ears. Elves have less body hair than humans, have an exceptional talent for magic and live much longer than humans, but mature about as fast.",
            new ExceptionalAttributeTalent("Mage"),
            new SixthSenseTalent(),
            new WeakTalent())
        {
        }
    }

    /// <summary>Class that represents the Dwarven race.</summary>
    [ExportGameRace]
    public class DwarfRace : WRMRace
    {
        /// <summary>Initializes a new instance of the <see cref="DwarfRace"/> class.</summary>
        public DwarfRace() : base(
            "Dwarf",
            "Dwarves are shorter and stockier than humans. Male dwarves usually sport thick, long beards. Dwarves are very strong and tough for their small size and are known for their fighting prowess. Dwarves live longer than humans, but not as long as elves.",
            new CraftsmanTalent(),
            new ExceptionalAttributeTalent("Warrior"),
            new NoTalentForMagicTalent())
        {
        }
    }

    /// <summary>Class that represents the Halfling race.</summary>
    [ExportGameRace]
    public class HalflingRace : WRMRace
    {
        /// <summary>Initializes a new instance of the <see cref="HalflingRace"/> class.</summary>
        public HalflingRace() : base(
            "Halfling",
            "Halflings are even shorter than Dwarves and are sometimes mistaken for human children. They usually live peaceful lives far away from the bustling towns of larger folk, but if one of the members of his race goes adventuring, their exceptional talent for thievery shines.",
            new ExceptionalAttributeTalent("Rogue"),
            new HunterTalent(),
            new WeakTalent())
        {
        }
    }

    /// <summary>Class that represents the Lizardmen race.</summary>
    [ExportGameRace]
    public class LizardmanRace : WRMRace
    {
        /// <summary>Initializes a new instance of the <see cref="LizardmanRace"/> class.</summary>
        public LizardmanRace() : base(
            "Lizardman",
            "Lizardmen are an intelligent species that evolved from lizards. Though they are warm-blooded, they still prefer hot and dry areas. They resemble humanoid lizards with scales instead of skin (which also double as armor). Lizardmen are known for their lack of strong emotions and their keen logic.",
            new NaturalArmorTalent(2),
            new OutcastTalent(),
            new ToughAsNailsTalent())
        {
        }
    }

    /// <summary>Class that represents the Goblin race.</summary>
    [ExportGameRace]
    public class GoblinRace : WRMRace
    {
        /// <summary>Initializes a new instance of the <see cref="GoblinRace"/> class.</summary>
        public GoblinRace() : base(
            "Goblin",
            "Goblins are the smallest of the green skin races. They have green skin and very long pointed ears, and are about three feet tall. They have high voices and mouths full of razor-sharp teeth. Goblins seem to have a knack for salvaging and tinkering.",
            new LuckyDevilTalent(),
            new TinkererTalent(),
            new WeakTalent())
        {
        }
    }

    /// <summary>Class that represents the Orc race.</summary>
    [ExportGameRace]
    public class OrcRace : WRMRace
    {
        /// <summary>Initializes a new instance of the <see cref="OrcRace"/> class.</summary>
        public OrcRace()
            : base(
                  "Orc",
                "Orcs are slightly larger than humans, green skinned and are exceptionally strong. They can savage warriors, but they have a strong sense of honor. They usually look down on smaller races like Goblins and Halflings. Orcish adventurers are a rare sight, but often they are accompanied by one of their tame black wolves.",
                new BeserkerTalent(),
                new ExceptionalAttributeTalent("Warrior"),
                new OutcastTalent())
        {
        }
    }
}