//-----------------------------------------------------------------------------
// <copyright file="MultipleParentsBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: June 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>Allows the attached Thing to have multiple parents; that is, to 'be in multiple places' at once.</summary>
    /// <remarks>Particularly useful for things like 'two-way exits' which are accessible/interactable in both locations.</remarks>
    public class MultipleParentsBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the MultipleParentsBehavior class.</summary>
        public MultipleParentsBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="MultipleParentsBehavior"/> class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public MultipleParentsBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /// <summary>Gets the secondary parents.</summary>
        /// <value>The secondary parents.</value>
        public List<Thing> SecondaryParents { get; private set; }

        /// <summary>Adds a new parent.</summary>
        /// <param name="newParent">The new parent.</param>
        public void AddParent(Thing newParent)
        {
            // Tracking parents for our attached thing only makes sense if we are indeed attached to a thing.
            // (Avoid race conditions against behavior attachment by using a temporary reference to this.Parent).
            var parent = this.Parent;
            if (parent != null)
            {
                // If the object we apply to (AKA our parent) does not have a parent yet, simply set it.
                if (parent.Parent == null)
                {
                    parent.Parent = newParent;
                    return;
                }

                // Go through our current secondary parents, and if we don't already track this as
                // a parent, then add it.
                if (this.SecondaryParents.Contains(newParent))
                {
                    return;
                }

                this.SecondaryParents.Add(newParent);
            }
        }

        /// <summary>Removes the specified parent.</summary>
        /// <param name="oldParent">The old parent.</param>
        public void RemoveParent(Thing oldParent)
        {
            // Tracking parents for our attached thing only makes sense if we are indeed attached to a thing.
            // (Avoid race conditions against behavior attachment by using a temporary reference to this.Parent).
            var parent = this.Parent;
            if (parent != null)
            {
                // If the object we apply to (AKA our parent) has the parent being removed as it's primary parent, 
                // we should promote one of the secondary parents, if any, to be the new primary parent. (If there
                // are no remaining secondary parents to promote, this correctly sets the primary parent to null.)
                if (parent.Parent == oldParent)
                {
                    parent.Parent = (from p in this.SecondaryParents where p != oldParent select p).FirstOrDefault();
                }

                // Remove this oldParent from our tracked parents (if tracked).
                this.SecondaryParents.Remove(oldParent);
            }
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            this.SecondaryParents = new List<Thing>();
        }
    }
}