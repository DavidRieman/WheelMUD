//-----------------------------------------------------------------------------
// <copyright file="TestMassiveAttackTalent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Talents
{
    using NUnit.Framework;
    using WarriorRogueMage;
    using WarriorRogueMage.Attributes;
    using WarriorRogueMage.Behaviors;
    using WarriorRogueMage.Stats;
    using WheelMUD.Core;

    /// <summary>Tests the TalentMassiveAttack class.</summary>
    [TestFixture]
    public class TestMassiveAttackTalent
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

        /// <summary>Tests the massive attack talent added mechanism.</summary>
        [Test]
        public void TestMassiveAttackTalentAddedMechanism()
        {
            var massiveAttack = new MassiveAttackTalent();
            playerThing.FindBehavior<TalentsBehavior>().AddTalent(massiveAttack);
            var behavior = playerThing.FindBehavior<TalentsBehavior>();

            Assert.IsTrue(behavior.ManagedTalents.Contains(massiveAttack));
            Assert.IsNotNull(behavior.FindFirst<MassiveAttackTalent>().PlayerThing);

            behavior.RemoveTalent(massiveAttack);
        }
    }
}
