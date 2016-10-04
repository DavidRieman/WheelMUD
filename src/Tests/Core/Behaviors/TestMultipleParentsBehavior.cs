//-----------------------------------------------------------------------------
// <copyright file="TestMultipleParentsBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: August 2011 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Behaviors
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NUnit.Framework;
    using WheelMUD.Core;
    
    /// <summary>Tests for the MultipleParentsBehavior class.</summary>
    [TestClass][TestFixture]
    public class TestMultipleParentsBehavior
    {
        /// <summary>The actors in the test.</summary>
        private Thing parent1, parent2, child;

        /// <summary>The basic MultipleParentsBehavior instance to test.</summary>
        private MultipleParentsBehavior multipleParentsBehavior;

        /// <summary>Common preparation for all MultipleParentsBehavior tests.</summary>
        [TestInitialize][SetUp]
        public void Init()
        {
            // Create 2 things and a basic MultipleParentsBehavior for testing.
            this.parent1 = new Thing() { Name = "Thing1", ID = TestThingID.Generate("testthing") };
            this.parent2 = new Thing() { Name = "Thing2", ID = TestThingID.Generate("testthing") };
            this.child = new Thing() { Name = "Child1", ID = TestThingID.Generate("testthing") };
            this.multipleParentsBehavior = new MultipleParentsBehavior();
        }

        /// <summary>Test normal parenting behaviors without a MultipleParentsBehavior being attached.</summary>
        [TestMethod][Test]
        public void TestSingleParentingBehavior()
        {
            // Verify that a thing which has not yet been added to a parent, has none.
            Verify.IsTrue(this.child.Parent == null);

            // Verify that a basic thing can be added to a parent correctly.
            this.parent1.Add(this.child);
            Verify.IsTrue(this.parent1.Children.Contains(this.child));
            Verify.IsTrue(!this.parent2.Children.Contains(this.child));
            Verify.IsTrue(this.child.Parent == this.parent1);

            // Verify adding it to a second parent actually reassigns the parent.
            this.parent2.Add(this.child);
            Verify.IsTrue(this.parent2.Children.Contains(this.child));
            Verify.IsTrue(!this.parent1.Children.Contains(this.child));
            Verify.IsTrue(this.child.Parent == this.parent2);

            // Verify removing it from the last parent, cleans up the parent/child relationships.
            this.parent2.Remove(this.child);
            Verify.IsTrue(!this.parent1.Children.Contains(this.child));
            Verify.IsTrue(!this.parent2.Children.Contains(this.child));
            Verify.IsTrue(this.child.Parent == null);
        }

        /// <summary>Test an unattached MultipleParentsBehavior.</summary>
        [TestMethod]
        [Test]
        public void TestUnattachedMultipleParentsBehavior()
        {
            // Verify that messing with a MultipleParentsBehavior while it is not attached to a host thing
            // does not throw (IE during possible deconstruction race conditions and such).
            this.multipleParentsBehavior.AddParent(parent1);
            this.multipleParentsBehavior.RemoveParent(parent1);
        }

        /// <summary>Test parenting behaviors with a MultipleParentsBehavior attached.</summary>
        [TestMethod][Test]
        public void TestMultipleParentingBehavior()
        {
            // Verify we can add and retrieve the MultipleParentsBehavior of a Thing.
            this.child.Behaviors.Add(this.multipleParentsBehavior);
            Verify.IsTrue(this.child.Behaviors.FindFirst<MultipleParentsBehavior>() == this.multipleParentsBehavior);

            // Verify it can now be a child of multiple parents, and one of those can be found as the primary Parent.
            this.parent1.Add(this.child);
            this.parent2.Add(this.child);
            Verify.IsTrue(this.parent1.Children.Contains(this.child));
            Verify.IsTrue(this.parent2.Children.Contains(this.child));
            Verify.IsTrue(this.child.Parent == this.parent1 || this.child.Parent == this.parent2);

            // Verify we can remove the item from a secondary parent, and still be attached well to the primary.
            this.parent2.Remove(this.child);
            Verify.IsTrue(this.parent1.Children.Contains(this.child));
            Verify.IsTrue(!this.parent2.Children.Contains(this.child));
            Verify.IsTrue(this.child.Parent == this.parent1);
            this.parent2.Add(this.child);
            
            // Verify we can remove the item from a primary parent, and a secondary parent becomes the primary.
            this.parent1.Remove(this.child);
            Verify.IsTrue(!this.parent1.Children.Contains(this.child));
            Verify.IsTrue(this.parent2.Children.Contains(this.child));
            Verify.IsTrue(this.child.Parent == this.parent2);
            this.parent1.Add(this.child);

            // Verify we can be attached to more than 2 parents.
            Thing parent3 = new Thing() { Name = "Thing3", ID = TestThingID.Generate("testthing") };
            parent3.Add(this.child);
            Verify.IsTrue(this.parent1.Children.Contains(this.child));
            Verify.IsTrue(this.parent2.Children.Contains(this.child));
            Verify.IsTrue(parent3.Children.Contains(this.child));
            Verify.IsTrue(this.child.Parent != null);
        }
    }
}