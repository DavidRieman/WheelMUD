//-----------------------------------------------------------------------------
// <copyright file="Stat.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A statistic.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using Raven.Imports.Newtonsoft.Json;
    using WheelMUD.Core.Events;
    using WheelMUD.Interfaces;

    /// <summary>A base class used to help define various elements of game attributes/stats/skills/etc.</summary>
    public abstract class BaseStat
    {
        /// <summary>The synchronization locking object.</summary>
        [JsonIgnore]
        private readonly object lockObject = new object();

        /// <summary>The current value of this stat.</summary>
        private int currentValue;

        /// <summary>The maximum value this stat can have.</summary>
        private int maxValue = 100;

        /// <summary>The minimum value this stat can have.</summary>
        private int minValue;

        /// <summary>Initializes a new instance of the <see cref="BaseStat"/> class.</summary>
        /// <param name="controller">The controller.</param>
        /// <param name="name">The stat name.</param>
        /// <param name="abbreviation">The ID that will be used to allow the gaming system to recognize this stat.</param>
        /// <param name="formula">The formula that may be needed to calculate the value of this stat.</param>
        /// <param name="value">The stat value.</param>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        /// <param name="visible">If set to <c>true</c> make this stat [visible].</param>
        public BaseStat(IController controller, string name, string abbreviation, string formula, int value, int minValue, int maxValue, bool visible)
        {
            this.minValue = 0;
            this.Host = controller;
            this.Name = name;
            this.Abbreviation = abbreviation;
            this.Formula = formula;
            this.currentValue = value;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.Visible = visible;
        }

        /// <summary>Initializes a new instance of the <see cref="BaseStat"/> class.</summary>
        /// <param name="controller">The parent controlling object.</param>
        /// <param name="name">The human readable name for this stat.</param>
        /// <param name="abbreviation">The ID that will be used to allow the gaming system to recognize this stat.</param>
        /// <param name="formula">The formula that may be needed to calculate the value of this stat.</param>
        public BaseStat(IController controller, string name, string abbreviation, string formula)
            : this(controller, name, abbreviation, formula, 0, 0, 100, true)
        {
        }

        /// <summary>Gets the name of this stat.</summary>
        public string Name { get; private set; }

        /// <summary>Gets the current value of this stat.</summary>
        public int Value
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.currentValue;
                }
            }
        }

        /// <summary>Gets the ID that will be used in formulas for calculating stuff in the different gaming systems.</summary>
        public string Abbreviation { get; private set; }

        /// <summary>Gets or sets the formula that will be used to determine the value of this stat.</summary>
        public string Formula { get; set; }

        /// <summary>Gets or sets the maximum value of this stat.</summary>
        public int MaxValue
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.maxValue;
                }
            }

            set
            {
                lock (this.lockObject)
                {
                    this.maxValue = value;
                }
            }
        }

        /// <summary>Gets or sets the minimum value of this stat.</summary>
        public int MinValue
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.minValue;
                }
            }

            set
            {
                lock (this.lockObject)
                {
                    this.minValue = value;
                }
            }
        }

        /// <summary>Gets a value indicating whether this state is visible.</summary>
        public bool Visible { get; private set; }

        /// <summary>Gets or sets the parent Thing this stat belongs to.</summary>
        /// <remarks>@@@ TODO: IMPLEMENT OnAdd AND OnRemove REACTIONS WHEN CHANGING PARENT AS PER BEHAVIORS!</remarks>
        [JsonIgnore]
        public Thing Parent { get; set; }

        /// <summary>Gets or sets the host that this stat applies to.</summary>
        [JsonIgnore]
        protected IController Host { get; set; }

        /// <summary>Called when a parent Thing has just been assigned this game element.</summary>
        public virtual void OnAdd()
        {
        }

        /// <summary>Called when the current parent Thing of this game element is about to be removed.</summary>
        public virtual void OnRemove()
        {
        }

        /// <summary>Sets the value of the stat.</summary>
        /// <param name="value">The new value.</param>
        /// <param name="sender">The sender of the stat change.</param>
        /// <param name="message">The contextual message to broadcast with the change.</param>
        public virtual void SetValue(int value, Thing sender, ContextualStringBuilder message)
        {
            lock (this.lockObject)
            {
                int oldValue = this.currentValue;

                if (this.Host != null)
                {
                    // Check if the player is in the character creation state.
                    Thing hostThing = this.Host.Thing;
                    if (hostThing != null)
                    {
                        var e = new StatChangeEvent(
                            hostThing,
                            new SensoryMessage(SensoryType.Sight, 100, message),
                            this,
                            value - oldValue,
                            oldValue);

                        hostThing.Eventing.OnCombatRequest(e, EventScope.ParentsDown);

                        if (!e.IsCancelled)
                        {
                            if (value >= this.maxValue)
                            {
                                this.currentValue = this.maxValue;
                            }
                            else if (value <= this.minValue)
                            {
                                this.currentValue = this.minValue;
                            }
                            else
                            {
                                this.currentValue = value;
                            }

                            hostThing.Eventing.OnCombatEvent(e, EventScope.ParentsDown);
                        }
                    }
                    else
                    {
                        this.currentValue = value;
                    }
                }
                else
                {
                    this.currentValue = value;
                }
            }
        }

        /// <summary>Sets the value of the stat.</summary>
        /// <param name="value">The new value.</param>
        /// <param name="sender">The sender of the stat change.</param>
        public void SetValue(int value, Thing sender)
        {
            this.SetValue(value, sender, null);
        }

        /// <summary>Increases the value of the stat.</summary>
        /// <param name="value">The amount to increase.</param>
        /// <param name="sender">The sender of the stat change.</param>
        /// <param name="message">The contextual message to broadcast with the change.</param>
        public void Increase(int value, Thing sender, ContextualStringBuilder message)
        {
            this.SetValue(this.Value + value, sender, message);
        }

        /// <summary>Increases the value of the stat.</summary>
        /// <param name="value">The amount to increase.</param>
        /// <param name="sender">The sender of the stat change.</param>
        public void Increase(int value, Thing sender)
        {
            this.SetValue(this.Value + value, sender);
        }

        /// <summary>Decreases the value of the stat.</summary>
        /// <param name="value">The amount to increase.</param>
        /// <param name="sender">The sender of the stat change.</param>
        /// <param name="message">The contextual message to broadcast with the change.</param>
        public void Decrease(int value, Thing sender, ContextualStringBuilder message)
        {
            this.SetValue(this.Value - value, sender, message);
        }

        /// <summary>Decreases the value of the stat.</summary>
        /// <param name="value">The amount to increase.</param>
        /// <param name="sender">The sender of the stat change.</param>
        public void Decrease(int value, Thing sender)
        {
            this.SetValue(this.Value - value, sender);
        }
    }
}