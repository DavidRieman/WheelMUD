//-----------------------------------------------------------------------------
// <copyright file="ThingManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   High level manager that provides tracking and global collection of all Thing instances.
//   Created: December 2010 by Karak, based on ItemManager as created January 2007 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using WheelMUD.Interfaces;

    /// <summary>High level manager that provides tracking and global collection of all Thing instances.</summary>
    /// <remarks>
    /// @@@ TODO: Provide search ability of registered Things through LINQ rather than, 
    /// or in addition to, specific-purpose search methods.
    /// </remarks>
    public class ThingManager : ManagerSystem
    {
        /// <summary>The singleton instance of this class.</summary>
        private static readonly Lazy<ThingManager> SingletonInstance = new Lazy<ThingManager>(() => new ThingManager());

        /// <summary>The dictionary of all managed Things.</summary>
        private readonly ConcurrentDictionary<string, Thing> things = new ConcurrentDictionary<string, Thing>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Prevents a default instance of the <see cref="ThingManager"/> class from being created.</summary>
        private ThingManager()
        {
        }

        /// <summary>Gets the singleton <see cref="ThingManager"/> instance.</summary>
        public static ThingManager Instance
        {
            get { return SingletonInstance.Value; }
        }

        /// <summary>Gets the collection of things.</summary>
        public ICollection<Thing> Things
        {
            get { return this.things.Values.ToList().AsReadOnly(); }
        }

        /// <summary>Determines whether a Thing is currently spawned in the world given the item id.</summary>
        /// <param name="thingID">The Thing ID to search for.</param>
        /// <returns><c>true</c> if Thing is in the world; otherwise, <c>false</c>.</returns>
        public bool IsThingInWorld(string thingID)
        {
            return this.things.ContainsKey(thingID);
        }

        /// <summary>Determines whether any Things match the specified condition.</summary>
        /// <param name="condition">The condition.</param>
        /// <returns>True if at least one Thing matches the condition; otherwise, false.</returns>
        public bool Any(Func<Thing, bool> condition)
        {
            return this.things.Values.Any(condition);
        }

        /// <summary>Counts the number of Things matching the specified condition.</summary>
        /// <param name="condition">The condition.</param>
        /// <returns>The number of Things matching the specified condition.</returns>
        public int Count(Func<Thing, bool> condition)
        {
            return this.things.Values.Count(condition);
        }

        /// <summary>Gets a Thing given a Thing ID.</summary>
        /// <param name="thingID">The ID of the Thing to get.</param>
        /// <returns>The <c>Thing</c> referenced by the given ID if found; otherwise <c>null</c></returns>
        public Thing FindThing(string thingID)
        {
            Thing thing;
            this.things.TryGetValue(thingID, out thing);
            return thing;
        }

        /// <summary>Finds a thing using a name or part name.</summary>
        /// <param name="name">The name of the thing to return.</param>
        /// <param name="partialMatch">Used to indicate whether the search criteria can look at just the start of the name.</param>
        /// <param name="ignoreCase">Set to true to ignore case for the search, false for case sensitive.</param>
        /// <returns>The Thing found, or null if no matching Thing was found.</returns>
        public Thing FindThingByName(string name, bool partialMatch = false, bool ignoreCase = false)
        {
            StringComparison comparison = ignoreCase
                                              ? StringComparison.CurrentCultureIgnoreCase
                                              : StringComparison.CurrentCulture;

            return partialMatch
                       ? this.things.Values.FirstOrDefault(thing => thing.Name.StartsWith(name, ignoreCase, null))
                       : this.things.Values.FirstOrDefault(thing => string.Equals(thing.Name, name, comparison));
        }

        /// <summary>Retrieves a collection of things matching the specified condition.</summary>
        /// <param name="condition">The condition.</param>
        /// <returns>A collection of things matching the condition.</returns>
        public IList<Thing> Find(Func<Thing, bool> condition)
        {
            return this.things.Values.Where(condition).ToList().AsReadOnly();
        }

        /// <summary>Tries to retrieve a Thing matching the specified condition.</summary>
        /// <param name="condition">The condition.</param>
        /// <returns>The first Thing matching the condition, or null if none was found.</returns>
        public Thing FirstOrDefault(Func<Thing, bool> condition)
        {
            return this.things.Values.FirstOrDefault(condition);
        }

        /// <summary>Tries to move a Thing from its current location into the specified location, if that thing is movable.</summary>
        /// <param name="thing">The Thing to move.</param>
        /// <param name="destinationThing">The new container to house the Thing.</param>
        /// <param name="goingVia">The going via.</param>
        /// <param name="leavingMessage">The leaving message.</param>
        /// <param name="arrivingMessage">The arriving message.</param>
        public void MoveThing(Thing thing, Thing destinationThing, Thing goingVia, SensoryMessage leavingMessage, SensoryMessage arrivingMessage)
        {
            MovableBehavior movableBehavior = thing.Behaviors.FindFirst<MovableBehavior>();
            if (movableBehavior != null)
            {
                movableBehavior.Move(destinationThing, goingVia, leavingMessage, arrivingMessage);
            }
        }

        /// <summary>Destroys the specified Thing.</summary>
        /// <param name="thing">The thing to destroy.</param>
        /// <returns>True if the destruction was successful; otherwise, false.</returns>
        public bool DestroyThing(Thing thing)
        {
            if (thing.Parent != null)
            {
                thing.Parent.Remove(thing);
            }

            Thing removedThing;
            return this.things.TryRemove(thing.ID, out removedThing);
        }

        /// <summary>Calls <see cref="DestroyThing"/> on all things matching the specified condition.</summary>
        /// <param name="condition">The condition.</param>
        /// <returns>The number of things that were successfully destroyed.</returns>
        public int Destroy(Func<Thing, bool> condition)
        {
            var thingsToRemove = this.things.Values.Where(condition);

            return thingsToRemove.Count(thing => this.DestroyThing(thing));
        }

        /// <summary>Starts this system's individual components.</summary>
        public override void Start()
        {
        }

        /// <summary>Stops this system's individual components.</summary>
        public override void Stop()
        {
            this.things.Clear();
        }

        /// <summary>Add or update the ThingManager's cache of Things; should be called when the ID of a Thing is established or changes.</summary>
        /// <param name="oldId">The previous ID of the Thing, or 0 if it has not had an ID yet.</param>
        /// <param name="newId">The new ID of the Thing.</param>
        /// <param name="updatedThing">The updated thing.</param>
        /// <returns>
        /// True if the update was successful. If false, presumably another call won a race
        /// and the caller should either update their reference or try again.
        /// </returns>
        internal bool UpdateThingRegistration(string oldId, string newId, Thing updatedThing)
        {
            Debug.Assert(oldId != newId, "UpdateThingRegistration should not be called when not changing the Thing ID.");
            Debug.Assert(!string.IsNullOrEmpty(newId), "After initialization, a Thing's ID should never become null or empty!");
            ////Debug.Assert(!this.things.ContainsKey(newID), "A Thing has been assigned an ID which is not unique!");

            if (!string.IsNullOrEmpty(oldId))
            {
                Thing removedThingOldId;
                this.things.TryRemove(oldId, out removedThingOldId);
            }

            Thing removedThingNewId;
            this.things.TryRemove(newId, out removedThingNewId);

            return this.things.TryAdd(newId, updatedThing);
        }

        /// <summary>Exporter for MEF.</summary>
        [ExportSystem]
        public class ThingManagerExporter : SystemExporter
        {
            /// <summary>Gets the singleton system instance.</summary>
            /// <returns>A new instance of the singleton system.</returns>
            public override ISystem Instance
            {
                get { return ThingManager.Instance; }
            }

            /// <summary>Gets the Type of the singleton system, without instantiating it.</summary>
            /// <returns>The Type of the singleton system.</returns>
            public override Type SystemType
            {
                get { return typeof(ThingManager); }
            }
        }
    }
}