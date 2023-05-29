//-----------------------------------------------------------------------------
// <copyright file="Thing.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using WheelMUD.Data.Repositories;
using WheelMUD.Interfaces;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Core
{
    /// <summary>A base class that pretty much any interactive thing within the game world is based on.</summary>
    /// <remarks>
    /// NOTE: This class is sealed since the changing of a Thing's behaviors should occur by the
    /// addition/removal/tweaking of the Thing's attached Behaviors.  For instance, a "player" is
    /// a Thing that has a PlayerBehavior (and likely a UserControlledBehavior, and so on).
    /// </remarks>
    [JsonObject(IsReference = true)]
    public sealed class Thing : IThing, IDisposable, IIdentifiable
    {
        /// <summary>The synchronization locking object.</summary>
        private readonly object lockObject = new object();

        /// <summary>The additional context commands available to this thing.</summary>
        private readonly Dictionary<string, ContextCommand> contextCommands = new Dictionary<string, ContextCommand>();

        /// <summary>The stats of this thing.</summary>
        private Dictionary<string, GameStat> stats = new Dictionary<string, GameStat>();

        /// <summary>The game attributes of this thing.</summary>
        private Dictionary<string, GameAttribute> attributes = new Dictionary<string, GameAttribute>();

        /// <summary>The game skills of this thing.</summary>
        private Dictionary<string, GameSkill> skills = new Dictionary<string, GameSkill>();

        /// <summary>The unique ID of this thing.</summary>
        private string id;

        public Thing() : this(null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="Thing"/> class.</summary>
        /// <param name="behaviors">The behaviors.</param>
        public Thing(params Behavior[] behaviors)
        {
            Eventing = new ThingEventing(this);
            KeyWords = new List<string>();
            Behaviors = new BehaviorManager(this);

            if (behaviors != null)
            {
                foreach (var behavior in behaviors)
                {
                    behavior.SetParent(this);
                    Behaviors.Add(behavior);
                }
            }
        }

        /// <summary>Finalizes an instance of the <see cref="Thing"/> class.</summary>
        ~Thing()
        {
            Dispose();
        }

        public ThingEventing Eventing { get; private set; }

        /// <summary>Gets the unique persistence ID of this Thing, or null if we have not acquired one yet.</summary>
        /// <remarks>See 'Id' for further details.</remarks>
        [JsonIgnore]
        public string PersistedId
        {
            get
            {
                lock (lockObject)
                {
                    return (string.IsNullOrEmpty(id) || id.EndsWith('/') || id.EndsWith('|')) ? null : id;
                }
            }
        }

        /// <summary>Gets or sets the unique ID of this Thing.</summary>
        /// <remarks>
        /// A Document DB may expect this property to be cased exactly this way ("Id") for finding the DB identifier.
        /// This property is NOT guaranteed to be unique; in fact it can be given a highly-reused value like "names/"
        /// to indicate that, when persisted, we expect it to become a unique ID with that prefix.
        /// For situations that require a unique ID (or null if not ready), you may need to read PersistenceId instead!
        /// </remarks>
        public string Id
        {
            get
            {
                // Avoid races with retrieving ID while it is in the process of changing.
                lock (lockObject)
                {
                    return id;
                }
            }

            set
            {
                lock (lockObject)
                {
                    if (value != id)
                    {
                        ThingManager.Instance.UpdateThingRegistration(id, value, this);
                        id = value;
                    }
                }
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            // A Thing will save and load with all its behaviors attached, but those behaviors won't necessarily have
            // their Parent property saved in storage since we can reverse-engineer that.
            RepairParentTree();

            // If we have just finished loading this Thing, then it is time to let any listeners know.
            // E.G. there may be a MultipleParentsBehavior whose other half loaded but have been waiting for us too.
            ThingManager.Instance.AnnounceLoadedThing(this);
        }

        /// <summary>Gets or sets the words that can be used to interact with this object.</summary>
        public List<string> KeyWords { get; set; } = new List<string>();

        /// <summary>Gets or sets the name of this thing.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets the full name of this thing.</summary>
        [JsonIgnore]
        public string FullName
        {
            get { return Name; }
        }

        /// <summary>Gets or sets the title of this thing.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets the description of this thing.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Gets or sets the persistence IDs of all children of this Thing which allow persistence.</summary>
        /// <remarks>
        /// Primarily used for persistence to store and restore children without storing the whole sub-documents as
        /// part of this document too.
        /// </remarks>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "For persistence; see remarks.")]
        private string[] ChildrenIds
        {
            get
            {
                return GetPersistableChildren().Select(c => c.Id).ToArray();
            }
            set
            {
                foreach (string childId in value)
                {
                    var child = ThingManager.Instance.FindThing(childId);
                    if (child != null)
                    {
                        Add(child);
                    }
                    else
                    {
                        ThingManager.Instance.QueueLoadChild(this, childId);
                    }
                }
            }
        }

        /// <summary>Gets or sets the parent of this thing, IE a container.</summary>
        /// <remarks>To set a Thing's parent, instead use newParentThing.Add(this) to rig up all relationships correctly.</remarks>
        [JsonIgnore]
        public Thing Parent { get; private set; }

        /// <summary>Gets a list of all parents of this thing, or an empty list if there are none.</summary>
        [JsonIgnore]
        public List<Thing> Parents
        {
            get
            {
                var parents = new List<Thing>();

                var mainParent = Parent;
                if (mainParent != null)
                {
                    parents.Add(mainParent);
                }

                var multipleParentsBehavior = Behaviors.FindFirst<MultipleParentsBehavior>();
                if (multipleParentsBehavior != null)
                {
                    parents.AddRange(multipleParentsBehavior.SecondaryParents);
                }

                return parents;
            }
        }

        /// <summary>Gets or sets a dictionary of the primary stats that apply to this thing.</summary>
        /// <remarks>TODO: Consider removal; should be probably layer on via Behaviors; most Things don't need this...</remarks>
        public Dictionary<string, GameStat> Stats
        {
            get
            {
                lock (lockObject)
                {
                    return stats;
                }
            }

            set
            {
                lock (lockObject)
                {
                    stats = value;
                }
            }
        }

        /// <summary>Gets or sets a dictionary of the attributes that apply to this thing.</summary>
        /// <remarks>TODO: Consider removal; should be probably layer on via Behaviors; most Things don't need this...</remarks>
        public Dictionary<string, GameAttribute> Attributes
        {
            get
            {
                lock (lockObject)
                {
                    return attributes;
                }
            }

            set
            {
                lock (lockObject)
                {
                    attributes = value;
                }
            }
        }

        /// <summary>Gets or sets a dictionary of the game skills that apply to this thing.</summary>
        /// <remarks>TODO: Consider removal; should be probably layer on via Behaviors; most Things don't need this...</remarks>
        public Dictionary<string, GameSkill> Skills
        {
            get
            {
                lock (lockObject)
                {
                    return skills;
                }
            }

            set
            {
                lock (lockObject)
                {
                    skills = value;
                }
            }
        }

        /// <summary>Gets the contextual commands for this Thing.</summary>
        /// <remarks>Reconstruction of our Behaviors should be responsible for recreating their own Context Commands, to avoid temporary commands possibly leaking forever.</remarks>
        [JsonIgnore]
        public Dictionary<string, ContextCommand> Commands
        {
            get
            {
                lock (lockObject)
                {
                    return contextCommands;
                }
            }
        }

        private List<Thing> children = new List<Thing>();

        /// <summary>Gets the children of this Thing as a read-only collection.</summary>
        /// <remarks>To add/remove a child properly, use the Add method.</remarks>
        [JsonIgnore]
        public ReadOnlyCollection<Thing> Children
        {
            get { return children.AsReadOnly(); }
        }

        /// <summary>If true, this Thing persists as a document and loads as a document after later reboots, etc.</summary>
        /// <remarks>
        /// Set false if you want the Thing and everything within it to be thrown away every reboot. (Things like players
        /// can still use other means to save explicitly. If a player loads but the game cannot find the place to put them
        /// by id (because it was not persisted), then the player will placed in the default room instead.
        /// Generally you should leave Persists true, but some example scenarios where you might want it false:
        /// * You want to import a openly-developed area as a DLL that gets exported via CreatorDefinitions.Area, say, to
        ///   keep yourself open to "upgrading" the area through taking a NuGet update. Since the Area will be regenerated
        ///   every time you boot the server, it will be automatically up-to-date.
        /// * You want a dynamically generated area like a maze to be always built at runtime (periodically or every
        ///   reboot) and thus don't have need of persisting the area.
        /// * You want a fully dynamically-generated world generation algorithm to produce a new world each reboot.
        /// You will need special considerations for providing ways for players to cross boundaries between persisting and
        /// non-persisting places, since a persisting place won't be able to persist an exit connected to an auto-generated
        /// place by ID, if that ID is not absolutely static. (So having maze area entryway and final exits generate with a
        /// static Thing ID will help a lot.)
        /// </remarks>
        /// <summary>If true, this world persists as a document and loads as a document after later reboots, etc.</summary>
        /// <remarks>Set false if you want to regenerate the entire game world programmatically, every reboot.</remarks>
        public bool Persists { get; set; } = true;

        /// <summary>Gets or sets the ID of the template this Thing is based on.</summary>
        /// <remarks>TODO: 'set' should be private once the Builders are finished being extracted!</remarks>
        public string TemplateId { get; set; }

        /// <summary>Gets the behavior manager for this item.</summary>
        public BehaviorManager Behaviors { get; private set; }

        // TODO: Only let internal Combine and such alter the Count directly?
        // TODO: All Things may be a stack (when count>0) it's up to us to split 
        // automatically at appropriate times, etc.
        public int Count { get; private set; }

        /// <summary>Gets or sets the string that is prepended to the name.</summary>
        public string SingularPrefix { get; set; }

        /// <summary>Gets or sets the string that is appended to the name.</summary>
        public string PluralSuffix { get; set; }

        /// <summary>Get a string representation of this Thing instance.</summary>
        /// <returns>A string representation of this Thing instance.</returns>
        public override string ToString()
        {
            return string.Format("{0} (ID: {1})", FullName, Id);
        }

        /// <summary>Performs tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            // TODO: Unregister from all things we subscribed to (just the current parent, individual behaviors may differ).
            // TODO: Dispose all our Children and Behaviors too (things should not be disposed lightly).
            if (Parent != null)
            {
                Parent.Remove(this);
            }
        }

        private IEnumerable<Thing> GetPersistableChildren()
        {
            return Children.Where(c => c.Persists && c.FindBehavior<PlayerBehavior>() == null);
        }

        public void Save()
        {
            if (Persists)
            {
                DocumentRepository<Thing>.SaveTree(this, t => t.GetPersistableChildren());
            }
        }

        /// <summary>Allows a caller to determine whether this thing can be detected by something's senses.</summary>
        /// <param name="senses">The sense manager.</param>
        /// <returns>True if detectable by sense, else false.</returns>
        public bool IsDetectableBySense(SenseManager senses)
        {
            return senses.Contains(SensoryType.Sight) && senses[SensoryType.Sight].Enabled;
        }

        /// <summary>Determine whether this Thing can stack with the specified Thing.</summary>
        /// <param name="thing">The thing to check for stacking ability.</param>
        /// <returns>True if the Things can become a single stack, else false.</returns>
        public bool CanStack(Thing thing)
        {
            if (TemplateId == thing.TemplateId && Name == thing.Name && FullName == thing.FullName)
            {
                // TODO: Better logic to see differing properties on housed behaviors, etc...
                if (Behaviors.CanStack(thing.Behaviors))
                {
                    // throw new NotImplementedException();
                }
            }

            return false;
        }

        /// <summary>Clone a new instance of this Thing and its properties, but with a new ID.</summary>
        /// <returns>A new, largely identical instance of the thing.</returns>
        /// <remarks>TODO: Consider replacing with a constructor like "new Thing(existingThing)".</remarks>
        public Thing Clone()
        {
            var newThing = new Thing();
            newThing.CloneProperties(this);
            newThing.TemplateId = Id;
            return newThing;
        }

        /// <summary>Sets the Parent of all of this Thing's children and behaviors to be this Thing. Use sparingly!</summary>
        /// <remarks>
        /// In order for persistence to work without infinite reference loops, the Parent of a Thing is not stored.
        /// As such, this method should be called after restoring a Thing from persistence (and is the ONLY time this
        /// should need to be called).
        /// </remarks>
        public void RepairParentTree()
        {
            lock (lockObject)
            {
                Behaviors.RepairParent(this);
                foreach (var child in children)
                {
                    child.Parent = this;
                    child.RepairParentTree(); // Recurse through their children and so on to repair them as well.
                }
            }
        }

        /// <summary>Clone the properties of the specified existing Thing to this Thing.</summary>
        /// <param name="existingThing">The existing thing to clone from.</param>
        public void CloneProperties(Thing existingThing)
        {
            lock (lockObject)
            {
                lock (existingThing.lockObject)
                {
                    // All Things should be able to clone, and most derived classes should find it sufficient
                    // to allow this base CloneProperties to take care of all the cloning.
                    // TODO: Test this.  Especially if any properties have indexers.
                    // TODO: Make sure this deep-copies things like behaviors.
                    var properties = GetType().GetProperties();

                    foreach (var property in properties)
                    {
                        property.SetValue(this, property.GetValue(existingThing, null), null);
                    }

                    Id = null;
                    Behaviors = (BehaviorManager)existingThing.Behaviors.Clone();
                }
            }
        }

        /// <summary>Finds a child using the predicate passed.</summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The Item found.</returns>
        public Thing FindChild(Func<Thing, bool> predicate)
        {
            lock (lockObject)
            {
                return Children.Where(predicate).FirstOrDefault();
            }
        }

        /// <summary>Finds an item by a full or partial ID or name.</summary>
        /// <param name="searchString">The ID or name to search for.</param>
        /// <returns>The Item found.</returns>
        public Thing FindChild(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return null;
            }

            lock (lockObject)
            {
                // Try to find the item in this collection by seeing if any item ID matches 
                // the string exactly, else if that find call returns null, find any item ID 
                // that starts with the specified string, else if that is null...
                return Children.Where(i => searchString.Equals(i.Id, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() ??
                       Children.Where(i => i.Name.Equals(searchString, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() ??
                       Children.Where(i => i.Name.ToLower().StartsWith(searchString, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() ??
                       Children.Where(i => i.KeyWords.Contains(searchString, StringComparer.OrdinalIgnoreCase)).FirstOrDefault();
            }
        }

        /// <summary>Try to find a thing that is relatively 'local' to this thing.</summary>
        /// <remarks>
        /// IE for a player who is trying to interact with an item or mobile, we want to search 
        /// that player's sub-things (inventory and such) and the player's parent thing's (IE room)
        /// contents for such a target thing too.</remarks>
        /// <param name="searchString">The ID or name to search for.</param>
        /// <returns>The Thing, if found, else null.</returns>
        public Thing FindLocalThing(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return null;
            }

            Thing foundThing = FindChild(searchString);
            if (foundThing == null)
            {
                foundThing = Parent.FindChild(searchString);
            }

            return foundThing;
        }

        /// <summary>Finds all behaviors of the given type amongst our children.</summary>
        /// <typeparam name="T">Behavior type.</typeparam>
        /// <returns>Enumeration of the behaviors.</returns>
        public IEnumerable<T> FindAllChildrenBehaviors<T>() where T : Behavior
        {
            return from child in Children
                   let behavior = child.FindBehavior<T>()
                   select behavior;
        }

        /// <summary>Finds the behavior in the behavior manager.</summary>
        /// <typeparam name="T">Any <see cref="Behavior"/> type.</typeparam>
        /// <returns>A behavior if one is found, otherwise null.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T FindBehavior<T>() where T : Behavior
        {
            return Behaviors.FindFirst<T>();
        }

        /// <summary>Asks the behavior manager whether this thing has the behavior specified in the type parameter.</summary>
        /// <typeparam name="T">Any <see cref="Behavior"/> type.</typeparam>
        /// <returns>True if the behavior was found, otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasBehavior<T>() where T : Behavior
        {
            return Behaviors.FindFirst<T>() != null;
        }

        /// <summary>Attempts to move the target thing to be a child of this thing.</summary>
        /// <param name="thing">The thing we intend to become our child.</param>
        /// <returns>True if fully successful, else false.</returns>
        public bool Add(Thing thing)
        {
            // No two threads may add/remove any combination of the parent/sub-thing at the same time,
            // in order to prevent race conditions resulting in thing-disconnection/duplication/etc.
            // TODO: May need to pick a consistent order to apply the locks (like locking the lowest
            //       alphabetical thing ID's lock first) to prevent possible deadlocks.
            // TODO: The whole MultipleParentsBehavior may be too complicated for our actual use cases,
            //       and we should consider a lighter approach that doesn't track multiple parents, but
            //       simply lets the thing be a child of multiple locations.
            lock (lockObject)
            {
                lock (thing.lockObject)
                {
                    // If the thing already has a parent, ensure we have permission to continue.
                    // Presence of MultipleParentsBehavior means we can always add more parents, but
                    // if it's not present, we have to see if removal would be accepted first.
                    var multipleParentsBehavior = thing.FindBehavior<MultipleParentsBehavior>();
                    RemoveChildEvent removalRequest = null;

                    var oldParent = thing.Parent;
                    if (oldParent != null && multipleParentsBehavior == null)
                    {
                        removalRequest = oldParent.RequestRemoval(thing);
                        if (removalRequest.IsCanceled)
                        {
                            return false;
                        }
                    }

                    var addRequest = RequestAdd(thing);
                    if (addRequest.IsCanceled)
                    {
                        return false;
                    }

                    // If we got this far, both removal (if needed) and add requests were accepted, so 
                    // perform both now and send the confirmation events for any listeners.  Removal is
                    // first, since we don't want to risk accidentally removing from the new parent, etc.
                    if (removalRequest != null)
                    {
                        oldParent.PerformRemoval(thing, removalRequest, multipleParentsBehavior);
                    }

                    PerformAdd(thing, addRequest, multipleParentsBehavior);
                }

                return true;
            }
        }

        /// <summary>Removes the specified thing.</summary>
        /// <param name="thing">The thing.</param>
        /// <returns>True if the thing has successfully been removed, else false.</returns>
        public bool Remove(Thing thing)
        {
            // No two threads may add/remove any combination of the parent/sub-thing at the same time,
            // in order to prevent race conditions resulting in thing-disconnection/duplication/etc.
            lock (lockObject)
            {
                lock (thing.lockObject)
                {
                    if (Children.Contains(thing))
                    {
                        var multipleParentsBehavior = thing.FindBehavior<MultipleParentsBehavior>();
                        var removalRequest = RequestRemoval(thing);
                        return PerformRemoval(thing, removalRequest, multipleParentsBehavior);
                    }
                }
            }

            return false;
        }

        /// <summary>Sets the Thing's Parent. DO NOT USE.</summary>
        /// <remarks>
        /// Generally you should use newParent.Add(newChild) or oldParent.Remove(oldChild) instead.
        /// This function is marked "unsafe" because it only performs one part of the parent-child updates that are usually required.
        /// For example, if you set player.RigParentUnsafe(room), the player object may behave as if it is in the room, but other players and
        /// mobs would not be able to perceive the player as one of the children of the room, because the room's children won't be updated.
        /// </remarks>
        /// <param name="newParent"></param>
        public void RigParentUnsafe(Thing newParent)
        {
            Parent = newParent;
        }

        /// <summary>Adds this Thing to a parent.</summary>
        /// <param name="parent">The new parent.</param>
        /// <returns>True if the item was added to the container.</returns>
        public bool AddTo(Thing parent)
        {
            return parent.Add(this);
        }

        /// <summary>Removes a Thing from the applicable parent(s).</summary>
        public void RemoveFromParents()
        {
            foreach (var parent in Parents)
            {
                parent.Remove(this);
            }
        }

        /// <summary>Adds the given attribute to this <see cref="Thing"/>.</summary>
        /// <param name="gameAttribute">The attribute to be added.</param>
        public void AddAttribute(GameAttribute gameAttribute)
        {
            gameAttribute.Parent = this;
            Attributes.Add(gameAttribute.Name, gameAttribute);
            gameAttribute.OnAdd();
        }

        /// <summary>Removes the given attribute from this <see cref="Thing"/>.</summary>
        /// <param name="gameAttribute">The attribute to be removed.</param>
        public void RemoveAttribute(GameAttribute gameAttribute)
        {
            gameAttribute.Parent = null;
            if (Attributes.ContainsKey(gameAttribute.Name))
            {
                Attributes.Remove(gameAttribute.Name);
            }
            gameAttribute.OnRemove();
        }

        /// <summary>Adds the game stat to this <see cref="Thing"/>.</summary>
        /// <param name="gameStat">The game stat to be added.</param>
        public void AddStat(GameStat gameStat)
        {
            gameStat.Parent = this;
            Stats.Add(gameStat.Name, gameStat);
            gameStat.OnAdd();
        }

        /// <summary>Removes the game stat from this <see cref="Thing"/>.</summary>
        /// <param name="gameStat">The game stat to be removed.</param>
        public void RemoveStat(GameStat gameStat)
        {
            gameStat.Parent = null;
            if (Stats.ContainsKey(gameStat.Name))
            {
                Stats.Remove(gameStat.Name);
            }
            gameStat.OnRemove();
        }

        /// <summary>Combines one (stack of) thing with another (stack of) thing.</summary>
        /// <param name="thing">The thing to add to this (stack).</param>
        /// <returns>The remainder (stack of) Thing if this stack couldn't combine all of the other, else null.</returns>
        private Thing Combine(Thing thing)
        {
            lock (lockObject)
            {
                lock (thing.lockObject)
                {
                    if (!CanStack(thing))
                    {
                        // Return the full original (stack of) thing as the unstacked remainder.
                        return thing;
                    }

                    // TODO: Better stacking: produce a remainder thing and return it, in cases where a maximum stack count
                    //       would be exceeded.  Also, take into account potentially different Behaviors attached to the
                    //       objects in the CanStack method!
                    Count += thing.Count;
                    return null;
                }
            }
        }

        private RemoveChildEvent RequestRemoval(Thing thingToRemove)
        {
            // Create and raise a removal event request.
            var removeChildEvent = new RemoveChildEvent(thingToRemove);
            Eventing.OnMovementRequest(removeChildEvent, EventScope.SelfDown);
            return removeChildEvent;
        }

        private AddChildEvent RequestAdd(Thing thingToAdd)
        {
            // Prepare an add event request, and ensure both the new parent (this) and the 
            // thing itself both get a chance to cancel this request before committing.
            var addChildEvent = new AddChildEvent(thingToAdd, this);
            Eventing.OnMovementRequest(addChildEvent, EventScope.SelfDown);
            thingToAdd.Eventing.OnMovementRequest(addChildEvent, EventScope.SelfDown);
            return addChildEvent;
        }

        /// <summary>Perform removal of the specified thing from our Children.</summary>
        /// <param name="thingToRemove">The thing to remove from our Children.</param>
        /// <param name="removalEvent">The removal event to work with; must have previously been sent as the request.</param>
        /// <param name="multipleParentsBehavior">The multipleParentsBehavior, if applicable.</param>
        /// <returns>True if the thing has been successfully removed, else false.</returns>
        private bool PerformRemoval(Thing thingToRemove, RemoveChildEvent removalEvent, MultipleParentsBehavior multipleParentsBehavior)
        {
            if (removalEvent.IsCanceled)
            {
                return false;
            }

            // Send the removal event.
            Eventing.OnMovementEvent(removalEvent, EventScope.SelfDown);

            // If the thing to remove was in our Children collection, remove it.
            if (children.Contains(thingToRemove))
            {
                children.Remove(thingToRemove);
            }

            // If we don't have a MultipleParentsBehavior, directly remove the one-allowed 
            // parent ourselves, else use the behavior's logic for adjusting the parents.
            if (multipleParentsBehavior == null)
            {
                thingToRemove.Parent = null;
            }
            else
            {
                multipleParentsBehavior.RemoveParent(this);
            }

            return true;
        }

        private bool PerformAdd(Thing thingToAdd, AddChildEvent addEvent, MultipleParentsBehavior multipleParentsBehavior)
        {
            // Our caller will pass us the multipleParentsBehavior of the thing to add since it already had to use it,
            // so reusing it will be fastest, but in debug we can validate that this argument is being used correctly.
            Debug.Assert(multipleParentsBehavior == thingToAdd.FindBehavior<MultipleParentsBehavior>());
            Debug.Assert(!addEvent.IsCanceled, "Should not try to PerformAdd if the add event was canceled.");

            // If an existing thing is stackable with the added thing, combine the new
            // thing into the existing thing instead of simply adding it.
            foreach (Thing currentThing in Children)
            {
                if (thingToAdd.CanStack(currentThing))
                {
                    currentThing.Combine(thingToAdd);
                    return true;
                }
            }

            // The item cannot be combined to an existing stack, so add the item as a child of the specified parent.
            if (!children.Contains(thingToAdd))
            {
                children.Add(thingToAdd);
            }

            // Tell the child who the new parent is too. Via the MultipleParentsBehavior if applicable.
            if (multipleParentsBehavior == null)
            {
                thingToAdd.Parent = this;
            }
            else
            {
                multipleParentsBehavior.AddParent(this);
            }

            Eventing.OnMovementEvent(addEvent, EventScope.SelfDown);
            return true;
        }
    }
}