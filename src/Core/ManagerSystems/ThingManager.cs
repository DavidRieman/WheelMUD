//-----------------------------------------------------------------------------
// <copyright file="ThingManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WheelMUD.Data.Repositories;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Core
{
    /// <summary>High level manager that provides tracking and global collection of all Thing instances.</summary>
    /// <remarks>
    /// TODO: Provide search ability of registered Things through LINQ rather than, 
    /// or in addition to, specific-purpose search methods.
    /// </remarks>
    public class ThingManager : ManagerSystem
    {
        /// <summary>A delegate for announcing a simple event related to a thing (such as having finished loading).</summary>
        public delegate void ThingEventHandler(Thing thing);

        /// <summary>The singleton instance of this class.</summary>
        private static readonly Lazy<ThingManager> SingletonInstance = new Lazy<ThingManager>(() => new ThingManager());

        /// <summary>The dictionary of all managed Things.</summary>
        private readonly ConcurrentDictionary<string, Thing> things = new ConcurrentDictionary<string, Thing>(StringComparer.OrdinalIgnoreCase);

        private readonly Queue<(Thing, string)> pendingChildLoads = new Queue<(Thing, string)>();

        /// <summary>Prevents a default instance of the <see cref="ThingManager"/> class from being created.</summary>
        private ThingManager()
        {
            StartPendingLoadWorker();
        }

        /// <summary>A non-canceled movement event.</summary>
        public event ThingEventHandler ThingLoaded;

        /// <summary>Gets the singleton <see cref="ThingManager"/> instance.</summary>
        public static ThingManager Instance => SingletonInstance.Value;

        /// <summary>Gets the collection of things.</summary>
        public ICollection<Thing> Things => things.Values.ToList().AsReadOnly();

        /// <summary>Determines whether a Thing is currently spawned in the world given the item id.</summary>
        /// <param name="thingID">The Thing ID to search for.</param>
        /// <returns><c>true</c> if Thing is in the world; otherwise, <c>false</c>.</returns>
        public bool IsThingInWorld(string thingID)
        {
            return things.ContainsKey(thingID);
        }

        /// <summary>Determines whether any Things match the specified condition.</summary>
        /// <param name="condition">The condition.</param>
        /// <returns>True if at least one Thing matches the condition; otherwise, false.</returns>
        public bool Any(Func<Thing, bool> condition)
        {
            return things.Values.Any(condition);
        }

        /// <summary>Counts the number of Things matching the specified condition.</summary>
        /// <param name="condition">The condition.</param>
        /// <returns>The number of Things matching the specified condition.</returns>
        public int Count(Func<Thing, bool> condition)
        {
            return things.Values.Count(condition);
        }

        /// <summary>Gets a Thing given a Thing ID.</summary>
        /// <param name="thingID">The ID of the Thing to get.</param>
        /// <returns>The <c>Thing</c> referenced by the given ID if found; otherwise <c>null</c></returns>
        public Thing FindThing(string thingID)
        {
            Debug.Assert(thingID != null);
            if (thingID == null)
            {
                return null;
            }

            Thing thing;
            things.TryGetValue(thingID, out thing);
            return thing;
        }

        private void StartPendingLoadWorker()
        {
            Task.Run(async () =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    int nextDelay = 10;
                    lock (pendingChildLoads)
                    {
                        if (pendingChildLoads.Any())
                        {
                            // Every loaded Thing will queue loading each of its children. However, some of those
                            // children may actually have multiple parents. If a Thing asked us to load a child, but
                            // that child ID has already been loaded, we need to reuse the loaded instance instead.
                            (Thing parent, string childID) = pendingChildLoads.Dequeue();
                            var childToAttach = FindThing(childID) ?? DocumentRepository<Thing>.Load(childID);
                            if (childToAttach != null)
                            {
                                parent.Add(childToAttach);
                            }

                            nextDelay = 0; // If we had work to do, don't wait long to check for the next batch too.
                        }
                    }
                    await Task.Delay(TimeSpan.FromMilliseconds(nextDelay));
                }
            }, cancellationTokenSource.Token);
        }

        public void QueueLoadChild(Thing parent, string childID)
        {
            lock (pendingChildLoads)
            {
                pendingChildLoads.Enqueue((parent, childID));
            }
        }

        public bool LoadsPending
        {
            get
            {
                lock (pendingChildLoads)
                {
                    return pendingChildLoads.Any();
                }
            }
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
                       ? things.Values.FirstOrDefault(thing => thing.Name.StartsWith(name, ignoreCase, null))
                       : things.Values.FirstOrDefault(thing => string.Equals(thing.Name, name, comparison));
        }

        /// <summary>Retrieves a collection of things matching the specified condition.</summary>
        /// <param name="condition">The condition.</param>
        /// <returns>A collection of things matching the condition.</returns>
        public IList<Thing> Find(Func<Thing, bool> condition)
        {
            return things.Values.Where(condition).ToList().AsReadOnly();
        }

        /// <summary>Tries to retrieve a Thing matching the specified condition.</summary>
        /// <param name="condition">The condition.</param>
        /// <returns>The first Thing matching the condition, or null if none was found.</returns>
        public Thing FirstOrDefault(Func<Thing, bool> condition)
        {
            return things.Values.FirstOrDefault(condition);
        }

        /// <summary>Tries to move a Thing from its current location into the specified location, if that thing is movable.</summary>
        /// <param name="thing">The Thing to move.</param>
        /// <param name="destinationThing">The new container to house the Thing.</param>
        /// <param name="goingVia">The going via.</param>
        /// <param name="leavingMessage">The leaving message.</param>
        /// <param name="arrivingMessage">The arriving message.</param>
        public void MoveThing(Thing thing, Thing destinationThing, Thing goingVia, SensoryMessage leavingMessage, SensoryMessage arrivingMessage)
        {
            MovableBehavior movableBehavior = thing.FindBehavior<MovableBehavior>();
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
            return things.TryRemove(thing.Id, out removedThing);
        }

        /// <summary>Calls <see cref="DestroyThing"/> on all things matching the specified condition.</summary>
        /// <param name="condition">The condition.</param>
        /// <returns>The number of things that were successfully destroyed.</returns>
        public int Destroy(Func<Thing, bool> condition)
        {
            var thingsToRemove = things.Values.Where(condition);

            return thingsToRemove.Count(thing => DestroyThing(thing));
        }

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>Starts this system's individual components.</summary>
        public override void Start()
        {
        }

        /// <summary>Stops this system's individual components.</summary>
        public override void Stop()
        {
            cancellationTokenSource.Cancel();
            things.Clear();
        }

        /// <summary>Add or update the ThingManager's cache of Things; should be called when the Id of a Thing is established or changes.</summary>
        /// <param name="oldId">The previous Id of the Thing, or 0 if it has not had an Id yet.</param>
        /// <param name="newId">The new Id of the Thing.</param>
        /// <param name="updatedThing">The updated thing.</param>
        /// <returns>
        /// True if the update was successful. If false, presumably another call won a race
        /// and the caller should either update their reference or try again.
        /// </returns>
        internal bool UpdateThingRegistration(string oldId, string newId, Thing updatedThing)
        {
            Debug.Assert(oldId != newId, "UpdateThingRegistration should not be called when not changing the Thing ID.");
            Debug.Assert(!string.IsNullOrEmpty(newId), "After initialization, a Thing's Id should never become null or empty!");

            if (!string.IsNullOrEmpty(oldId))
            {
                Thing removedThingOldId;
                things.TryRemove(oldId, out removedThingOldId);
            }

            Thing removedThingNewId;
            things.TryRemove(newId, out removedThingNewId);

            return things.TryAdd(newId, updatedThing);
        }

        public void AnnounceLoadedThing(Thing thing)
        {
            ThingLoaded?.Invoke(thing);
        }

        /// <summary>Exporter for MEF.</summary>
        [CoreExports.System(0)]
        public class ThingManagerExporter : SystemExporter
        {
            /// <summary>Gets the singleton system instance.</summary>
            /// <returns>A new instance of the singleton system.</returns>
            public override ISystem Instance => ThingManager.Instance;

            /// <summary>Gets the Type of the singleton system, without instantiating it.</summary>
            /// <returns>The Type of the singleton system.</returns>
            public override Type SystemType => typeof(ThingManager);
        }
    }
}