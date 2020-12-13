//-----------------------------------------------------------------------------
// <copyright file="TestSkillsBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   Testing the WRM SkillsBehavior
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Behaviors
{
    using NUnit.Framework;
    using WarriorRogueMage.Behaviors;
    using WarriorRogueMage.Skills;
    using WheelMUD.Core;

    /// <summary>Replace this with this class summary.</summary>
    [TestFixture]
    public class TestSkillsBehavior
    {
        /// <summary>Common actors in the test.</summary>
        private Thing playerThing;

        [SetUp]
        public void Init()
        {
            this.playerThing = new Thing() { Name = "PlayerThing", Id = TestThingID.Generate("testthing") };
        }

        /// <summary>Test to make sure WRM behaviors are attaching properly.</summary>
        [Test]
        public void AttachSkillsBehaviorToPlayerTest()
        {
            var skillsBehavior = new SkillsBehavior(null);
            this.playerThing.Behaviors.Add(skillsBehavior);
            Assert.IsNotNull(this.playerThing.Behaviors.FindFirst<SkillsBehavior>());
            this.playerThing.Behaviors.Remove(skillsBehavior);
        }

        [Test]
        public void AddSkillBeforeBehaviorParentSetTest()
        {
            var skillsBehavior = new SkillsBehavior(null);
            var testSkill = new SkillUnarmed();
            skillsBehavior.Add(testSkill);
            this.playerThing.Behaviors.Add(skillsBehavior);

            var behavior = this.playerThing.Behaviors.FindFirst<SkillsBehavior>();
            Assert.IsTrue(behavior.ManagedSkills.Contains(testSkill));
            Assert.IsNotNull(testSkill.PlayerThing);

            behavior.Remove(testSkill);
            this.playerThing.Behaviors.Remove(skillsBehavior);
        }

        [Test]
        public void AddBehaviorBeforeSkillParentSetTest()
        {
            var skillsBehavior = new SkillsBehavior(null);
            var testSkill = new SkillUnarmed();
            this.playerThing.Behaviors.Add(skillsBehavior);
            skillsBehavior.Add(testSkill);

            var behavior = this.playerThing.Behaviors.FindFirst<SkillsBehavior>();
            Assert.IsTrue(this.playerThing.Behaviors.FindFirst<SkillsBehavior>().ManagedSkills.Contains(testSkill));
            Assert.IsNotNull(testSkill.PlayerThing);

            behavior.Remove(testSkill);
            this.playerThing.Behaviors.Remove(skillsBehavior);
        }
    }
}
