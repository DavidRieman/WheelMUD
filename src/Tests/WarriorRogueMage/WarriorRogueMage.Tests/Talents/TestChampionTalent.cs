//-----------------------------------------------------------------------------
// <copyright file="TestChampionTalent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 2/2/2012 11:27:55 PM
//   Purpose   : Tests the Champion talent.
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

    [TestClass][TestFixture]
    public class TestChampionTalent
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
            var attackStat = new AttackStat();

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
            playerThing.Stats.Add(attackStat.Name, attackStat);
        }

        /// <summary>Tests the champion talent added mechanism.</summary>
        [TestMethod]
        [Test]
        public void TestChampionTalentAddedMechanism()
        {
            var champion = new ChampionTalent();

            this.playerThing.Behaviors.FindFirst<TalentsBehavior>().AddTalent(champion);

            var behavior = this.playerThing.Behaviors.FindFirst<TalentsBehavior>();

            Verify.IsTrue(behavior.ManagedTalents.Contains(champion));
            Verify.IsNotNull(behavior.FindFirst<ChampionTalent>().PlayerThing);

            behavior.RemoveTalent(champion);
        }

        /// <summary>Tests the champion talent auto set rule.</summary>
        [TestMethod]
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

            Verify.AreNotEqual(oldAttackValue, attackStat.Value);
            Verify.AreNotEqual(oldDamageValue, damageStat.Value);

            behavior.RemoveTalent(champion);

            Verify.AreEqual(oldAttackValue, attackStat.Value);
            Verify.AreEqual(oldDamageValue, damageStat.Value);
        }
    }
}
