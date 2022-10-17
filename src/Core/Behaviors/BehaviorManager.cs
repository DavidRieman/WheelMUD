﻿//-----------------------------------------------------------------------------
// <copyright file="BehaviorManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WheelMUD.Core
{
    /// <summary>A manager for behaviors.</summary>
    public class BehaviorManager : ICloneable
    {
        /// <summary>Initializes a new instance of the <see cref="BehaviorManager"/> class.</summary>
        /// <param name="parent">The parent.</param>
        public BehaviorManager(Thing parent)
        {
            ManagedBehaviors = new List<Behavior>();
            Parent = parent;
        }

        /// <summary>Gets the parent (container) of this instance.</summary>
        /// <value>The parent.</value>
        [JsonIgnore]
        public Thing Parent { get; private set; }

        /// <summary>Gets a snapshot of all managed behaviors.</summary>
        [JsonIgnore]
        public ReadOnlyCollection<Behavior> AllBehaviors
        {
            get
            {
                lock (ManagedBehaviors)
                {
                    return ManagedBehaviors.ToList().AsReadOnly();
                }
            }
        }

        /// <summary>Gets or sets the list of behaviors managed by this BehaviorManager.</summary>
        /// <remarks>This list is private in order to enforce property locking thread safety, disallowing simultaneous iteration and edits.</remarks>
        private List<Behavior> ManagedBehaviors { get; set; }

        /// <summary>Clone this BehaviorManager.</summary>
        /// <remarks>Override Clone for all derived BehaviorManagers!</remarks>
        /// <returns>Throws an exception; derived managers should not call base.Clone.</returns>
        public virtual object Clone()
        {
            throw new Exception("Each manager derived from BehaviorManager needs to implement its own Clone!");
        }

        /// <summary>Saves all behaviors in the manager.</summary>
        public virtual void Save()
        {
            throw new Exception("Each manager derived from BehaviorManager needs to implement its own save!");
        }

        /// <summary>Repair the parent relationship of this BehaviorManager and all its behaviors.</summary>
        /// <remarks>
        /// Things like deserialization won't necessarily restore all parent relationships correctly, for a freshly
        /// loaded Thing, so this method provides a means to repair the parent relationship, once, right afterwards.
        /// </remarks>
        public void RepairParent(Thing parent)
        {
            Parent = parent;
            lock (ManagedBehaviors)
            {
                foreach (var behavior in ManagedBehaviors)
                {
                    behavior.SetParent(parent);
                }
            }
        }

        /// <summary>Determines whether this instance houses items that can be stacked with items in the other behavior manager.</summary>
        /// <param name="otherBehaviorManager">The other behavior manager.</param>
        /// <returns>true if items in this instance can stack with items from the specified other behavior manager; otherwise, false.</returns>
        public bool CanStack(BehaviorManager otherBehaviorManager)
        {
            // TODO: Determine whether these BehaviorManagers house item-stackably-identical behavior instances.
            //// throw new NotImplementedException();

            return false;
        }

        /// <summary>Find the first instance of a behavior of the specified type.</summary>
        /// <typeparam name="U">The type of behavior to locate.</typeparam>
        /// <returns>The first managed behavior of the specified type, if found, else null.</returns>
        public U FindFirst<U>() where U : Behavior
        {
            lock (ManagedBehaviors)
            {
                return ManagedBehaviors.OfType<U>().FirstOrDefault();
            }
        }

        /// <summary>Gets a filtered list of Behaviors of the specified type.</summary>
        /// <typeparam name="T">The type of Behaviors to find.</typeparam>
        /// <returns>A filtered list of Behaviors of the specified type.</returns>
        public List<T> OfType<T>() where T : Behavior
        {
            lock (ManagedBehaviors)
            {
                return ManagedBehaviors.OfType<T>().ToList();
            }
        }

        /// <summary>Add a new behavior to the list of managed behaviors.</summary>
        /// <param name="newBehavior">The new behavior to add.</param>
        public void Add(Behavior newBehavior)
        {
            lock (ManagedBehaviors)
            {
                if (!ManagedBehaviors.Contains(newBehavior))
                {
                    ManagedBehaviors.Add(newBehavior);
                    newBehavior.SetParent(Parent);
                }
            }
        }

        /// <summary>Removes the specified behavior.</summary>
        /// <param name="behavior">The behavior to remove.</param>
        public void Remove(Behavior behavior)
        {
            lock (ManagedBehaviors)
            {
                if (ManagedBehaviors.Contains(behavior))
                {
                    ManagedBehaviors.Remove(behavior);
                    behavior.SetParent(null);
                }
            }
        }

        /// <summary>Clone all the behaviors being managed by the specified manager.</summary>
        /// <param name="existingManager">The manager whose behaviors we are to clone.</param>
        protected void CloneBehaviors(BehaviorManager existingManager)
        {
            // TODO: Update comment to use a relevant example.
            // When cloning a behavior manager, we want a new set of behaviors that are based 
            // on the existing behaviors;  if one object instance's behaviors change (IE a 
            // player spends some gold so their Currency StackableItemBehavior reduces its 
            // own Count property), we don't want all other behaviors that were based on the 
            // original behavior to also have their referenced count get reduced.  IE A Clone 
            // of coin stacks can legitimately occur if the player say, drops 10 coins when 
            // they are carrying 50 coins; the Currency.Clone() is used to make a second coin 
            // stack, (causing the Currency's behaviors to also clone here), then both coin 
            // stacks have their Count property set appropriately.
            lock (existingManager.ManagedBehaviors)
            {
                lock (ManagedBehaviors)
                {
                    ManagedBehaviors.Clear();
                    foreach (Behavior behavior in existingManager.ManagedBehaviors)
                    {
                        Behavior clonedBehavior = behavior.Clone();
                        ManagedBehaviors.Add(clonedBehavior);
                    }
                }
            }
        }
    }
}