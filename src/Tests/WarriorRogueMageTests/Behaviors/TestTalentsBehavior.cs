//-----------------------------------------------------------------------------
// <copyright file="TestTalentsBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Behaviors
{
    using NUnit.Framework;
    using WarriorRogueMage;
    using WarriorRogueMage.Attributes;
    using WarriorRogueMage.Behaviors;
    using WarriorRogueMage.Stats;
    using WheelMUD.Core;

    /// <summary>Tests the TalentsBehavior class.</summary>
    [TestFixture]
    public class TestTalentsBehavior
    {
        /// <summary>Common actors in the test.</summary>
        private Thing playerThing;

        [SetUp]
        public void Init()
        {
            playerThing = new Thing() { Name = "PlayerThing", Id = TestThingID.Generate("testthing") };
        }

        /// <summary>Test to make sure WRM behaviors are attaching properly.</summary>
        [Test]
        public void AttachTalentsBehaviorToPlayerTest()
        {
            var testBehavior = new TalentsBehavior(null);
            playerThing.Behaviors.Add(testBehavior);
            Assert.IsNotNull(playerThing.FindBehavior<TalentsBehavior>());
            playerThing.Behaviors.Remove(testBehavior);
        }

        [Test]
        public void AddTalentBeforeBehaviorParentSetTest()
        {
            var testTalent = new CraftsmanTalent();
            var testBehavior = new TalentsBehavior(null);
            testBehavior.AddTalent(testTalent);
            playerThing.Behaviors.Add(testBehavior);
            var behavior = playerThing.FindBehavior<TalentsBehavior>();

            Assert.IsTrue(behavior.ManagedTalents.Contains(testTalent));
            Assert.IsNotNull(testTalent.PlayerThing);

            behavior.RemoveTalent(testTalent);
            playerThing.Behaviors.Remove(testBehavior);
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

            warriorAttribute.Parent = playerThing;
            playerThing.AddAttribute(warriorAttribute);

            mageAttribute.Parent = playerThing;
            playerThing.AddAttribute(rogueAttribute);

            rogueAttribute.Parent = playerThing;
            playerThing.AddAttribute(mageAttribute);

            warriorAttribute.SetValue(10, playerThing);
            rogueAttribute.SetValue(10, playerThing);
            mageAttribute.SetValue(10, playerThing);

            playerThing.Stats.Add(damageStat.Name, damageStat);
            playerThing.Stats.Add(attackStat.Name, attackStat);

            var testTalent = new ChampionTalent();
            playerThing.Behaviors.Add(testBehavior);

            var behavior = playerThing.FindBehavior<TalentsBehavior>();
            behavior.AddTalent(testTalent);
            Assert.IsTrue(behavior.ManagedTalents.Contains(testTalent));
            Assert.IsNotNull(testTalent.PlayerThing);

            behavior.RemoveTalent(testTalent);
            playerThing.Behaviors.Remove(testBehavior);
        }

        [Test]
        public void AddBehaviorBeforeTalentParentSetTest()
        {
            var testTalent = new CraftsmanTalent();
            var testBehavior = new TalentsBehavior(null);
            playerThing.Behaviors.Add(testBehavior);
            var behavior = playerThing.FindBehavior<TalentsBehavior>();
            testBehavior.AddTalent(testTalent);
            Assert.IsTrue(behavior.ManagedTalents.Contains(testTalent));
            Assert.IsNotNull(testTalent.PlayerThing);

            behavior.RemoveTalent(testTalent);
            playerThing.Behaviors.Remove(testBehavior);
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

            warriorAttribute.Parent = playerThing;
            playerThing.AddAttribute(warriorAttribute);

            mageAttribute.Parent = playerThing;
            playerThing.AddAttribute(rogueAttribute);

            rogueAttribute.Parent = playerThing;
            playerThing.AddAttribute(mageAttribute);

            warriorAttribute.SetValue(10, playerThing);
            rogueAttribute.SetValue(10, playerThing);
            mageAttribute.SetValue(10, playerThing);

            playerThing.Stats.Add(damageStat.Name, damageStat);
            playerThing.Stats.Add(attackStat.Name, attackStat);
            var testTalent = new ChampionTalent();
            playerThing.Behaviors.Add(testBehavior);
            var behavior = playerThing.FindBehavior<TalentsBehavior>();
            testBehavior.AddTalent(testTalent);
            Assert.IsTrue(behavior.ManagedTalents.Contains(testTalent));
            Assert.IsNotNull(testTalent.PlayerThing);

            behavior.RemoveTalent(testTalent);
            playerThing.Behaviors.Remove(testBehavior);
        }
    }
}
