//-----------------------------------------------------------------------------
// <copyright file="TestPreciseShotTalent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using NUnit.Framework;
using WarriorRogueMage;
using WarriorRogueMage.Attributes;
using WarriorRogueMage.Behaviors;
using WarriorRogueMage.Stats;
using WheelMUD.Core;

namespace WheelMUD.Tests.Talents
{
    /// <summary>Tests the TalentPreciseShot class.</summary>
    [TestFixture]
    public class TestPreciseShotTalent
    {
        /// <summary>Common actors in the test.</summary>
        private Thing playerThing;

        [SetUp]
        public void Init()
        {
            var testBehavior = new TalentsBehavior(null);
            var warriorAttribute = new WarriorAttribute();
            var rogueAttribute = new RogueAttribute();
            var mageAttribute = new MageAttribute();
            var damageStat = new DamageStat();

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
        }

        /// <summary>Tests the precise shot talent added mechanism.</summary>
        [Test]
        public void TestPreciseShotTalentAddedMechanism()
        {
            var preciseShot = new PreciseShotTalent();
            playerThing.FindBehavior<TalentsBehavior>().AddTalent(preciseShot);
            var behavior = playerThing.FindBehavior<TalentsBehavior>();
            Assert.IsTrue(behavior.ManagedTalents.Contains(preciseShot));
            Assert.IsNotNull(behavior.FindFirst<PreciseShotTalent>().PlayerThing);
            behavior.RemoveTalent(preciseShot);
        }
    }
}
