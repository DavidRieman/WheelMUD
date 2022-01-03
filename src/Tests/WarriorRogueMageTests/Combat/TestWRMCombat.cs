//-----------------------------------------------------------------------------
// <copyright file="TestWrmCombat.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WarriorRogueMage;
using WarriorRogueMage.Attributes;
using WarriorRogueMage.Behaviors;
using WarriorRogueMage.Skills;
using WarriorRogueMage.Stats;
using WheelMUD.Core;

namespace WheelMUD.Tests.WRMCombat
{
    /// <summary>Test class for unit and integration tests of WRM combat situations.</summary>
    [TestClass]
    public class TestWrmCombat
    {
        /// <summary>Common actors in the test.</summary>
        private Thing playerThing;

        /// <summary>Initializes data for this test class.</summary>
        [TestInitialize]
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
        [TestMethod]
        public void CheckAwarenessByInstanceTest()
        {
            // TODO: Repair WRMAttribute/WRMSkill/etc. (Do not place them on Thing; they should live on a Behavior that
            //       is only attached to game entities that actually need to work with them, like players/mobiles.)
            //GameSkill awareness = playerThing.FindGameSkill<SkillAwareness>();
            //Assert.IsNotNull(awareness);
        }

        /// <summary>A test for checking for the awareness skill using the skill name.</summary>
        [TestMethod]
        public void CheckAwarenessByNameTest()
        {
            // TODO: Repair WRMAttribute/WRMSkill/etc. (Do not place them on Thing; they should live on a Behavior that
            //       is only attached to game entities that actually need to work with them, like players/mobiles.)
            //GameSkill awareness = playerThing.FindGameSkill("Awareness");
            //Assert.IsNotNull(awareness);
        }

        /// <summary>A test for CombatSession.</summary>
        [TestMethod]
        public void CombatSessionTest()
        {
            var expected = new GameCombatSession();
            expected.AddCombatant(ref playerThing);
            Assert.AreEqual(1, expected.Combatants.Count);
        }

        /// <summary>A test for DoInititiveRoll.</summary>
        [TestMethod]
        public void DoInititiveRollTest()
        {
            Die combatDie = DiceService.Instance.GetDie(6);
            int roll = combatDie.Roll();
            Assert.AreNotSame(roll, 0);
        }

        /// <summary>A test for Instance.</summary>
        [TestMethod]
        public void InstanceTest()
        {
            var actual = WrmCombat.Instance;
            Assert.IsNotNull(actual);
        }
    }
}