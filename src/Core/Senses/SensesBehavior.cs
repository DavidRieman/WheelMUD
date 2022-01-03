﻿//-----------------------------------------------------------------------------
// <copyright file="SensesBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace WheelMUD.Core
{
    /// <summary>Encapsulates sensory behavior.</summary>
    public class SensesBehavior : Behavior
    {
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
            ID = instanceId;
        }

        /// <summary>Gets or sets the senses this thing has access to.</summary>
        public SenseManager Senses { get; set; }

        /// <summary>Have the entity perceive a list of possible exits with its senses.</summary>
        /// <returns>A list of perceived exits.</returns>
        public List<string> PerceiveExits()
        {
            if (Parent != null)
            {
                var exits = new List<string>();

                // If the thing we are in now is exitable, then add the exit command.
                var location = Parent.Parent;
                var enterableExitableBehavior = Parent.FindBehavior<EnterableExitableBehavior>();
                if (enterableExitableBehavior != null)
                {
                    exits.Add(enterableExitableBehavior.ExitCommand);
                }

                // Check our parent's parent (location) for exits (or enterables).
                if (location != null)
                {
                    foreach (Thing thing in location.Children)
                    {
                        var exitBehavior = thing.FindBehavior<ExitBehavior>();
                        if (exitBehavior != null)
                        {
                            if (thing.IsDetectableBySense(Senses))
                            {
                                exits.Add(exitBehavior.GetExitCommandFrom(location));
                            }
                        }
                        else
                        {
                            enterableExitableBehavior = thing.FindBehavior<EnterableExitableBehavior>();
                            if (enterableExitableBehavior != null)
                            {
                                if (thing.IsDetectableBySense(Senses))
                                {
                                    exits.Add(enterableExitableBehavior.EnterCommand);
                                }
                            }
                        }
                    }
                }

                exits.Sort();

                return exits;
            }

            return null;
        }

        /// <summary>Have the entity perceive a list of entities with its senses.</summary>
        /// <returns>A list of perceived entities.</returns>
        public IList<Thing> PerceiveEntities()
        {
            // TODO: Refactor the perceive categories... players, mobs, items, exits, etc.
            if (Parent != null)
            {
                var outEntities = new List<Thing>();

                // TODO: Change Parent.Parent to a predicate that will find RoomBehaviors. This is a an ugly hack that needs to go away.
                var entities = Parent.Parent.Children.Where(t => t.HasBehavior<PlayerBehavior>() || t.HasBehavior<MobileBehavior>());

                foreach (Thing thing in entities)
                {
                    // TODO: Add '&& thing has EntityBehavior' or whatnot...
                    if (thing != Parent && thing.IsDetectableBySense(Senses))
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
        /// <remarks>
        /// TODO: Remove? Not sure if this is going to be necessary, as a RoomRenderer and such for a given game
        ///       is free to make its own decisions about how to group/categorize Things that are in the room, etc.
        /// </remarks>
        public IList<Thing> PerceiveItems()
        {
            var self = Parent;
            if (self != null)
            {
                var room = self.Parent;
                if (room != null)
                {
                    var items = new List<Thing>();
                    foreach (Thing item in room.Children)
                    {
                        // TODO: Use something like 'has ItemBehavior' instead?
                        if (item.IsDetectableBySense(Senses) &&
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
            // Distance-size, the perceiving thing should be able to perceive the place it is in (e.g. room), other
            // things in the same place, and its own things (e.g. inventory items).
            bool isLocal = Parent.Parent == thing ||
                (Parent.Parent != null && Parent.Parent.FindChild(t => t == thing) != null) ||
                Parent.FindChild(t => t == thing) != null;
            return isLocal && thing.IsDetectableBySense(Senses);
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            Senses = new SenseManager();
            LoadDefaultSenses();
        }

        /// <summary>Load the senses of the entity.</summary>
        private void LoadDefaultSenses()
        {
            // TODO: Each sense's details should be persistable/designable per race, etc.
            Senses.AddSense(new Sense()
            {
                SensoryType = SensoryType.Hearing,
                MessagePrefix = "[HEARING]",
                Measurement = SensoryTypeMeasurement.Decibel,
                Enabled = true,
                LowThreshold = 0,
                HighThreshold = 100,
            });

            Senses.AddSense(new Sense()
            {
                SensoryType = SensoryType.Sight,
                MessagePrefix = "[SIGHT]",
                Measurement = SensoryTypeMeasurement.Lumen,
                Enabled = true,
                LowThreshold = 0,
                HighThreshold = 100,
            });

            Senses.AddSense(new Sense()
            {
                SensoryType = SensoryType.Smell,
                MessagePrefix = "[SMELL]",
                Measurement = SensoryTypeMeasurement.PartsPerMillion,
                Enabled = true,
                LowThreshold = 0,
                HighThreshold = 100,
            });

            Senses.AddSense(new Sense()
            {
                SensoryType = SensoryType.Touch,
                MessagePrefix = "[TOUCH]",
                Measurement = SensoryTypeMeasurement.PoundsPerSquareInch,
                Enabled = true,
                LowThreshold = 0,
                HighThreshold = 100,
            });

            Senses.AddSense(new Sense()
            {
                SensoryType = SensoryType.Taste,
                MessagePrefix = "[TASTE]",
                Measurement = SensoryTypeMeasurement.PartsPerMillion,
                Enabled = true,
                LowThreshold = 0,
                HighThreshold = 100,
            });

            Senses.AddSense(new Sense()
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