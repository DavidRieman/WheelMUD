//-----------------------------------------------------------------------------
// <copyright file="TestWrmCombat.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: January 2012 by Fastalanasa.
//   Edited by Duane King as needed.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.WRMCombat
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NUnit.Framework;

    using WarriorRogueMage;
    using WarriorRogueMage.Attributes;
    using WarriorRogueMage.Behaviors;
    using WarriorRogueMage.Skills;
    using WarriorRogueMage.Stats;

    using WheelMUD.Core;

    /// <summary>This is a test class for WRMCombatTest and is intended to contain all WRMCombatTest Unit Tests.</summary>
    [TestClass]
    [TestFixture]
    public class TestWrmCombat
    {
        /// <summary>Common actors in the test.</summary>
        private Thing playerThing;

        /// <summary>Initializes data for this test class.</summary>
        [TestInitialize]
        [SetUp]
        public void Init()
        {
            this.playerThing = new Thing() { Name = "PlayerThing", ID = TestThingID.Generate("testthing") };

            var testBehavior = new TalentsBehavior(null);
            var warriorAttribute = new WarriorAttribute();
            var rogueAttribute = new RogueAttribute();
            var mageAttribute = new MageAttribute();
            var damageStat = new DamageStat();
            var attackStat = new AttackStat();
            var initiativeStat = new InitiativeStat();
            var awarenessSkill = new SkillAwareness();

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
            this.playerThing.Stats.Add(initiativeStat.Name, initiativeStat);

            this.playerThing.Skills.Add(awarenessSkill.Name, awarenessSkill);

            this.playerThing.Behaviors.Add(testBehavior);
        }

        /// <summary>A test for checking for the awareness skill using an instance.</summary>
        [TestMethod]
        [Test]
        public void CheckAwarenessByInstanceTest()
        {
            GameSkill awareness = this.playerThing.FindGameSkill<SkillAwareness>();

            Verify.IsNotNull(awareness);
        }

        /// <summary>A test for checking for the awareness skill using the skill name.</summary>
        [TestMethod]
        [Test]
        public void CheckAwarenessByNameTest()
        {
            GameSkill awareness = this.playerThing.FindGameSkill("Awareness");

            Verify.IsNotNull(awareness);
        }

        /// <summary>A test for CombatSession.</summary>
        [TestMethod]
        [Test]
        public void CombatSessionTest()
        {
            var expected = new GameCombatSession();

            expected.AddCombatant(ref this.playerThing);

            Verify.AreEqual(1, expected.Combatants.Count);
        }

        /// <summary>A test for DoInititiveRoll.</summary>
        [TestMethod]
        [Test]
        public void DoInititiveRollTest()
        {
            Die combatDie = DiceService.Instance.GetDie(6);

            int roll = combatDie.Roll();

            Verify.AreNotSame(roll, 0);
        }


        /// <summary>A test for Instance.</summary>
        [TestMethod]
        [Test]
        public void InstanceTest()
        {
            var actual = WrmCombat.Instance;
            Verify.IsNotNull(actual);
        }
    }
}