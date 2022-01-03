//-----------------------------------------------------------------------------
// <copyright file="NormalTalent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

// TODO: Implement modifiers. See: https://github.com/DavidRieman/WheelMUD/discussions/31 for starters.
// TODO: Some of these need special behaviors or circumstances and need specific non-modifier code too.
namespace WarriorRogueMage
{
    /// <summary>Armored Caster Talent.</summary>
    [ExportTalent]
    public class ArmoredCasterTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="ArmoredCasterTalent"/> class.</summary>
        public ArmoredCasterTalent()
            : base(
                "Armored Caster",
                "You may reduce the armor penalty by 2. May be taken more than once.",
                TalentType.Normal) // TODO: ARMORPENALTY-2
        {
        }
    }

    /// <summary>BloodMage Talent.</summary>
    [ExportTalent]
    public class BloodMageTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="BloodMageTalent"/> class.</summary>
        public BloodMageTalent()
            : base(
                "Blood Mage",
                "You may use hit points instead of mana when casting spells. You may use your HP for some or all of the cost of a spell when it is cast.",
                TalentType.Normal)
        {
        }
    }

    /// <summary>Champion Talent.</summary>
    [ExportTalent]
    public class ChampionTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="ChampionTalent"/> class.</summary>
        public ChampionTalent()
            : base(
                "Champion",
                "Select a cause. You get a +2 bonus on attack and damage rolls against enemies of that cause. May be taken more than once.",
                TalentType.Normal) // TODO: ATTACK+2, DAMAGE+2
        {
        }

        /// <summary>Called when a parent has just been assigned to this talent. (Refer to PlayerThing)</summary>
        public override void OnAddTalent()
        {
            base.OnAddTalent();
        }

        /// <summary>Called when the current parent of this talent is about to be removed. (Refer to PlayerThing)</summary>
        public override void OnRemoveTalent()
        {
            base.OnRemoveTalent();
        }
    }

    /// <summary>Channeler Talent.</summary>
    [ExportTalent]
    public class ChannelerTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="ChannelerTalent"/> class.</summary>
        public ChannelerTalent()
            : base(
                "Channeler",
                "You may add your Mage attribute level to your magic attack damage once per combat.",
                TalentType.Normal) // TODO: DAMAGE+MAGE (but needs to track last combat target, check which kind of damage)
        {
        }

        /// <summary>Called when a parent has just been assigned to this talent. (Refer to PlayerThing)</summary>
        public override void OnAddTalent()
        {
            base.OnAddTalent();
        }

        /// <summary>Called when the current parent of this talent is about to be removed. (Refer to PlayerThing)</summary>
        public override void OnRemoveTalent()
        {
            base.OnRemoveTalent();
        }
    }

    /// <summary>Craftsman Talent.</summary>
    [ExportTalent]
    public class CraftsmanTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="CraftsmanTalent"/> class.</summary>
        public CraftsmanTalent()
            : base(
                "Craftsman",
                "You are trained in a craft like blacksmithing, carpentry or bowmaking.",
                TalentType.Normal) // TODO: Perhaps adds context commands to player?
        {
        }
    }

    /// <summary>Dual Wielding Talent.</summary>
    [ExportTalent]
    public class DualWieldingTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="DualWieldingTalent"/> class.</summary>
        public DualWieldingTalent()
            : base(
                "Dual Wielding",
                "You may wield a weapon in your off-hand without penalty. Does not grant and extra attack.",
                TalentType.Normal) // TODO: WEAPONWIELDMAX=2, or implement another way?
        {
        }
    }

    /// <summary>Familiar Talent.</summary>
    [ExportTalent]
    public class FamiliarTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="FamiliarTalent"/> class.</summary>
        public FamiliarTalent()
            : base(
                "Familiar",
                "You have a small animal like a cat or falcon as a pet that can do some simple tricks.",
                TalentType.Normal) // TODO: FAMILIAR=1, or implement another way?
        {
        }
    }

    /// <summary>Henchman Talent.</summary>
    [ExportTalent]
    public class HenchmanTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="HenchmanTalent"/> class.</summary>
        public HenchmanTalent()
            : base(
                "Henchman",
                "You are followed by a henchman who caries your equipment and treasure around and may be asked to perform tasks.",
                TalentType.Normal) // TODO: HENCHMAN+1, or implement another way?
        {
        }
    }

    /// <summary>Hunter Talent.</summary>
    [ExportTalent]
    public class HunterTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="HunterTalent"/> class.</summary>
        public HunterTalent()
            : base(
                "Hunter",
                "You are are a trained hunter and may live off the land easily. When given enough time, you can provide enough food to feed a party of four.",
                TalentType.Normal) // TODO: Perhaps adds context commands to player?
        {
        }
    }

    /// <summary>Leadership Talent.</summary>
    [ExportTalent]
    public class LeadershipTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="LeadershipTalent"/> class.</summary>
        public LeadershipTalent()
            : base("Leadership", "You are a talented leader and may command troops.", TalentType.Normal)
        {
        }
    }

    /// <summary>Lucky Devil Talent.</summary>
    [ExportTalent]
    public class LuckyDevilTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="LuckyDevilTalent"/> class.</summary>
        public LuckyDevilTalent()
            : base("Lucky Devil", "You may reroll any roll once per scene (or combat).", TalentType.Normal)
        {
        }
    }

    /// <summary>Massive Attack Talent.</summary>
    [ExportTalent]
    public class MassiveAttackTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="MassiveAttackTalent"/> class.</summary>
        public MassiveAttackTalent()
            : base(
                "Massive Attack",
                "You can add your Warrior attribute level to your melee attack damage once per combat.",
                TalentType.Normal) // TODO: DAMAGE+WARRIOR (but needs to track last combat target, check which kind of damage)
        {
        }

        /// <summary>Called when a parent has just been assigned to this talent. (Refer to PlayerThing.)</summary>
        public override void OnAddTalent()
        {
            base.OnAddTalent();
        }

        /// <summary>Called when the current parent of this talent is about to be removed. (Refer to PlayerThing.)</summary>
        public override void OnRemoveTalent()
        {
            base.OnRemoveTalent();
        }
    }

    /// <summary>Precise Shot Talent.</summary>
    [ExportTalent]
    public class PreciseShotTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="PreciseShotTalent"/> class.</summary>
        public PreciseShotTalent()
            : base(
                "Precise Shot",
                "You can add your Rogue attribute level to your ranged attack damage once per combat.",
                TalentType.Normal) // TODO: DAMAGE+ROGUE (but needs to track last combat target, check which kind of damage)
        {
        }

        /// <summary>Called when a parent has just been assigned to this talent. (Refer to PlayerThing.)</summary>
        public override void OnAddTalent()
        {
            base.OnAddTalent();
        }

        /// <summary>Called when the current parent of this talent is about to be removed. (Refer to PlayerThing.)</summary>
        public override void OnRemoveTalent()
        {
            base.OnRemoveTalent();
        }
    }

    /// <summary>Sailor Talent.</summary>
    [ExportTalent]
    public class SailorTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="SailorTalent"/> class.</summary>
        public SailorTalent()
            : base(
                "Sailor",
                "You are trained in steering a boat or sailing ship and don't get any penalties for fighting on a sea vessel.",
                TalentType.Normal) // TODO: Add contextual commands for this player, and custom boost for offsetting penalties?
        {
        }
    }

    /// <summary>Sixth Sense Talent.</summary>
    [ExportTalent]
    public class SixthSenseTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="SixthSenseTalent"/> class.</summary>
        public SixthSenseTalent()
            : base(
                "Sixth Sense",
                "You may roll a die before any ambush or other situation where you are about to be surprised. If you roll 4+, you are not surprised and may act first.",
                TalentType.Normal) // TODO: Custom implementation, or just a massive modifier half the time to initiative stat checks?
        {
        }
    }

    /// <summary>Tough As Nails Talent.</summary>
    [ExportTalent]
    public class ToughAsNailsTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="ToughAsNailsTalent"/> class.</summary>
        public ToughAsNailsTalent()
            : base(
                "Tough As Nails",
                "Damage you take from an individual attack is reduced by 2.",
                TalentType.Normal) // TODO: RECIEVEDDAMAGE-2 or DAMAGEREDUCTION stat?
        {
        }
    }
}