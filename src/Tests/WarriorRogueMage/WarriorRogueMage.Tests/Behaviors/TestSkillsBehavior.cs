//-----------------------------------------------------------------------------
// <copyright file="TestSkillsBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 1/30/2012 11:01:38 PM
//   Purpose   : Testing the WRM SkillsBehavior
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Behaviors
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NUnit.Framework;
    using WarriorRogueMage.Behaviors;
    using WarriorRogueMage.Skills;
    using WheelMUD.Core;

    /// <summary>Replace this with this class summary.</summary>
    [TestClass][TestFixture]
    public class TestSkillsBehavior
    {
        /// <summary>Common actors in the test.</summary>
        private Thing playerThing;

        [TestInitialize]
        [SetUp]
        public void Init()
        {
            this.playerThing = new Thing() { Name = "PlayerThing", ID = TestThingID.Generate("testthing") };
        }

        /// <summary>Test to make sure WRM behaviors are attaching properly.</summary>
        [TestMethod]
        [Test]
        public void AttachSkillsBehaviorToPlayerTest()
        {
            var skillsBehavior = new SkillsBehavior(null);

            playerThing.Behaviors.Add(skillsBehavior);

            Verify.IsNotNull(playerThing.Behaviors.FindFirst<SkillsBehavior>());

            playerThing.Behaviors.Remove(skillsBehavior);
        }

        [TestMethod]
        [Test]
        public void AddSkillBeforeBehaviorParentSetTest()
        {
            var skillsBehavior = new SkillsBehavior(null);
            var testSkill = new SkillUnarmed();

            skillsBehavior.Add(testSkill);

            playerThing.Behaviors.Add(skillsBehavior);

            var behavior = playerThing.Behaviors.FindFirst<SkillsBehavior>();

            Verify.IsTrue(playerThing.Behaviors.FindFirst<SkillsBehavior>().ManagedSkills.Contains(testSkill));
            Verify.IsNotNull(testSkill.PlayerThing);

            behavior.Remove(testSkill);

            playerThing.Behaviors.Remove(skillsBehavior);
        }

        [TestMethod]
        [Test]
        public void AddBehaviorBeforeSkillParentSetTest()
        {
            var skillsBehavior = new SkillsBehavior(null);
            var testSkill = new SkillUnarmed();

            playerThing.Behaviors.Add(skillsBehavior);

            skillsBehavior.Add(testSkill);

            var behavior = playerThing.Behaviors.FindFirst<SkillsBehavior>();

            Verify.IsTrue(playerThing.Behaviors.FindFirst<SkillsBehavior>().ManagedSkills.Contains(testSkill));
            Verify.IsNotNull(testSkill.PlayerThing);

            behavior.Remove(testSkill);

            playerThing.Behaviors.Remove(skillsBehavior);
        }
    }
}
