//-----------------------------------------------------------------------------
// <copyright file="MultipleParentsBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace WheelMUD.Core
{
    /// <summary>Allows the attached Thing to have multiple parents; that is, to 'be in multiple places' at once.</summary>
    /// <remarks>Particularly useful for things like two-way exits which are accessible/interactive in both locations.</remarks>
    public class MultipleParentsBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the MultipleParentsBehavior class.</summary>
        public MultipleParentsBehavior() : base(null) { }

        /// <summary>Gets the secondary parents.</summary>
        /// <value>The secondary parents.</value>
        [JsonIgnore]
        public List<Thing> SecondaryParents { get; private set; } = new List<Thing>();

        /// <summary>Adds a new parent.</summary>
        /// <param name="newParent">The new parent.</param>
        public void AddParent(Thing newParent)
        {
            lock (lockObject)
            {
                // Tracking parents for our attached thing only makes sense if we are indeed attached to a thing.
                // (Avoid race conditions against behavior attachment by using a temporary reference to Parent).
                var thing = Parent;
                if (thing != null)
                {
                    // If the object we apply to does not have a primary Parent yet, simply Add it to set Parent.
                    if (thing.Parent == null)
                    {
                        thing.RigParentUnsafe(newParent);
                        return;
                    }
                    else if (thing.Parent == newParent)
                    {
                        return;
                    }

                    // Go through our current secondary parents, and if we don't already track this as one of the
                    // parents, then add it.
                    if (SecondaryParents.Contains(newParent))
                    {
                        return;
                    }

                    SecondaryParents.Add(newParent);
                }
            }
        }

        /// <summary>Removes the specified parent.</summary>
        /// <param name="oldParent">The old parent.</param>
        public void RemoveParent(Thing oldParent)
        {
            lock (lockObject)
            {
                // Tracking parents for our attached thing only makes sense if we are indeed attached to a thing.
                // (Avoid race conditions against behavior attachment by using a temporary reference to Parent).
                var thing = Parent;
                if (thing != null)
                {
                    // If our thing has the parent being removed as it's primary parent, we need to shift one of the
                    // remaining secondary parents to be the primary parent, removing it from the secondary parents.
                    // (If there are no remaining secondary parents, this will correctly set the Parent as null.)
                    if (thing.Parent == oldParent)
                    {
                        var newPrimaryParent = (from p in SecondaryParents where p != oldParent select p).FirstOrDefault();
                        thing.RigParentUnsafe(newPrimaryParent);
                    }

                    // Remove this oldParent from our tracked parents (if tracked).
                    SecondaryParents.Remove(oldParent);
                }
            }
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties() { }
    }
}