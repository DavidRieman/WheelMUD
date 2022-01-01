﻿//-----------------------------------------------------------------------------
// <copyright file="TestChampionTalent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WarriorRogueMage;
using WarriorRogueMage.Attributes;
using WarriorRogueMage.Behaviors;
using WarriorRogueMage.Stats;
using WheelMUD.Core;

namespace WheelMUD.Tests.Talents
{
    // <summary>Tests the Champion talent.</summary>
    [TestClass]
    public class TestChampionTalent
    {
        /// <summary>Common actors in the test.</summary>
        private Thing playerThing;

        [TestInitialize]
        public void Init()
        {
            var testBehavior = new TalentsBehavior(null);
            var warriorAttribute = new WarriorAttribute();
            var rogueAttribute = new RogueAttribute();
            var mageAttribute = new MageAttribute();
            var damageStat = new DamageStat();
            var attackStat = new AttackStat();

            playerThing = new Thing() { Name = "PlayerThing", Id = TestThingID.Generate("testthing") };

            playerThing.Behaviors.Add(testBehavior);

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
        }

        /// <summary>Tests the champion talent added mechanism.</summary>
        [TestMethod]
        public void TestChampionTalentAddedMechanism()
        {
            var champion = new ChampionTalent();

            playerThing.FindBehavior<TalentsBehavior>().AddTalent(champion);

            var behavior = playerThing.FindBehavior<TalentsBehavior>();

            Assert.IsTrue(behavior.ManagedTalents.Contains(champion));
            Assert.IsNotNull(behavior.FindFirst<ChampionTalent>().PlayerThing);

            behavior.RemoveTalent(champion);
        }
    }
}
