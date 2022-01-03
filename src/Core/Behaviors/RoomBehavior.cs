//-----------------------------------------------------------------------------
// <copyright file="RoomBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace WheelMUD.Core
{
    // TODO: Move to own file?
    public class Furnishing
    {
        public Furnishing() { }
        public string[] Keywords { get; set; }
        public string Description { get; set; }
    }

    /// <summary>Represents a room in the MUD.</summary>
    public class RoomBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the RoomBehavior class.</summary>
        public RoomBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the RoomBehavior class.</summary>
        /// <param name="instanceId">The instance ID.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public RoomBehavior(long instanceId, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            ID = instanceId;
        }

        /// <summary>
        /// Gets the collection of furnishings for this room.
        /// A "furnishing" is a specific aspect of this thing which the player can look at for more info, but cannot deeply interact with.
        /// </summary>
        /// <summary>Collection of furnishings (basic extra room furnishings to examine).</summary>
        /// <remarks>
        /// TODO: Consider moving to Thing so you can look at aspects of your inventory items and Things in the current room.
        ///       The same OLC rules (forcing keywords to highlight) should apply to those Things descriptions as well.
        /// </remarks>
        public List<Furnishing> Furnishings { get; private set; } = new List<Furnishing>();

        /// <summary>
        /// Searches for a visual within the room. The partial match allows the player
        /// to e.g. "look wha" as a shortcut for "look whatchamacallit".
        /// </summary>
        /// <param name="partialTerm">The name of the visual item to find.</param>
        /// <returns>A string to describe the visual item, or null if the item was not found.</returns>
        /// <remarks>TODO: Should tie in to standard targeting. TODO: Thread-safe adds/removes too.</remarks>
        public Furnishing FindFurnishing(string partialTerm)
        {
            lock (Furnishings)
            {
                return (from f in Furnishings
                        where f.Keywords.Contains(partialTerm, StringComparer.OrdinalIgnoreCase)
                        select f).FirstOrDefault();
            }
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
        }
    }
}