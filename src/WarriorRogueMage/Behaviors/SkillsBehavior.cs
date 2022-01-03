//-----------------------------------------------------------------------------
// <copyright file="SkillsBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using WarriorRogueMage.Skills;
using WheelMUD.Core;

namespace WarriorRogueMage.Behaviors
{
    /// <summary>A behavior housing player skills functionality.</summary>
    public class SkillsBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the SkillsBehavior class.</summary>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public SkillsBehavior(Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            ManagedSkills = new List<WRMSkill>();
        }

        /// <summary>Gets or sets the list of active skills.</summary>
        /// <value>The active skills.</value>
        public List<WRMSkill> ManagedSkills { get; set; }

        /// <summary>Called when a parent has just been assigned to this behavior. (Refer to Parent)</summary>
        protected override void OnAddBehavior()
        {
            // When adding this behavior to a Thing, register relevant movement events so we can cancel
            // the movement of anything through our parent Thing while our parent Thing is "closed".
            var parent = Parent;
            if (parent != null)
            {
                foreach (var managedSkill in ManagedSkills)
                {
                    if (managedSkill.PlayerThing == null)
                    {
                        managedSkill.PlayerThing = Parent;
                    }
                }
            }

            base.OnAddBehavior();
        }

        /// <summary>Called when the current parent of this behavior is about to be removed. (Refer to Parent)</summary>
        protected override void OnRemoveBehavior()
        {
            var parent = Parent;
            if (parent != null)
            {
                foreach (var managedSkill in ManagedSkills)
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
            return ManagedSkills.OfType<U>().FirstOrDefault();
        }

        /// <summary>Add a new skill to the list of managed talents.</summary>
        /// <param name="newTalent">The new skill to add.</param>
        public void Add(WRMSkill newTalent)
        {
            lock (ManagedSkills)
            {
                if (!ManagedSkills.Contains(newTalent))
                {
                    ManagedSkills.Add(newTalent);
                    newTalent.PlayerThing = Parent;
                    newTalent.OnAddSkill();
                }
            }
        }

        /// <summary>Removes the specified skill.</summary>
        /// <param name="talent">The skill to remove.</param>
        public void Remove(WRMSkill talent)
        {
            lock (ManagedSkills)
            {
                if (ManagedSkills.Contains(talent))
                {
                    ManagedSkills.Remove(talent);
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