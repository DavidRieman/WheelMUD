//-----------------------------------------------------------------------------
// <copyright file="TestPreciseShotTalent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   Tests the TalentPreciseShot class.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Talents
{
    using NUnit.Framework;
    using WarriorRogueMage;
    using WarriorRogueMage.Attributes;
    using WarriorRogueMage.Behaviors;
    using WarriorRogueMage.Stats;
    using WheelMUD.Core;

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

            this.playerThing = new Thing() { Name = "PlayerThing", Id = TestThingID.Generate("testthing") };

            this.playerThing.Behaviors.Add(testBehavior);

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
        }

        /// <summary>Tests the precise shot talent added mechanism.</summary>
        [Test]
        public void TestPreciseShotTalentAddedMechanism()
        {
            var preciseShot = new PreciseShotTalent();
            this.playerThing.Behaviors.FindFirst<TalentsBehavior>().AddTalent(preciseShot);
            var behavior = this.playerThing.Behaviors.FindFirst<TalentsBehavior>();
            Assert.IsTrue(behavior.ManagedTalents.Contains(preciseShot));
            Assert.IsNotNull(behavior.FindFirst<PreciseShotTalent>().PlayerThing);
            behavior.RemoveTalent(preciseShot);
        }

        /// <summary>Tests the precise shot talent auto set rule.</summary>
        [Test]
        public void TestPreciseShotTalentAutosetRule()
        {
            var preciseShot = new PreciseShotTalent();
            var behavior = this.playerThing.Behaviors.FindFirst<TalentsBehavior>();
            var damageStat = this.playerThing.FindGameStat("Damage");
            int oldDamaveValue = damageStat.Value;
            behavior.AddTalent(preciseShot);
            Assert.AreNotEqual(oldDamaveValue, damageStat.Value);
            behavior.RemoveTalent(preciseShot);
            Assert.AreEqual(oldDamaveValue, damageStat.Value);
        }
    }
}
