//-----------------------------------------------------------------------------
// <copyright file="NormalTalent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage
{
    using WarriorRogueMage.Rules;
    using WheelMUD.Core;

    /// <summary>Armored Caster Talent.</summary>
    [ExportTalent]
    public class ArmoredCasterTalent : Talent
    {
        /// <summary>Initializes a new instance of the <see cref="ArmoredCasterTalent"/> class.</summary>
        public ArmoredCasterTalent()
            : base(
                "Armored Caster",
                "You may reduce the armor penalty by 2. May be taken more than once.",
                TalentType.Normal,
            "<rule type=\"decrease\">ARMORPENALTY-2</rule>")
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
                TalentType.Normal,
            "<rule type=\"assign\">HP</rule>")
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
                TalentType.Normal,
            "<rule type=\"increase\">ATTACK+2</rule>",
            "<rule type=\"increase\">DAMAGE+2</rule>")
        {
        }

        /// <summary>Called when a parent has just been assigned to this talent. (Refer to this.PlayerThing)</summary>
        public override void OnAddTalent()
        {
            new IncreaseStatRule().Execute(this.PlayerThing, "Damage", 2);
            new IncreaseStatRule().Execute(this.PlayerThing, "Attack", 2);

            base.OnAddTalent();
        }

        /// <summary>Called when the current parent of this talent is about to be removed. (Refer to this.PlayerThing)</summary>
        public override void OnRemoveTalent()
        {
            new DecreaseStatRule().Execute(this.PlayerThing, "Damage", 2);
            new DecreaseStatRule().Execute(this.PlayerThing, "Attack", 2);

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
                TalentType.Normal,
            "<rule type=\"increase\">DAMAGE+MAGE</rule>")
        {
        }

        /// <summary>Called when a parent has just been assigned to this talent. (Refer to this.PlayerThing)</summary>
        public override void OnAddTalent()
        {
            new AddAttributeToStatRule<GameAttribute, GameStat>().Execute(this.PlayerThing, "Mage", "Damage");

            base.OnAddTalent();
        }

        /// <summary>Called when the current parent of this talent is about to be removed. (Refer to this.PlayerThing)</summary>
        public override void OnRemoveTalent()
        {
            new SubstractStatFromAttributeRule<GameAttribute, GameStat>().Execute(this.PlayerThing, "Mage", "Damage");

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
                "Craftsman", "You are trained in a craft like blacksmithing, carpentry or bowmaking.", TalentType.Normal)
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
                TalentType.Normal,
            "<rule type=\"assign\">WEAPONWIELDMAX=2</rule>")
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
                TalentType.Normal,
            "<rule type=\"assign\">FAMILIAR=1</rule>")
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
                TalentType.Normal,
            "<rule type=\"increase\">HENCHMAN+1</rule>")
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
                TalentType.Normal,
            "<rule type=\"assign\">HUNTER=True</rule>")
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
                TalentType.Normal,
            "<rule type=\"increase\">DAMAGE+WARRIOR</rule>")
        {
        }

        /// <summary>Called when a parent has just been assigned to this talent. (Refer to this.PlayerThing.)</summary>
        public override void OnAddTalent()
        {
            new AddAttributeToStatRule<GameAttribute, GameStat>().Execute(this.PlayerThing, "Warrior", "Damage");

            base.OnAddTalent();
        }

        /// <summary>Called when the current parent of this talent is about to be removed. (Refer to this.PlayerThing.)</summary>
        public override void OnRemoveTalent()
        {
            new SubstractStatFromAttributeRule<GameAttribute, GameStat>().Execute(this.PlayerThing, "Warrior", "Damage");

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
                TalentType.Normal,
            "<rule type=\"increase\">DAMAGE+ROGUE</rule>")
        {
        }

        /// <summary>Called when a parent has just been assigned to this talent. (Refer to this.PlayerThing.)</summary>
        public override void OnAddTalent()
        {
            new AddAttributeToStatRule<GameAttribute, GameStat>().Execute(this.PlayerThing, "Rogue", "Damage");

            base.OnAddTalent();
        }

        /// <summary>Called when the current parent of this talent is about to be removed. (Refer to this.PlayerThing.)</summary>
        public override void OnRemoveTalent()
        {
            new SubstractStatFromAttributeRule<GameAttribute, GameStat>().Execute(this.PlayerThing, "Rogue", "Damage");

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
                TalentType.Normal,
            "<rule type=\"assign\">SEANAUSEA=False</rule>")
        {
            // @@@ TODO: Use an Effect object?
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
                TalentType.Normal,
            "<rule type=\"assign\">INITIATIVE = (1d6 => 4)</rule>")
        {
        }

        /// <summary>Called when the game engine, or other systems, need to activate the talent.</summary>
        /// <remarks>Some talents are not automatic and can only be used/activated in certain situations.</remarks>
        public override void OnActivateTalent()
        {
            ////new RollDieRule("1d6").Execute(this.PlayerThing, "Initiative", 4);
            base.OnActivateTalent();
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
                TalentType.Normal,
            "<rule type=\"subtract\">RECIEVEDDAMAGE-2</rule>")
        {
            // @@@ TODO: Use an Effect object?
        }
    }
}