//-----------------------------------------------------------------------------
// <copyright file="TestMassiveAttackTalent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 2/12/2012
//   Purpose   : Tests the TalentMassiveAttack class.
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

    /// <summary>Tests the TalentMassiveAttack class.</summary>
    [TestClass]
    [TestFixture]
    public class TestMassiveAttackTalent
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

        /// <summary>Tests the massive attack talent added mechanism.</summary>
        [TestMethod]
        [Test]
        public void TestMassiveAttackTalentAddedMechanism()
        {
            var massiveAttack = new MassiveAttackTalent();

            this.playerThing.Behaviors.FindFirst<TalentsBehavior>().AddTalent(massiveAttack);

            var behavior = this.playerThing.Behaviors.FindFirst<TalentsBehavior>();

            Verify.IsTrue(behavior.ManagedTalents.Contains(massiveAttack));
            Verify.IsNotNull(behavior.FindFirst<MassiveAttackTalent>().PlayerThing);

            behavior.RemoveTalent(massiveAttack);
        }

        /// <summary>Tests the massive attack talent auto set rule.</summary>
        [TestMethod]
        [Test]
        public void TestMassiveAttackTalentAutosetRule()
        {
            var massiveAttack = new MassiveAttackTalent();

            var behavior = this.playerThing.Behaviors.FindFirst<TalentsBehavior>();

            var damageStat = this.playerThing.FindGameStat("Damage");
            int oldDamaveValue = damageStat.Value;

            behavior.AddTalent(massiveAttack);

            Verify.AreNotEqual(oldDamaveValue, damageStat.Value);

            behavior.RemoveTalent(massiveAttack);

            Verify.AreEqual(oldDamaveValue, damageStat.Value);
        }
    }
}
