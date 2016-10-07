//-----------------------------------------------------------------------------
// <copyright file="Thing.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A base class that anything within the world inherits from.
//   Created: October 2006 by Foxedup
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using Raven.Imports.Newtonsoft.Json;
    using WheelMUD.Core.Events;
    using WheelMUD.Interfaces;

    /// <summary>A base class that pretty much any interactive thing within the game world is based on.</summary>
    /// <remarks>
    /// NOTE: This class is sealed since the changing of a Thing's behaviors should occur by the
    /// addition/removal/tweaking of the Thing's attached Behaviors.  For instance, a "player" is
    /// a Thing that has a PlayerBehavior (and likely a UserControlledBehavior, and so on).
    /// </remarks>
    [JsonObject(IsReference = true)]
    public sealed class Thing : IThing, IPersistable, IDisposable
    {
        /// <summary>The synchronization locking object.</summary>
        private readonly object lockObject = new object();

        /// <summary>The additional context commands available to this thing.</summary>
        private Dictionary<string, ContextCommand> contextCommands;

        /// <summary>The stats of this thing.</summary>
        private Dictionary<string, GameStat> stats;

        /// <summary>The game attributes of this thing.</summary>
        private Dictionary<string, GameAttribute> attributes;

        /// <summary>The game skills of this thing.</summary>
        private Dictionary<string, GameSkill> skills;

        /// <summary>The unique ID of this thing.</summary>
        private string id;

        /// <summary>Initializes a new instance of the <see cref="Thing"/> class.</summary>
        /// <param name="behaviors">The behaviors.</param>
        public Thing(params Behavior[] behaviors)
        {
            this.Eventing = new ThingEventing(this);
            this.KeyWords = new List<string>();
            this.Children = new List<Thing>();
            this.Behaviors = new BehaviorManager(this);

            this.Parent = null;
            this.Name = string.Empty;
            this.Title = string.Empty;
            this.Description = string.Empty;

            this.stats = new Dictionary<string, GameStat>();
            this.attributes = new Dictionary<string, GameAttribute>();
            this.skills = new Dictionary<string, GameSkill>();
            this.contextCommands = new Dictionary<string, ContextCommand>();

            if (behaviors != null)
            {
                foreach (var behavior in behaviors)
                {
                    behavior.Parent = this;
                    this.Behaviors.Add(behavior);
                }
            }
        }

        /// <summary>Finalizes an instance of the <see cref="Thing"/> class.</summary>
        ~Thing()
        {
            this.Dispose();
        }

        public ThingEventing Eventing { get; private set; }

        /// <summary>Gets or sets the unique ID of this thing.</summary>
        public string ID
        {
            // The ID should be a unique ID as per the DB, post-persisted.
            // @@@ Thing may also get a TemplateID added as we work out the templating story...
            get
            {
                // Avoid races with retrieving ID while it is in the process of changing.
                lock (this.lockObject)
                {
                    return this.id;
                }
            }

            set
            {
                lock (this.lockObject)
                {
                    if (value != this.id)
                    {
                        ThingManager.Instance.UpdateThingRegistration(this.id, value, this);
                        this.id = value;
                    }
                }
            }
        }

        /// <summary>Gets or sets the words that can be used to interact with this object.</summary>
        public List<string> KeyWords { get; set; }

        /// <summary>Gets or sets the name of this thing.</summary>
        public string Name { get; set; }

        /// <summary>Gets the full name of this thing.</summary>
        public string FullName
        {
            get { return this.Name; }
        }

        /// <summary>Gets or sets the title of this thing.</summary>
        public string Title { get; set; }

        /// <summary>Gets or sets the description of this thing.</summary>
        public string Description { get; set; }

        /// <summary>Gets or sets the parent of this thing, IE a container.</summary>
        [JsonIgnore]
        public Thing Parent { get; set; }

        /// <summary>Gets a list of all parents of this thing, or an empty list if there are none.</summary>
        [JsonIgnore]
        public List<Thing> Parents
        {
            get
            {
                var parents = new List<Thing>();

                var mainParent = this.Parent;
                if (mainParent != null)
                {
                    parents.Add(mainParent);
                }

                var multipleParentsBehavior = this.Behaviors.FindFirst<MultipleParentsBehavior>();
                if (multipleParentsBehavior != null)
                {
                    parents.AddRange(multipleParentsBehavior.SecondaryParents);
                }

                return parents;
            }
        }

        /// <summary>Gets or sets a dictionary of the primary stats that apply to this thing.</summary>
        public Dictionary<string, GameStat> Stats
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.stats;
                }
            }

            set
            {
                lock (this.lockObject)
                {
                    this.stats = value;
                }
            }
        }

        /// <summary>Gets or sets a dictionary of the attributes that apply to this thing.</summary>
        public Dictionary<string, GameAttribute> Attributes
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.attributes;
                }
            }

            set
            {
                lock (this.lockObject)
                {
                    this.attributes = value;
                }
            }
        }

        /// <summary>Gets or sets a dictionary of the game skills that apply to this thing.</summary>
        public Dictionary<string, GameSkill> Skills
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.skills;
                }
            }

            set
            {
                lock (this.lockObject)
                {
                    this.skills = value;
                }
            }
        }

        /// <summary>Gets the contextual commands for this thing.</summary>
        public Dictionary<string, ContextCommand> Commands
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.contextCommands;
                }
            }
        }

        /// <summary>Gets or sets the children of this Thing.</summary>
        public List<Thing> Children { get; set; }

        /// <summary>Gets or sets the ID of the template this Thing is based on.</summary>
        /// <remarks>@@@ TODO: 'set' should be private once the Builders are finished being extracted!</remarks>
        public string TemplateId { get; set; }

        /// <summary>Gets the behavior manager for this item.</summary>
        public BehaviorManager Behaviors { get; private set; }

        // @@@ Only let internal Combine and such alter the Count directly?
        // @@@ TODO: All Things may be a stack (when count>0) it's up to us to split 
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
            return string.Format("{0} (ID: {1})", this.FullName, this.ID);
        }

        /// <summary>Performs tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            // @@@ TODO: Unregister from all things we subscribed to (just the current parent, individual behaviors may differ).
            // @@@ TODO: Dispose all our Children and Behaviors too (things should not be disposed lightly).
        }

        /// <summary>Saves this Thing.</summary>
        public void Save()
        {
            // If this thing is a player, use the player saving code instead of the generic
            // saving code, since players currently have their own DB/persistence concerns.
            var playerBehavior = this.Behaviors.FindFirst<PlayerBehavior>();
            if (playerBehavior != null)
            {
                playerBehavior.SaveWholePlayer();
            }
            else
            {
                // @@@ TODO: If a thing is asked to save, see if it is a child/subchild of a player, and if so, save the player instead?
                // @@@ TODO: Implement saving of core thing -> saving of housed Behaviors too.
                if (this.Parent.HasBehavior<PlayerBehavior>())
                {
                    this.Parent.Behaviors.FindFirst<PlayerBehavior>().SaveWholePlayer();
                }
                else
                {
                    ////throw new NotImplementedException();
                }
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
            if (this.TemplateId == thing.TemplateId && this.Name == thing.Name && this.FullName == thing.FullName)
            {
                // @@@ TODO: Better logic to see differing properties on housed behaviors, etc...
                if (this.Behaviors.CanStack(thing.Behaviors))
                {
                    // throw new NotImplementedException();
                }
            }

            return false;
        }

        /// <summary>Clone a new instance of this Thing and its properties, but with a new ID.</summary>
        /// <returns>A new, largely identical instance of the thing.</returns>
        public Thing Clone()
        {
            var newThing = new Thing();
            newThing.CloneProperties(this);
            return newThing;
        }

        /// <summary>Save the item to the path. Useful for debugging, as well as later for DB persistence.</summary>
        /// <param name="w">The stream that support writing that you should serialize to.</param>
        /// <returns>true on success, false otherwise.</returns>
        public bool SaveAsXml(Stream w)
        {
            // @@@ TODO: Ensure this saves the housed behaviors too.
            if (w == null || !w.CanWrite)
            {
                return false;
            }

            try
            {
                var s = new XmlSerializer(typeof(Thing));
                s.Serialize(w, this.Clone());
                s = null;
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>De-serialize the next object in the stream and re-create the current one in its image.</summary>
        /// <remarks>Uses the internal CloneProperties() for ease of maintenance by developers.</remarks>
        /// <param name="r">The stream to read from.</param>
        /// <returns>True on success, false on error.</returns>
        public bool LoadFromXML(Stream r)
        {
            try
            {
                var s = new XmlSerializer(typeof(Thing));
                var i = (Thing)s.Deserialize(r);
                this.CloneProperties(i);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>Clone the properties of the specified existing thing.</summary>
        /// <param name="existingThing">The existing thing.</param>
        public void CloneProperties(Thing existingThing)
        {
            lock (this.lockObject)
            {
                lock (existingThing.lockObject)
                {
                    // All Items should be cloneable, and most derived classes should find it sufficient 
                    // to allow this base Item.Clone to take care of all the cloning.
                    // @@@ TODO: Test this.  Especially if any properties have indexers.
                    // @@@ TODO: Make sure this deep-copies things like behaviors.
                    var properties = this.GetType().GetProperties();

                    foreach (var property in properties)
                    {
                        property.SetValue(this, property.GetValue(existingThing, null), null);
                    }

                    this.ID = null;
                    this.Behaviors = (BehaviorManager)existingThing.Behaviors.Clone();
                }
            }
        }

        /// <summary>Finds a child using the predicate passed.</summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The Item found.</returns>
        public Thing FindChild(Predicate<Thing> predicate)
        {
            lock (this.lockObject)
            {
                return this.Children.Find(predicate);
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

            string s = searchString.ToLower();

            lock (this.lockObject)
            {
                // Try to find the item in this collection by seeing if any item ID matches 
                // the string exactly, else if that find call returns null, find any item ID 
                // that starts with the specified string, else if that is null...
                // @@@ Test: Does the ID check here work? long.Equals(string)?
                Thing foundThing = this.Children.Find(i => i.ID.Equals(s)) ??
                                   this.Children.Find(i => i.Name.ToLower().Equals(s)) ??
                                   this.Children.Find(i => i.Name.ToLower().StartsWith(s)) ??
                                   this.Children.Find(i => i.KeyWords.Contains(s));

                return foundThing;
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

            Thing foundThing = this.FindChild(searchString);
            if (foundThing == null)
            {
                foundThing = this.Parent.FindChild(searchString);
            }

            return foundThing;
        }

        /// <summary>Finds all child Things that match the predicate.</summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>List of Items.</returns>
        public List<Thing> FindAllChildren(Predicate<Thing> predicate)
        {
            // @@@ TODO: Why are these locking?  If this is to avoid iteration of Children while it may 
            //           also be modified by another thread, it fails to do so, as the user can access/change
            //           the public this.Children list directly.  We may need a private this.children member 
            //           and only allow specific access to children via our public methods which would return
            //           new lists with the appropriate members (instead of sharing the actual list).
            lock (this.lockObject)
            {
                return this.Children.FindAll(predicate);
            }
        }

        /// <summary>Finds all behaviors of the given type amongst our children.</summary>
        /// <typeparam name="T">Behavior type.</typeparam>
        /// <returns>List of behaviors.</returns>
        public List<T> FindAllChildrenBehaviors<T>() where T : Behavior
        {
            lock (this.lockObject)
            {
                var found = new List<T>();

                foreach (Thing thing in this.Children)
                {
                    T behavior = thing.Behaviors.FindFirst<T>();

                    if (behavior != null)
                    {
                        found.Add(behavior);
                    }
                }

                return found;
            }
        }

        /// <summary>Finds the behavior in the behavior manager.</summary>
        /// <typeparam name="T">Any <see cref="Behavior"/> type.</typeparam>
        /// <returns>A behavior if one is found, otherwise null.</returns>
        public T FindBehavior<T>() where T : Behavior
        {
            T behavior = this.Behaviors.FindFirst<T>();

            return behavior;
        }

        /// <summary>Asks the behavior manager whether this thing has the behavior specified in the type parameter.</summary>
        /// <typeparam name="T">Any <see cref="Behavior"/> type.</typeparam>
        /// <returns>True if the behavior was found, otherwise false.</returns>
        public bool HasBehavior<T>() where T : Behavior
        {
            T behavior = this.Behaviors.FindFirst<T>();
            return behavior != null;
        }

        /// <summary>Finds the stat.</summary>
        /// <typeparam name="T">The GameState type to search for.</typeparam>
        /// <returns>The matching GameState, if found, else null.</returns>
        public GameStat FindStat<T>() where T : GameStat
        {
            var statList = new List<GameStat>(this.Stats.Values);
            T stat = statList.OfType<T>().FirstOrDefault();
            return stat;
        }

        /// <summary>Finds the game stat.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The matching GameState, if found, else null.</returns>
        public GameStat FindGameStat(string name)
        {
            GameStat stat;
            this.Stats.TryGetValue(name, out stat);
            return stat;
        }

        /// <summary>Finds the behavior in the behavior manager.</summary>
        /// <param name="name">The name of the attribute to search for.</param>
        /// <returns>A behavior if one is found, otherwise null.</returns>
        public GameAttribute FindGameAttribute(string name)
        {
            GameAttribute attribute;

            this.Attributes.TryGetValue(name, out attribute);

            return attribute;
        }

        /// <summary>Finds the game attribute.</summary>
        /// <typeparam name="T">The GameAttribute type to search for.</typeparam>
        /// <returns>The matching attribute, if found, else null.</returns>
        public T FindGameAttribute<T>() where T : GameAttribute
        {
            var attribList = new List<GameAttribute>(this.Attributes.Values);

            T attribute = attribList.OfType<T>().FirstOrDefault();

            return attribute;
        }

        /// <summary>Finds a specific game skill.</summary>
        /// <typeparam name="T">The type of the GameSkill.</typeparam>
        /// <returns>The GameSkill, if found, or null.</returns>
        public GameSkill FindGameSkill<T>() where T : GameSkill
        {
            var skillList = new List<GameSkill>(this.Skills.Values);
            T skill = skillList.OfType<T>().FirstOrDefault();
            return skill;
        }

        /// <summary>Finds a specific game skill.</summary>
        /// <param name="skillName">Name of the skill.</param>
        /// <returns>The GameSkill, if found, or null.</returns>
        public GameSkill FindGameSkill(string skillName)
        {
            GameSkill skill;
            this.Skills.TryGetValue(skillName, out skill);
            return skill;
        }

        // @@@ All things (world, room, item, mob, etc), should all potentially have sub-things
        //     but it should be up to the code which moves things about to do so intelligently
        //     (IE should not allow adding an Area inside a Room, etc.)  These can be prevented
        //     either explicitly here, or preferably via event request cancellation.
        public bool Add(Thing thing)
        {
            // No two threads may add/remove any combination of the parent/sub-thing at the same time,
            // in order to prevent race conditions resulting in thing-disconnection/duplication/etc.
            lock (this.lockObject)
            {
                lock (thing.lockObject)
                {
                    // If the thing already has a parent, ensure we have permission to continue.
                    // Presence of MultipleParentsBehavior means we can always add more parents, but
                    // if it's not present, we have to see if removal would be accepted first.
                    var multipleParentsBehavior = thing.Behaviors.FindFirst<MultipleParentsBehavior>();
                    RemoveChildEvent removalRequest = null;

                    var oldParent = thing.Parent;
                    if (oldParent != null && multipleParentsBehavior == null)
                    {
                        removalRequest = oldParent.RequestRemoval(thing);
                        if (removalRequest.IsCancelled)
                        {
                            return false;
                        }
                    }

                    var addRequest = this.RequestAdd(thing);
                    if (addRequest.IsCancelled)
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

                    this.PerformAdd(thing, addRequest, multipleParentsBehavior);
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
            lock (this.lockObject)
            {
                lock (thing.lockObject)
                {
                    if (this.Children.Contains(thing))
                    {
                        var multipleParentsBehavior = thing.Behaviors.FindFirst<MultipleParentsBehavior>();
                        var removalRequest = this.RequestRemoval(thing);
                        return this.PerformRemoval(thing, removalRequest, multipleParentsBehavior);
                    }
                }
            }

            return false;
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
            foreach (var parent in this.Parents)
            {
                parent.Remove(this);
            }
        }

        /// <summary>Adds the given attribute to this <see cref="Thing"/>.</summary>
        /// <param name="gameAttribute">The attribute to be added.</param>
        public void AddAttribute(GameAttribute gameAttribute)
        {
            gameAttribute.Parent = this;

            this.Attributes.Add(gameAttribute.Name, gameAttribute);

            gameAttribute.OnAdd();
        }

        /// <summary>Removes the given attribute from this <see cref="Thing"/>.</summary>
        /// <param name="gameAttribute">The attribute to be removed.</param>
        public void RemoveAttribute(GameAttribute gameAttribute)
        {
            gameAttribute.Parent = null;

            if (this.Attributes.ContainsKey(gameAttribute.Name))
            {
                this.Attributes.Remove(gameAttribute.Name);
            }

            gameAttribute.OnRemove();
        }

        /// <summary>Adds the game stat to this <see cref="Thing"/>.</summary>
        /// <param name="gameStat">The game stat to be added.</param>
        public void AddStat(GameStat gameStat)
        {
            gameStat.Parent = this;
            this.Stats.Add(gameStat.Name, gameStat);
            gameStat.OnAdd();
        }

        /// <summary>Removes the game stat from this <see cref="Thing"/>.</summary>
        /// <param name="gameStat">The game stat to be removed.</param>
        public void RemoveStat(GameStat gameStat)
        {
            gameStat.Parent = null;

            if (this.Stats.ContainsKey(gameStat.Name))
            {
                this.Stats.Remove(gameStat.Name);
            }

            gameStat.OnRemove();
        }

        /// <summary>Combines one (stack of) thing with another (stack of) thing.</summary>
        /// <param name="thing">The thing to add to this (stack).</param>
        /// <returns>The remainder (stack of) Thing if this stack couldn't combine all of the other, else null.</returns>
        private Thing Combine(Thing thing)
        {
            lock (this.lockObject)
            {
                lock (thing.lockObject)
                {
                    if (!this.CanStack(thing))
                    {
                        // Return the full original (stack of) thing as the unstacked remainder.
                        return thing;
                    }

                    // @@@ TODO: better stacking: produce a remainder thing and return it, in
                    //     cases where a maximum stack count would be exceeded.  Also, take into
                    //     account potentially different Behaviors attached to the objects in 
                    //     the CanStack method!
                    this.Count += thing.Count;
                    return null;
                }
            }
        }

        private RemoveChildEvent RequestRemoval(Thing thingToRemove)
        {
            // Create and raise a removal event request.
            var removeChildEvent = new RemoveChildEvent(thingToRemove);
            this.Eventing.OnMovementRequest(removeChildEvent, EventScope.SelfDown);
            return removeChildEvent;
        }

        private AddChildEvent RequestAdd(Thing thingToAdd)
        {
            // Prepare an add event request, and ensure both the new parent (this) and the 
            // thing itself both get a chance to cancel this request before committing.
            var addChildEvent = new AddChildEvent(thingToAdd, this);
            this.Eventing.OnMovementRequest(addChildEvent, EventScope.SelfDown);
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
            if (removalEvent.IsCancelled)
            {
                return false;
            }

            // Send the removal event.
            this.Eventing.OnMovementEvent(removalEvent, EventScope.SelfDown);

            // If the thing to remove was in our Children collection, remove it.
            if (this.Children.Contains(thingToRemove))
            {
                this.Children.Remove(thingToRemove);
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
            if (addEvent.IsCancelled)
            {
                return false;
            }

            // If an existing thing is stackable with the added thing, combine the new
            // thing into the existing thing instead of simply adding it.
            foreach (Thing currentThing in this.Children)
            {
                if (thingToAdd.CanStack(currentThing))
                {
                    currentThing.Combine(thingToAdd);
                    return true;
                }
            }

            // The item cannot be stacked so add the item to the specified parent.
            if (!this.Children.Contains(thingToAdd))
            {
                this.Children.Add(thingToAdd);
            }

            // If we don't have a MultipleParentsBehavior, directly set the one-allowed 
            // parent ourselves, else use the behavior's logic for setting parents.
            if (multipleParentsBehavior == null)
            {
                thingToAdd.Parent = this;
            }
            else
            {
                multipleParentsBehavior.AddParent(this);
            }

            this.Eventing.OnMovementEvent(addEvent, EventScope.SelfDown);
            return true;
        }
    }
}