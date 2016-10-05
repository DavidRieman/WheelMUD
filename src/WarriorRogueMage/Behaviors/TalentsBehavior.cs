//-----------------------------------------------------------------------------
// <copyright file="TalentsBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Behavior that deals with a player's talents.
//   Author: Fastalanasa
//   Date: May 12, 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage.Behaviors
{
    using System.Collections.Generic;
    using System.Linq;
    using WheelMUD.Core;

    /// <summary>A behavior to house talents-related functionality.</summary>
    public class TalentsBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the TalentsBehavior class.</summary>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public TalentsBehavior(Dictionary<string, object> instanceProperties) : base(instanceProperties)
        {
            this.ManagedTalents = new List<Talent>();
        }

        /// <summary>Gets or sets the list of active talents.</summary>
        public List<Talent> ManagedTalents { get; set; }

        /// <summary>Called when a parent has just been assigned to this behavior. (Refer to this.Parent)</summary>
        public override void OnAddBehavior()
        {
            // When adding this behavior to a Thing, register relevant movement events so we can cancel
            // the movement of anything through our parent Thing while our parent Thing is "closed".
            var parent = this.Parent;
            if (parent != null)
            {
                // This is to handle the case when talents have been added to the behavior
                // before it being added to a thing.
                foreach (var managedTalent in this.ManagedTalents)
                {
                    if (managedTalent.PlayerThing == null)
                    {
                        managedTalent.PlayerThing = this.Parent;
                    }
                }
            }

            base.OnAddBehavior();
        }

        /// <summary>Called when the current parent of this behavior is about to be removed. (Refer to this.Parent.)</summary>
        public override void OnRemoveBehavior()
        {
            var parent = this.Parent;
            if (parent != null)
            {
                // Make sure to "unhook" all children talents from the
                // player Thing object. Otherwise we could have a memory leak,
                // and/or weird errors.
                foreach (var managedTalent in this.ManagedTalents)
                {
                    managedTalent.PlayerThing = null;
                }
            }

            base.OnRemoveBehavior();
        }

        /// <summary>Find the first instance of a talent of the specified type.</summary>
        /// <typeparam name="U">The type of talent to locate.</typeparam>
        /// <returns>The first managed talent of the specified type, if found, else null.</returns>
        public U FindFirst<U>() where U : Talent
        {
            return this.ManagedTalents.OfType<U>().FirstOrDefault();
        }

        /// <summary>Add a new talent to the list of managed talents.</summary>
        /// <param name="newTalent">The new talent to add.</param>
        public void AddTalent(Talent newTalent)
        {
            lock (this.ManagedTalents)
            {
                if (!this.ManagedTalents.Contains(newTalent))
                {
                    this.ManagedTalents.Add(newTalent);
                    newTalent.PlayerThing = this.Parent;
                    newTalent.OnAddTalent();
                }
            }
        }

        /// <summary>Removes the specified talent.</summary>
        /// <param name="talent">The talent to remove.</param>
        public void RemoveTalent(Talent talent)
        {
            lock (this.ManagedTalents)
            {
                if (this.ManagedTalents.Contains(talent))
                {
                    this.ManagedTalents.Remove(talent);
                    talent.OnRemoveTalent();
                    talent.PlayerThing = null;
                }
            }
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            // Nothing to do here.
        }
    }
}