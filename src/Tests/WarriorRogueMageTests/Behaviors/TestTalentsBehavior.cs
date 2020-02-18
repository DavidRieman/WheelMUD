//-----------------------------------------------------------------------------
// <copyright file="TestTalentsBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   Tests the TalentsBehavior class.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Behaviors
{
    using NUnit.Framework;
    using WarriorRogueMage;
    using WarriorRogueMage.Attributes;
    using WarriorRogueMage.Behaviors;
    using WarriorRogueMage.Stats;
    using WheelMUD.Core;

    [TestFixture]
    public class TestTalentsBehavior
    {
        /// <summary>Common actors in the test.</summary>
        private Thing playerThing;

        [SetUp]
        public void Init()
        {
            this.playerThing = new Thing() { Name = "PlayerThing", Id = TestThingID.Generate("testthing") };
        }

        /// <summary>Test to make sure WRM behaviors are attaching properly.</summary>
        [Test]
        public void AttachTalentsBehaviorToPlayerTest()
        {
            var testBehavior = new TalentsBehavior(null);
            this.playerThing.Behaviors.Add(testBehavior);
            Assert.IsNotNull(this.playerThing.Behaviors.FindFirst<TalentsBehavior>());
            this.playerThing.Behaviors.Remove(testBehavior);
        } 

        [Test]
        public void AddTalentBeforeBehaviorParentSetTest()
        {
            var testTalent = new CraftsmanTalent();
            var testBehavior = new TalentsBehavior(null);
            testBehavior.AddTalent(testTalent);
            this.playerThing.Behaviors.Add(testBehavior);
            var behavior = this.playerThing.Behaviors.FindFirst<TalentsBehavior>();

            Assert.IsTrue(behavior.ManagedTalents.Contains(testTalent));
            Assert.IsNotNull(testTalent.PlayerThing);

            behavior.RemoveTalent(testTalent);
            this.playerThing.Behaviors.Remove(testBehavior);
        }

        [Test]
        public void AddTalentWithRulesBeforeBehaviorParentSetTest()
        {
            var testBehavior = new TalentsBehavior(null);
            var warriorAttribute = new WarriorAttribute();
            var rogueAttribute = new RogueAttribute();
            var mageAttribute = new MageAttribute();
            var damageStat = new DamageStat();
            var attackStat = new AttackStat();

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

            var testTalent = new ChampionTalent();
            this.playerThing.Behaviors.Add(testBehavior);

            var behavior = this.playerThing.Behaviors.FindFirst<TalentsBehavior>();
            behavior.AddTalent(testTalent);
            Assert.IsTrue(behavior.ManagedTalents.Contains(testTalent));
            Assert.IsNotNull(testTalent.PlayerThing);

            behavior.RemoveTalent(testTalent);
            this.playerThing.Behaviors.Remove(testBehavior);
        }

        [Test]
        public void AddBehaviorBeforeTalentParentSetTest()
        {
            var testTalent = new CraftsmanTalent();
            var testBehavior = new TalentsBehavior(null);
            this.playerThing.Behaviors.Add(testBehavior);
            var behavior = this.playerThing.Behaviors.FindFirst<TalentsBehavior>();
            testBehavior.AddTalent(testTalent);
            Assert.IsTrue(behavior.ManagedTalents.Contains(testTalent));
            Assert.IsNotNull(testTalent.PlayerThing);

            behavior.RemoveTalent(testTalent);
            this.playerThing.Behaviors.Remove(testBehavior);
        }

        [Test]
        public void AddBehaviorBeforeTalentWithRulesParentSetTest()
        {
            var testBehavior = new TalentsBehavior(null);
            var warriorAttribute = new WarriorAttribute();
            var rogueAttribute = new RogueAttribute();
            var mageAttribute = new MageAttribute();
            var damageStat = new DamageStat();
            var attackStat = new AttackStat();

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
            var testTalent = new ChampionTalent();
            this.playerThing.Behaviors.Add(testBehavior);
            var behavior = this.playerThing.Behaviors.FindFirst<TalentsBehavior>();
            testBehavior.AddTalent(testTalent);
            Assert.IsTrue(behavior.ManagedTalents.Contains(testTalent));
            Assert.IsNotNull(testTalent.PlayerThing);

            behavior.RemoveTalent(testTalent);
            this.playerThing.Behaviors.Remove(testBehavior);
        }
    }
}
