//-----------------------------------------------------------------------------
// <copyright file="TestMultipleParentsBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Behaviors
{
    using NUnit.Framework;
    using WheelMUD.Core;

    /// <summary>Tests for the MultipleParentsBehavior class.</summary>
    [TestFixture]
    public class TestMultipleParentsBehavior
    {
        /// <summary>The actors in the test.</summary>
        private Thing parent1, parent2, child;

        /// <summary>The basic MultipleParentsBehavior instance to test.</summary>
        private MultipleParentsBehavior multipleParentsBehavior;

        /// <summary>Common preparation for all MultipleParentsBehavior tests.</summary>
        [SetUp]
        public void Init()
        {
            // Create 2 things and a basic MultipleParentsBehavior for testing.
            parent1 = new Thing() { Name = "Thing1", Id = TestThingID.Generate("testthing") };
            parent2 = new Thing() { Name = "Thing2", Id = TestThingID.Generate("testthing") };
            child = new Thing() { Name = "Child1", Id = TestThingID.Generate("testthing") };
            multipleParentsBehavior = new MultipleParentsBehavior();
        }

        /// <summary>Test normal parenting behaviors without a MultipleParentsBehavior being attached.</summary>
        [Test]
        public void TestSingleParentingBehavior()
        {
            // Verify that a thing which has not yet been added to a parent, has none.
            Assert.IsTrue(child.Parent == null);

            // Verify that a basic thing can be added to a parent correctly.
            parent1.Add(child);
            Assert.IsTrue(parent1.Children.Contains(child));
            Assert.IsTrue(!parent2.Children.Contains(child));
            Assert.IsTrue(child.Parent == parent1);

            // Verify adding it to a second parent actually reassigns the parent.
            parent2.Add(child);
            Assert.IsTrue(parent2.Children.Contains(child));
            Assert.IsTrue(!parent1.Children.Contains(child));
            Assert.IsTrue(child.Parent == parent2);

            // Verify removing it from the last parent, cleans up the parent/child relationships.
            parent2.Remove(child);
            Assert.IsTrue(!parent1.Children.Contains(child));
            Assert.IsTrue(!parent2.Children.Contains(child));
            Assert.IsTrue(child.Parent == null);
        }

        /// <summary>Test an unattached MultipleParentsBehavior.</summary>
        [Test]
        public void TestUnattachedMultipleParentsBehavior()
        {
            // Verify that messing with a MultipleParentsBehavior while it is not attached to a host thing
            // does not throw (IE during possible deconstruction race conditions and such).
            multipleParentsBehavior.AddParent(parent1);
            multipleParentsBehavior.RemoveParent(parent1);
        }

        /// <summary>Test parenting behaviors with a MultipleParentsBehavior attached.</summary>
        [Test]
        public void TestMultipleParentingBehavior()
        {
            // Verify we can add and retrieve the MultipleParentsBehavior of a Thing.
            child.Behaviors.Add(multipleParentsBehavior);
            Assert.IsTrue(child.Behaviors.FindFirst<MultipleParentsBehavior>() == multipleParentsBehavior);

            // Verify it can now be a child of multiple parents, and one of those can be found as the primary Parent.
            parent1.Add(child);
            parent2.Add(child);
            Assert.IsTrue(parent1.Children.Contains(child));
            Assert.IsTrue(parent2.Children.Contains(child));
            Assert.IsTrue(child.Parent == parent1 || child.Parent == parent2);

            // Verify we can remove the item from a secondary parent, and still be attached well to the primary.
            parent2.Remove(child);
            Assert.IsTrue(parent1.Children.Contains(child));
            Assert.IsTrue(!parent2.Children.Contains(child));
            Assert.IsTrue(child.Parent == parent1);
            parent2.Add(child);

            // Verify we can remove the item from a primary parent, and a secondary parent becomes the primary.
            parent1.Remove(child);
            Assert.IsTrue(!parent1.Children.Contains(child));
            Assert.IsTrue(parent2.Children.Contains(child));
            Assert.IsTrue(child.Parent == parent2);
            parent1.Add(child);

            // Verify we can be attached to more than 2 parents.
            Thing parent3 = new Thing() { Name = "Thing3", Id = TestThingID.Generate("testthing") };
            parent3.Add(child);
            Assert.IsTrue(parent1.Children.Contains(child));
            Assert.IsTrue(parent2.Children.Contains(child));
            Assert.IsTrue(parent3.Children.Contains(child));
            Assert.IsTrue(child.Parent != null);
        }
    }
}