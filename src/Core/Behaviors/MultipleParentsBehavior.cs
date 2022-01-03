//-----------------------------------------------------------------------------
// <copyright file="MultipleParentsBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace WheelMUD.Core
{
    /// <summary>Allows the attached Thing to have multiple parents; that is, to 'be in multiple places' at once.</summary>
    /// <remarks>Particularly useful for things like 'two-way exits' which are accessible/interactable in both locations.</remarks>
    public class MultipleParentsBehavior : Behavior
    {
        private List<string> pendingParentIds = new List<string>();

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
            ID = instanceID;
        }

        /// <summary>
        /// The MultipleParentsBehavior may have parents in two separate Areas (like an exit linking two areas). In such
        /// cases, the primary Parent will determine which Area this Thing loads with. After loading, any more secondary
        /// parents may or may not have already loaded. When loading 
        /// </summary>
        /// <remarks>Being private hides this property from external usage, but it will still get exercised by persistence.</remarks>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Persistence (see remarks).")]
        private string[] SecondaryParentIds
        {
            get
            {
                lock (lockObject)
                {
                    // Even if haven't found the concrete secondary parent when we are persisting, we still need to
                    // keep those pending parent links as well so we don't accidentally erase them upon next load.
                    return SecondaryParents.Select(p => p.Id).Concat(pendingParentIds).ToArray();
                }
            }
            set
            {
                lock (lockObject)
                {
                    foreach (var id in value)
                    {
                        var thing = ThingManager.Instance.FindThing(id);
                        if (thing != null)
                        {
                            SecondaryParents.Add(thing);
                        }
                        else
                        {
                            pendingParentIds.Add(id);
                        }
                    }
                }
            }
        }

        /// <summary>Gets the secondary parents.</summary>
        /// <value>The secondary parents.</value>
        [JsonIgnore]
        public List<Thing> SecondaryParents { get; private set; }

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
        protected override void SetDefaultProperties()
        {
            lock (lockObject)
            {
                SecondaryParents = new List<Thing>();
            }
        }
    }
}