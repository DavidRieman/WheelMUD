//-----------------------------------------------------------------------------
// <copyright file="TestChannelerTalent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   Tests the TalentChanneler class.
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

    /// <summary>Tests the TalentChanneler class.</summary>
    [TestFixture]
    public class TestChannelerTalent
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

        /// <summary>Tests the channeler talent added mechanism.</summary>
        [Test]
        public void TestChannelerTalentAddedMechanism()
        {
            var channeler = new ChannelerTalent();

            this.playerThing.Behaviors.FindFirst<TalentsBehavior>().AddTalent(channeler);

            var behavior = this.playerThing.Behaviors.FindFirst<TalentsBehavior>();

            Assert.IsTrue(behavior.ManagedTalents.Contains(channeler));
            Assert.IsNotNull(behavior.FindFirst<ChannelerTalent>().PlayerThing);

            behavior.RemoveTalent(channeler);
        }

        /// <summary>Tests the channeler talent auto set rule.</summary>
        [Test]
        public void TestChannelerTalentAutosetRule()
        {
            var channeler = new ChannelerTalent();

            var behavior = this.playerThing.Behaviors.FindFirst<TalentsBehavior>();
            var damageStat = this.playerThing.FindGameStat("Damage");
            int oldDamaveValue = damageStat.Value;

            behavior.AddTalent(channeler);

            Assert.AreNotEqual(oldDamaveValue, damageStat.Value);

            behavior.RemoveTalent(channeler);

            Assert.AreEqual(oldDamaveValue, damageStat.Value);
        }
    }
}
