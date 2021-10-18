//-----------------------------------------------------------------------------
// <copyright file="TestSkillsBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Behaviors
{
    using NUnit.Framework;
    using WarriorRogueMage.Behaviors;
    using WarriorRogueMage.Skills;
    using WheelMUD.Core;

    /// <summary>Tests the WRM SkillsBehavior.</summary>
    [TestFixture]
    public class TestSkillsBehavior
    {
        /// <summary>Common actors in the test.</summary>
        private Thing playerThing;

        [SetUp]
        public void Init()
        {
            playerThing = new Thing() { Name = "PlayerThing", Id = TestThingID.Generate("testthing") };
        }

        /// <summary>Test to make sure WRM behaviors are attaching properly.</summary>
        [Test]
        public void AttachSkillsBehaviorToPlayerTest()
        {
            var skillsBehavior = new SkillsBehavior(null);
            playerThing.Behaviors.Add(skillsBehavior);
            Assert.IsNotNull(playerThing.FindBehavior<SkillsBehavior>());
            playerThing.Behaviors.Remove(skillsBehavior);
        }

        [Test]
        public void AddSkillBeforeBehaviorParentSetTest()
        {
            var skillsBehavior = new SkillsBehavior(null);
            var testSkill = new SkillUnarmed();
            skillsBehavior.Add(testSkill);
            playerThing.Behaviors.Add(skillsBehavior);

            var behavior = playerThing.FindBehavior<SkillsBehavior>();
            Assert.IsTrue(behavior.ManagedSkills.Contains(testSkill));
            Assert.IsNotNull(testSkill.PlayerThing);

            behavior.Remove(testSkill);
            playerThing.Behaviors.Remove(skillsBehavior);
        }

        [Test]
        public void AddBehaviorBeforeSkillParentSetTest()
        {
            var skillsBehavior = new SkillsBehavior(null);
            var testSkill = new SkillUnarmed();
            playerThing.Behaviors.Add(skillsBehavior);
            skillsBehavior.Add(testSkill);

            var behavior = playerThing.FindBehavior<SkillsBehavior>();
            Assert.IsTrue(playerThing.FindBehavior<SkillsBehavior>().ManagedSkills.Contains(testSkill));
            Assert.IsNotNull(testSkill.PlayerThing);

            behavior.Remove(testSkill);
            playerThing.Behaviors.Remove(skillsBehavior);
        }
    }
}
