//-----------------------------------------------------------------------------
// <copyright file="TestChampionTalent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   Tests the Champion talent.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Talents
{
    using NUnit.Framework;
    using WarriorRogueMage;
    using WarriorRogueMage.Attributes;
    using WarriorRogueMage.Behaviors;
    using WarriorRogueMage.Stats;
    using WheelMUD.Core;

    [TestFixture]
    public class TestChampionTalent
    {
        /// <summary>Common actors in the test.</summary>
        private Thing playerThing;

        [SetUp]
        public void Init()
        {
            var testBehavior = new TalentsBehavior(null);
            var warriorAttribute = new WarriorAttribute();
            var rogueAttribute = new RogueAttribute();
            var mageAttribute = new MageAttribute();
            var damageStat = new DamageStat();
            var attackStat = new AttackStat();

            this.playerThing = new Thing() { Name = "PlayerThing", Id = TestThingID.Generate("testthing") };

            this.playerThing.Behaviors.Add(testBehavior);

            warriorAttribute.Parent = this.playerThing;
            this.playerThing.AddAttribute(warriorAttribute);

            mageAttribute.Parent = this.playerThing;
            this.playerThing.AddAttribute(rogueAttribute);

            rogueAttribute.Parent = this.playerThing;
            this.playerThing.AddAttribute(mageAttribute);
            
            warriorAttribute.SetValue(10, this.playerThing);
            rogueAttribute.SetValue(10, this.playerThing);
            mageAttribute.SetValue(10, this.playerThing);

            this.playerThing.Stats.Add(damageStat.Name, damageStat);
            this.playerThing.Stats.Add(attackStat.Name, attackStat);
        }

        /// <summary>Tests the champion talent added mechanism.</summary>
        [Test]
        public void TestChampionTalentAddedMechanism()
        {
            var champion = new ChampionTalent();

            this.playerThing.Behaviors.FindFirst<TalentsBehavior>().AddTalent(champion);

            var behavior = this.playerThing.Behaviors.FindFirst<TalentsBehavior>();

            Assert.IsTrue(behavior.ManagedTalents.Contains(champion));
            Assert.IsNotNull(behavior.FindFirst<ChampionTalent>().PlayerThing);

            behavior.RemoveTalent(champion);
        }

        /// <summary>Tests the champion talent auto set rule.</summary>
        [Test]
        public void TestChampionTalentAutosetRule()
        {
            var champion = new ChampionTalent();

            var behavior = this.playerThing.Behaviors.FindFirst<TalentsBehavior>();
            
            var attackStat = this.playerThing.FindGameStat("Attack");
            int oldAttackValue = attackStat.Value;
            
            var damageStat = this.playerThing.FindGameStat("Damage");
            int oldDamageValue = damageStat.Value;

            behavior.AddTalent(champion);

            Assert.AreNotEqual(oldAttackValue, attackStat.Value);
            Assert.AreNotEqual(oldDamageValue, damageStat.Value);

            behavior.RemoveTalent(champion);

            Assert.AreEqual(oldAttackValue, attackStat.Value);
            Assert.AreEqual(oldDamageValue, damageStat.Value);
        }
    }
}
