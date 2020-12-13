﻿//-----------------------------------------------------------------------------
// <copyright file="Behavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using WheelMUD.Interfaces;
    using WheelMUD.Utilities;

    /// <summary>Base implementation of the Behavior class.</summary>
    public abstract class Behavior : IPersistsWithPlayer
    {
        /// <summary>The synchronization locking object.</summary>
        [JsonIgnore]
        private readonly object lockObject = new object();

        /// <summary>Initializes a new instance of the Behavior class.</summary>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        protected Behavior(Dictionary<string, object> instanceProperties)
        {
            this.ID = 0;
            this.SetDefaultProperties();

            // If we're handed a set of propertyName-propertyValue pairs (as may have come from
            // persistence of the behavior or whatnot) then restore those as actual properties.
            if (instanceProperties != null)
            {
                // TODO: Test w/behavior persistence implementation...
                PropertyTools.SetProperties(this, instanceProperties);
            }
        }

        /// <summary>Gets or sets the ID for this behavior instance.</summary>
        public long ID { get; set; }

        /// <summary>Gets the parent of this behavior instance.</summary>
        /// <remarks>Use SetParent to set or change the parent.</remarks>
        [JsonIgnore]
        public Thing Parent { get; private set; }

        public void SetParent(Thing newParent)
        {
            // Ignore SetParent requests that are already satisfied; avoid redundant eventing.
            if (this.Parent != newParent)
            {
                if (this.Parent != null)
                {
                    this.OnRemoveBehavior();
                }
                this.Parent = newParent;
                if (newParent != null)
                {
                    this.OnAddBehavior();
                }
            }
        }

        /// <summary>Called when a parent has just been assigned to this behavior. (Refer to this.Parent)</summary>
        /// <remarks>Especially helpful for registering to relevant parent events.</remarks>
        protected virtual void OnAddBehavior()
        {
        }

        /// <summary>Called when the current parent of this behavior is about to be removed. (Refer to this.Parent)</summary>
        /// <remarks>Especially helpful for unregistering from relevant parent events.</remarks>
        protected virtual void OnRemoveBehavior()
        {
        }

        /// <summary>Abstract placeholder for the clone method.</summary>
        /// <returns>IBehavior object</returns>
        ////public virtual T Clone<T>()
        ////    where T : Behavior, new()
        public Behavior Clone()
        {
            Type t = this.GetType();

            ////TODO: EXPERIMENT OPTIONS:
            ////newBehavior.CloneProperties(this);
            ////newBehavior = (Behavior)this.MemberwiseClone();

            // Create an instance of the most-derived-type (then as Behavior for local storage).
            Behavior newBehavior = (Behavior)Activator.CreateInstance(t);

            // Then clone this instance of that type's properties into the new instance.  We lock
            // during the operation to ensure none of our property values change as we iterate them;
            // we don't have to lock the newBehavior since nobody else has it yet.
            lock (this.lockObject)
            {
                // All Items should be cloneable, and most derived classes should find it sufficient 
                // to allow this base Item.Clone to take care of all the cloning.
                // TODO: Test this.  Especially if any properties have indexers.
                // TODO: Make sure this deep-copies things like behaviors.
                var properties = this.GetType().GetProperties();
                foreach (var property in properties)
                {
                    object value = property.GetValue(this, null);
                    property.SetValue(newBehavior, value, null);
                }

                this.ID = 0;
            }

            newBehavior.ID = 0;
            return newBehavior;
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected abstract void SetDefaultProperties();
    }
}