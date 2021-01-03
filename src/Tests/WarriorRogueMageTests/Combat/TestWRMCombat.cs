//-----------------------------------------------------------------------------
// <copyright file="TestWrmCombat.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.WRMCombat
{
    using NUnit.Framework;
    using WarriorRogueMage;
    using WarriorRogueMage.Attributes;
    using WarriorRogueMage.Behaviors;
    using WarriorRogueMage.Skills;
    using WarriorRogueMage.Stats;
    using WheelMUD.Core;

    /// <summary>Test class for unit and integration tests of WRM combat situations.</summary>
    [TestFixture]
    public class TestWrmCombat
    {
        /// <summary>Common actors in the test.</summary>
        private Thing playerThing;

        /// <summary>Initializes data for this test class.</summary>
        [SetUp]
        public void Init()
        {
            playerThing = new Thing() { Name = "PlayerThing", Id = TestThingID.Generate("testthing") };

            var testBehavior = new TalentsBehavior(null);
            var warriorAttribute = new WarriorAttribute();
            var rogueAttribute = new RogueAttribute();
            var mageAttribute = new MageAttribute();
            var damageStat = new DamageStat();
            var attackStat = new AttackStat();
            var initiativeStat = new InitiativeStat();
            var awarenessSkill = new SkillAwareness();

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
            playerThing.Stats.Add(initiativeStat.Name, initiativeStat);
            playerThing.Skills.Add(awarenessSkill.Name, awarenessSkill);
            playerThing.Behaviors.Add(testBehavior);
        }

        /// <summary>A test for checking for the awareness skill using an instance.</summary>
        [Test]
        public void CheckAwarenessByInstanceTest()
        {
            GameSkill awareness = playerThing.FindGameSkill<SkillAwareness>();
            Assert.IsNotNull(awareness);
        }

        /// <summary>A test for checking for the awareness skill using the skill name.</summary>
        [Test]
        public void CheckAwarenessByNameTest()
        {
            GameSkill awareness = playerThing.FindGameSkill("Awareness");
            Assert.IsNotNull(awareness);
        }

        /// <summary>A test for CombatSession.</summary>
        [Test]
        public void CombatSessionTest()
        {
            var expected = new GameCombatSession();
            expected.AddCombatant(ref playerThing);
            Assert.AreEqual(1, expected.Combatants.Count);
        }

        /// <summary>A test for DoInititiveRoll.</summary>
        [Test]
        public void DoInititiveRollTest()
        {
            Die combatDie = DiceService.Instance.GetDie(6);
            int roll = combatDie.Roll();
            Assert.AreNotSame(roll, 0);
        }

        /// <summary>A test for Instance.</summary>
        [Test]
        public void InstanceTest()
        {
            var actual = WrmCombat.Instance;
            Assert.IsNotNull(actual);
        }
    }
}