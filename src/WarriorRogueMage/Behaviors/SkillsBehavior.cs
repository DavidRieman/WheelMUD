//-----------------------------------------------------------------------------
// <copyright file="SkillsBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Behavior that deals with a player's skills.
//   Author: Fastalanasa
//   Date: May 12, 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage.Behaviors
{
    using System.Collections.Generic;
    using System.Linq;
    using WarriorRogueMage.Skills;
    using WheelMUD.Core;

    /// <summary>A behavior housing player skills functionality.</summary>
    public class SkillsBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the SkillsBehavior class.</summary>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public SkillsBehavior(Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ManagedSkills = new List<WRMSkill>();
        }

        /// <summary>Gets or sets the list of active skills.</summary>
        /// <value>The active skills.</value>
        public List<WRMSkill> ManagedSkills { get; set; }

        /// <summary>Called when a parent has just been assigned to this behavior. (Refer to this.Parent)</summary>
        public override void OnAddBehavior()
        {
            // When adding this behavior to a Thing, register relevant movement events so we can cancel
            // the movement of anything through our parent Thing while our parent Thing is "closed".
            var parent = this.Parent;
            if (parent != null)
            {
                foreach (var managedSkill in this.ManagedSkills)
                {
                    if (managedSkill.PlayerThing == null)
                    {
                        managedSkill.PlayerThing = this.Parent;
                    }
                }
            }

            base.OnAddBehavior();
        }

        /// <summary>Called when the current parent of this behavior is about to be removed. (Refer to this.Parent)</summary>
        public override void OnRemoveBehavior()
        {
            var parent = this.Parent;
            if (parent != null)
            {
                foreach (var managedSkill in this.ManagedSkills)
                {
                    managedSkill.PlayerThing = null;
                }
            }

            base.OnRemoveBehavior();
        }

        /// <summary>Find the first instance of a skill of the specified type.</summary>
        /// <typeparam name="U">The type of skill to locate.</typeparam>
        /// <returns>The first managed skill of the specified type, if found, else null.</returns>
        public U FindFirst<U>() where U : WRMSkill
        {
            return this.ManagedSkills.OfType<U>().FirstOrDefault();
        }

        /// <summary>Add a new skill to the list of managed talents.</summary>
        /// <param name="newTalent">The new skill to add.</param>
        public void Add(WRMSkill newTalent)
        {
            lock (this.ManagedSkills)
            {
                if (!this.ManagedSkills.Contains(newTalent))
                {
                    this.ManagedSkills.Add(newTalent);
                    newTalent.PlayerThing = this.Parent;
                    newTalent.OnAddSkill();
                }
            }
        }

        /// <summary>Removes the specified skill.</summary>
        /// <param name="talent">The skill to remove.</param>
        public void Remove(WRMSkill talent)
        {
            lock (this.ManagedSkills)
            {
                if (this.ManagedSkills.Contains(talent))
                {
                    this.ManagedSkills.Remove(talent);
                    talent.OnRemoveSkill();
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