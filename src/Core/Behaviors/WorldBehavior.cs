//-----------------------------------------------------------------------------
// <copyright file="WorldBehavior.cs" company="WheelMUD Development Team">
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
    using System.Diagnostics;
    using WheelMUD.Core.Events;
    using WheelMUD.Data.Entities;
    using WheelMUD.Data.Repositories;

    /// <summary>Represents a world in the MUD.</summary>
    public class WorldBehavior : Behavior
    {
        // @@@ TODO: Register to static RoomBehavior.RoomCreated events or something to keep updated?
        // @@@ HMM, what about Template ID vs. instance ID (IE able to have instanced rooms or multiple 
        //     instances of a given vehicle which has a RoomBehavior, etc)
        // @@@ Also, consider whether such caches should use WeakReferences instead of references.
        ////private readonly Dictionary<long, RoomBehavior> roomsCache;

        /// <summary>Initializes a new instance of the WorldBehavior class.</summary>
        public WorldBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the WorldBehavior class.</summary>
        /// <param name="instanceID">The instance ID.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public WorldBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /// <summary>Loads areas into the world.</summary>
        public void Load()
        {
            var areaRepository = new AreaRepository();
            ICollection<AreaRecord> areas = areaRepository.FetchAll();

            foreach (var areaRecord in areas)
            {
                var areaBehavior = new AreaBehavior()
                {
                    ID = areaRecord.ID,
                };
                var area = new Thing(areaBehavior)
                {
                    Name = areaRecord.Name,
                    ID = "area/" + areaRecord.ID,
                };

                // Load this area and it's children.
                areaBehavior.Load();
                this.Parent.Add(area);
            }
        }

        /// <summary>Searches for a given room and returns it if found.</summary>
        /// <param name="roomId">The room ID.</param>
        /// <returns>The Room found.</returns>
        public Thing FindRoom(long roomId)
        {
            ////foreach (KeyValuePair<long, Thing> kvp in this.Areas)
            ////{
            ////    if (kvp.Value.Rooms.ContainsKey(roomId))
            ////    {
            ////        return kvp.Value.Rooms[roomId];
            ////    }
            ////}

            ////Thing roomBehavior = this.roomsCache[roomId];
            Thing roomBehavior = null;

            if (roomBehavior == null)
            {
                return null;
            }

            return roomBehavior.Parent;
        }

        /// <summary>Called when a parent has just been assigned to this behavior. (Refer to this.Parent.)</summary>
        public override void OnAddBehavior()
        {
            // Once our WorldBehavior is attached to a thing, ensure that thing isn't currently a
            // child of anything else, and don't let that happen later either.
            Debug.Assert(this.Parent.Parent == null, "A world cannot be a child of any other thing.");
            this.Parent.Eventing.MovementRequest += this.DenyWorldParentChanges;
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
        }

        /// <summary>Event handler for denying the changing of the parent of a Thing which has a WorldBehavior since World should be highest.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void DenyWorldParentChanges(Thing sender, CancellableGameEvent e)
        {
            if (e.ActiveThing == this.Parent)
            {
                var addChildRequest = e as AddChildEvent;
                if (addChildRequest != null)
                {
                    addChildRequest.Cancel("World cannot become a child of anything.");
                }
            }
        }
    }
}