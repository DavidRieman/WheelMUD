//-----------------------------------------------------------------------------
// <copyright file="SensesBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: May 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections.Generic;
    using Raven.Imports.Newtonsoft.Json;
    using WheelMUD.Core.Enums;

    /// <summary>Encapsulates sensory behavior.</summary>
    public class SensesBehavior : Behavior
    {
        /// <summary>The synchronization locking object.</summary>
        [JsonIgnore]
        private readonly object lockObject = new object();

        /// <summary>Initializes a new instance of the SensesBehavior class.</summary>
        public SensesBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the SensesBehavior class.</summary>
        /// <param name="instanceId">The instance ID.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public SensesBehavior(long instanceId, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceId;
        }

        /// <summary>Gets or sets the senses this thing has access to.</summary>
        public SenseManager Senses { get; set; }

        /// <summary>Gets the things last perceived by this thing.</summary>
        [JsonIgnore]
        public List<Thing> PerceivedThings { get; private set; }

        /// <summary>Have the entity perceive a list of possible exits with its senses.</summary>
        /// <returns>A list of perceived exits.</returns>
        public List<string> PerceiveExits()
        {
            if (this.Parent != null)
            {
                var exits = new List<string>();

                // If the thing we are in now is exitable, then add the exit command.
                var location = this.Parent.Parent;
                var enterableExitableBehavior = this.Parent.Behaviors.FindFirst<EnterableExitableBehavior>();
                if (enterableExitableBehavior != null)
                {
                    exits.Add(enterableExitableBehavior.ExitCommand);
                }

                // Check our parent's parent (location) for exits (or enterables).
                if (location != null)
                {
                    foreach (Thing thing in location.Children)
                    {
                        var exitBehavior = thing.Behaviors.FindFirst<ExitBehavior>();
                        if (exitBehavior != null)
                        {
                            if (thing.IsDetectableBySense(this.Senses))
                            {
                                exits.Add(exitBehavior.GetExitCommandFrom(location));
                            }
                        }
                        else
                        {
                            enterableExitableBehavior = thing.Behaviors.FindFirst<EnterableExitableBehavior>();
                            if (enterableExitableBehavior != null)
                            {
                                if (thing.IsDetectableBySense(this.Senses))
                                {
                                    exits.Add(enterableExitableBehavior.EnterCommand);
                                }
                            }
                        }
                    }
                }

                return exits;
            }

            return null;
        }

        /// <summary>Have the entity perceive a list of entities with its senses.</summary>
        /// <returns>A list of perceived entities.</returns>
        public IList<Thing> PerceiveEntities()
        {
            // @@@ TODO: Refactor the perceive categories... players, mobs, items, exits, etc.
            if (this.Parent != null)
            {
                var outEntities = new List<Thing>();
                
                // @@@ TODO: Change Parent.Parent to a predicate that will find RoomBehaviors. This is a an ugly hack that needs to go away.
                var entities = this.Parent.Parent.FindAllChildren(t => t.HasBehavior<PlayerBehavior>() || t.HasBehavior<MobileBehavior>());

                foreach (Thing thing in entities)
                {
                    // @@@ ADD: '&& thing has EntityBehavior' or whatnot...
                    if (thing != this.Parent && thing.IsDetectableBySense(this.Senses))
                    {
                        outEntities.Add(thing);
                    }
                }

                return outEntities.AsReadOnly();
            }

            return new List<Thing>();
        }

        /// <summary>Have the entity perceive a list of items with its senses.</summary>
        /// <returns>A list of perceived items.</returns>
        public IList<Thing> PerceiveItems()
        {
            var self = this.Parent;
            if (self != null)
            {
                var room = self.Parent;
                if (room != null)
                {
                    var items = new List<Thing>();
                    foreach (Thing item in room.Children)
                    {
                        // @@@ Use something like 'has ItemBehavior' instead?
                        if (item.IsDetectableBySense(this.Senses) &&
                            !item.HasBehavior<ExitBehavior>() &&
                            !item.HasBehavior<PlayerBehavior>() &&
                            !item.HasBehavior<MobileBehavior>())
                        {
                            items.Add(item);
                        }
                    }

                    return items.AsReadOnly();
                }
            }

            return null;
        }

        /// <summary>Determines whether the thing passed into the function can be perceived by this entity.</summary>
        /// <remarks>Checks the Items, Entities and Exits for the Thing passed.</remarks>
        /// <param name="thing">The thing to test.</param>
        /// <returns>true if the thing can be perceived; otherwise false.</returns>
        public bool CanPerceiveThing(Thing thing)
        {
            // @@@ Doesn't seem like this should be bound only by this entity's location?
            //     What about detecting something in a bag, etc?
            return this.Parent.FindChild(t => t == thing) != null && thing.IsDetectableBySense(this.Senses);
        }

        /// <summary>Allows the entity to determine what things are around it.</summary>
        public void ProcessSurroundings()
        {
            if (this.Parent == null)
            {
                return;
            }

            lock (this.lockObject)
            {
                this.PerceivedThings.Clear();
                this.PerceivedThings.AddRange(this.PerceiveItems());
                this.PerceivedThings.AddRange(this.PerceiveEntities());
            }
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            this.PerceivedThings = new List<Thing>();
            this.Senses = new SenseManager();
            this.LoadSenses();
        }

        /// <summary>Load the senses of the entity.</summary>
        private void LoadSenses()
        {
            // @@@ TODO: Each sense's details should be persistable/designable per race, etc.
            this.Senses.AddSense(new Sense()
            {
                SensoryType = SensoryType.Hearing,
                MessagePrefix = "[HEARING]",
                Measurement = SensoryTypeMeasurement.Decibel,
                Enabled = true,
                LowThreshold = 0,
                HighThreshold = 100,
            });

            this.Senses.AddSense(new Sense()
            {
                SensoryType = SensoryType.Sight,
                MessagePrefix = "[SIGHT]",
                Measurement = SensoryTypeMeasurement.Lumen,
                Enabled = true,
                LowThreshold = 0,
                HighThreshold = 100,
            });

            this.Senses.AddSense(new Sense()
            {
                SensoryType = SensoryType.Smell,
                MessagePrefix = "[SMELL]",
                Measurement = SensoryTypeMeasurement.PartsPerMillion,
                Enabled = true,
                LowThreshold = 0,
                HighThreshold = 100,
            });

            this.Senses.AddSense(new Sense()
            {
                SensoryType = SensoryType.Touch,
                MessagePrefix = "[TOUCH]",
                Measurement = SensoryTypeMeasurement.PoundsPerSquareInch,
                Enabled = true,
                LowThreshold = 0,
                HighThreshold = 100,
            });

            this.Senses.AddSense(new Sense()
            {
                SensoryType = SensoryType.Taste,
                MessagePrefix = "[TASTE]",
                Measurement = SensoryTypeMeasurement.PartsPerMillion,
                Enabled = true,
                LowThreshold = 0,
                HighThreshold = 100,
            });

            this.Senses.AddSense(new Sense()
            {
                SensoryType = SensoryType.Debug,
                MessagePrefix = "[DEBUG]",
                Measurement = SensoryTypeMeasurement.Debug,
                Enabled = true,
                LowThreshold = 0,
                HighThreshold = 100,
            });
        }
    }
}