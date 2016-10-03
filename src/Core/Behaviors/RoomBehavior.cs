//-----------------------------------------------------------------------------
// <copyright file="RoomBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: June 2010 by Karak.
//   Updated: June 2011 by Karak: fixing recreation of multiple-parent exits from simple A-to-B DB entries.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using WheelMUD.Data.Entities;
    using WheelMUD.Data.Repositories;

    /// <summary>Represents a room in the MUD.</summary>
    public class RoomBehavior : Behavior
    {
        /// <summary>Info on exits which still need to be rigged to a secondary room/Thing, once said Thing is loaded.</summary>
        private static List<PendingExitRigging> pendingExitRiggings = new List<PendingExitRigging>();

        /// <summary>Collection of room visuals.</summary>
        /// <remarks>Keys are the visual item names, and values are the descriptions shown to players when they look at the visuals.</remarks>
        private Dictionary<string, string> visuals = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

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
            this.ID = instanceId;
        }

        /// <summary>
        /// Gets the collection of visuals for this room.
        /// A "visual" is a pseudo-item within a room, which the player can look at.
        /// The keys of this dictionary contain the names of visual items, and the
        /// values are the corresponding descriptions shown to players.
        /// </summary>
        public Dictionary<string, string> Visuals
        {
            get
            {
                return this.visuals;
            }
        }

        /// <summary>
        /// Searches for a visual within the room. The partial match allows the player
        /// to e.g. "look wha" as a shortcut for "look whatchamacallit".
        /// </summary>
        /// <param name="partialTerm">The name of the visual item to find.</param>
        /// <returns>A string to describe the visual item, or null if the item was not found.</returns>
        public string FindVisual(string partialTerm)
        {
            var matches = this.visuals.Where(pair => pair.Key.StartsWith(partialTerm)).Select(pair => pair.Value);
            return matches.FirstOrDefault();
        }

        /// <summary>Loads the exits for this room behavior.</summary>
        public void Load()
        {
            var roomRepository = new RoomRepository();

            // Standard exits are simple DB records, so we need to load them specifically for this room;
            // non-standard exits should be loaded just like any other generic Thing.
            // @@@ TODO: Fix hack: http://www.wheelmud.net/tabid/59/aft/1622/Default.aspx
            string roomNumber = this.Parent.ID.Replace("room/", string.Empty);
            long persistedRoomID = long.Parse(roomNumber);
            ICollection<ExitRecord> exits = roomRepository.LoadExitsForRoom(persistedRoomID);

            foreach (var exitRecord in exits)
            {
                // Create a Thing to represent this exit, which can live in multiple places (IE rooms)
                // as a child of each parent - thus sharing substate (like for doors) will be automatic.
                var exitBehavior = new ExitBehavior()
                {
                    ID = exitRecord.ID,
                };
                var exit = new Thing(exitBehavior, new MultipleParentsBehavior())
                {
                    Name = "[StandardExit]",
                    ID = "exit/" + exitRecord.ID,
                };
                
                // Add the exit destinations.
                string exitRoomA = "room/" + exitRecord.ExitRoomAID.ToString(CultureInfo.InvariantCulture);
                string exitRoomB = "room/" + exitRecord.ExitRoomBID.ToString(CultureInfo.InvariantCulture);
                exitBehavior.AddDestination(exitRecord.DirectionA, exitRoomB);
                exitBehavior.AddDestination(exitRecord.DirectionB, exitRoomA);

                // Add this Exit Thing as a child of the Room Thing.
                this.Parent.Add(exit);

                // Look for the other room; if it exists, add this exit to that room too, else
                // set up an event reaction to add the exit to the room when it gets loaded.
                var otherRoom = ThingManager.Instance.FindThing(exitRoomB);
                if (otherRoom != null)
                {
                    otherRoom.Add(exit);
                }
                else
                {
                    lock (pendingExitRiggings)
                    {
                        pendingExitRiggings.Add(new PendingExitRigging()
                        {
                            RoomID = exitRoomB,
                            ExitThing = exit,
                        });
                    }
                }
            }

            // If this room is the secondary parent for a pending exit rigging, rig it up.
            lock (pendingExitRiggings)
            {
                var matchedExitRiggings = this.FindMatchedPendingExitRiggings(this.Parent.ID);
                foreach (var matchedExitRigging in matchedExitRiggings)
                {
                    this.Parent.Add(matchedExitRigging.ExitThing);
                    pendingExitRiggings.Remove(matchedExitRigging);
                }
            }
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
        }

        /// <summary>Find pending exit riggings which are meant to be added to the specified unique Thing ID.</summary>
        /// <param name="matchID">The unique thing ID looking for matches.</param>
        /// <returns>All matching pending exit riggings intended for the specified unique ID.</returns>
        private IEnumerable<PendingExitRigging> FindMatchedPendingExitRiggings(string matchID)
        {
            return (from r in pendingExitRiggings
                    where r.RoomID == matchID
                    select r).ToList();
        }

        /// <summary>Information about an Exit which still needs to be rigged to an additional room/Thing.</summary>
        private class PendingExitRigging
        {
            /// <summary>Gets or sets the Thing which needs to be rigged up to another room (or other Thing).</summary>
            public Thing ExitThing { get; set; }

            /// <summary>Gets or sets the room (or other Thing) this exit needs to be rigged up to.</summary>
            public string RoomID { get; set; }
        }
    }
}