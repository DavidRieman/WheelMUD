//-----------------------------------------------------------------------------
// <copyright file="TestTalentsBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 1/30/2012 10:37:31 PM
//   Purpose   : Testing the TalentsBehavior class.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Behaviors
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NUnit.Framework;

    using WarriorRogueMage;
    using WarriorRogueMage.Attributes;
    using WarriorRogueMage.Behaviors;
    using WarriorRogueMage.Stats;

    using WheelMUD.Core;

    [TestClass()][TestFixture]
    public class TestTalentsBehavior
    {
        /// <summary>Common actors in the test.</summary>
        private Thing playerThing;

        [TestInitialize]
        [SetUp]
        public void Init()
        {
            this.playerThing = new Thing() { Name = "PlayerThing", ID = TestThingID.Generate("testthing") };
        }

        /// <summary>Test to make sure WRM behaviors are attaching properly.</summary>
        [TestMethod]
        [Test]
        public void AttachTalentsBehaviorToPlayerTest()
        {
            var testBehavior = new TalentsBehavior(null);

            playerThing.Behaviors.Add(testBehavior);

            Verify.IsNotNull(playerThing.Behaviors.FindFirst<TalentsBehavior>());

            playerThing.Behaviors.Remove(testBehavior);
        } 

        [TestMethod]
        [Test]
        public void AddTalentBeforeBehaviorParentSetTest()
        {
            var testTalent = new CraftsmanTalent();
            var testBehavior = new TalentsBehavior(null);

            testBehavior.AddTalent(testTalent);

            playerThing.Behaviors.Add(testBehavior);

            var behavior = playerThing.Behaviors.FindFirst<TalentsBehavior>();

            Verify.IsTrue(behavior.ManagedTalents.Contains(testTalent)); 
            Verify.IsNotNull(testTalent.PlayerThing);

            behavior.RemoveTalent(testTalent);

            playerThing.Behaviors.Remove(testBehavior);
        }

        [TestMethod]
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
            playerThing.AddAttribute(warriorAttribute);

            mageAttribute.Parent = this.playerThing;
            playerThing.AddAttribute(rogueAttribute);

            rogueAttribute.Parent = this.playerThing;
            playerThing.AddAttribute(mageAttribute);

            warriorAttribute.SetValue(10, playerThing);
            rogueAttribute.SetValue(10, playerThing);
            mageAttribute.SetValue(10, playerThing);

            playerThing.Stats.Add(damageStat.Name, damageStat);
            playerThing.Stats.Add(attackStat.Name, attackStat);

            var testTalent = new ChampionTalent();

            playerThing.Behaviors.Add(testBehavior);

            var behavior = playerThing.Behaviors.FindFirst<TalentsBehavior>();

            behavior.AddTalent(testTalent);

            Verify.IsTrue(behavior.ManagedTalents.Contains(testTalent));
            Verify.IsNotNull(testTalent.PlayerThing);

            behavior.RemoveTalent(testTalent);

            playerThing.Behaviors.Remove(testBehavior);
        }

        [TestMethod]
        [Test]
        public void AddBehaviorBeforeTalentParentSetTest()
        {
            var testTalent = new CraftsmanTalent();
            var testBehavior = new TalentsBehavior(null);

            playerThing.Behaviors.Add(testBehavior);

            var behavior = playerThing.Behaviors.FindFirst<TalentsBehavior>();

            testBehavior.AddTalent(testTalent);

            Verify.IsTrue(behavior.ManagedTalents.Contains(testTalent));
            Verify.IsNotNull(testTalent.PlayerThing);

            behavior.RemoveTalent(testTalent);

            playerThing.Behaviors.Remove(testBehavior);
        }

        [TestMethod]
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
            playerThing.AddAttribute(warriorAttribute);

            mageAttribute.Parent = this.playerThing;
            playerThing.AddAttribute(rogueAttribute);

            rogueAttribute.Parent = this.playerThing;
            playerThing.AddAttribute(mageAttribute);

            warriorAttribute.SetValue(10, playerThing);
            rogueAttribute.SetValue(10, playerThing);
            mageAttribute.SetValue(10, playerThing);

            playerThing.Stats.Add(damageStat.Name, damageStat);
            playerThing.Stats.Add(attackStat.Name, attackStat);

            var testTalent = new ChampionTalent();

            playerThing.Behaviors.Add(testBehavior);

            var behavior = playerThing.Behaviors.FindFirst<TalentsBehavior>();

            testBehavior.AddTalent(testTalent);

            Verify.IsTrue(behavior.ManagedTalents.Contains(testTalent));
            Verify.IsNotNull(testTalent.PlayerThing);

            behavior.RemoveTalent(testTalent);

            playerThing.Behaviors.Remove(testBehavior);
        }
    }
}
