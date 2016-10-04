//-----------------------------------------------------------------------------
// <copyright file="TestPreciseShotTalent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 2/12/2012
//   Purpose   : Tests the TalentPreciseShot class.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Talents
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NUnit.Framework;
    using WarriorRogueMage;
    using WarriorRogueMage.Attributes;
    using WarriorRogueMage.Behaviors;
    using WarriorRogueMage.Stats;
    using WheelMUD.Core;

    /// <summary>Tests the TalentPreciseShot class.</summary>
    [TestClass()]
    [TestFixture]
    public class TestPreciseShotTalent
    {
        /// <summary>Common actors in the test.</summary>
        private Thing playerThing;

        [TestInitialize]
        [SetUp]
        public void Init()
        {
            var testBehavior = new TalentsBehavior(null);
            var warriorAttribute = new WarriorAttribute();
            var rogueAttribute = new RogueAttribute();
            var mageAttribute = new MageAttribute();
            var damageStat = new DamageStat();

            this.playerThing = new Thing() { Name = "PlayerThing", ID = TestThingID.Generate("testthing") };

            playerThing.Behaviors.Add(testBehavior);

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
        }

        /// <summary>Tests the precise shot talent added mechanism.</summary>
        [TestMethod]
        [Test]
        public void TestPreciseShotTalentAddedMechanism()
        {
            var preciseShot = new PreciseShotTalent();

            this.playerThing.Behaviors.FindFirst<TalentsBehavior>().AddTalent(preciseShot);

            var behavior = this.playerThing.Behaviors.FindFirst<TalentsBehavior>();

            Verify.IsTrue(behavior.ManagedTalents.Contains(preciseShot));
            Verify.IsNotNull(behavior.FindFirst<PreciseShotTalent>().PlayerThing);

            behavior.RemoveTalent(preciseShot);
        }

        /// <summary>Tests the precise shot talent auto set rule.</summary>
        [TestMethod]
        [Test]
        public void TestPreciseShotTalentAutosetRule()
        {
            var preciseShot = new PreciseShotTalent();

            var behavior = this.playerThing.Behaviors.FindFirst<TalentsBehavior>();

            var damageStat = this.playerThing.FindGameStat("Damage");
            int oldDamaveValue = damageStat.Value;

            behavior.AddTalent(preciseShot);

            Verify.AreNotEqual(oldDamaveValue, damageStat.Value);

            behavior.RemoveTalent(preciseShot);

            Verify.AreEqual(oldDamaveValue, damageStat.Value);
        }
    }
}
