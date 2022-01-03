﻿//-----------------------------------------------------------------------------
// <copyright file="AAA_TEMP.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   TEMP CONTENT FROM REMOVED FILES - FOR TEMPORARY SLN SEARCHABILITY ONLY WHILE
//   TRANSITIONING CODE TO NEW BEHAVIORS, EVENTS, MEF, ETC.
//   DO NOT REMOVE UNTIL EACH PART IS DEFINITELY REPLACED/PROVEN WITH BEHAVIORS ETC.
//   (Or remove when replacement work is planned and tracked properly in GitHub.)
// </summary>
//-----------------------------------------------------------------------------

// Extracted from Things.cs:
/* TODO: Replace with a ProvidesConsumableBehavior
/// <summary>An interface defining a ConsumableProvider.</summary>
public interface IConsumableProvider : IItem
{
    /// <summary>Gets the resource type.</summary>
    string ResourceType { get; }

    /// <summary>Gets the number of resources left.</summary>
    int NumberOfResources { get; }

    /// <summary>Huh? Chop is only for wood...?</summary>
    /// <returns>A Consumable yielded by the action.</returns>
    IConsumable Chop();
}*/

// Removed ITimeSystem.cs:
/*
namespace WheelMUD.Interfaces
{
    /// <summary>An interface defining a TimeSystem.</summary>
    public interface ITimeSystem : ISystem
    {
        /// <summary>Gets the current time.</summary>
        string Time { get; }

        /// <summary>Gets the current day.</summary>
        string Day { get; }

        /// <summary>Gets the current month.</summary>
        string Month { get; }

        /// <summary>Gets the current year.</summary>
        string Year { get; }

        /// <summary>Gets a formated string of the current date and time.</summary>
        string Now { get; }
    }
}
*/

// Removed EffectBase.cs:
/*
namespace WheelMUD.Effects
{
    /// <summary>Effect base class.</summary>
    public abstract class EffectBase : IEffect
    {
        /// <summary>The time the effect completes.</summary>
        private DateTime completesAt;

        /// <summary>The event that is raised to signal back that the effect has expired and needs to be removed from the collection.</summary>
        public event EffectElapsedEventHandler EffectElapsed;

        /// <summary>Gets or sets the host that this effect is going to apply to.</summary>
        public Thing Host { protected get; set; }

        /// <summary>Gets or sets the thing that created/instigated this effect on the host.</summary>
        public Thing Creator { protected get; set; }

        /// <summary>Gets the time left before the remove method is called.</summary>
        public TimeSpan RemainingTime
        {
            get
            {
                if (completesAt > DateTime.Now)
                {
                    return completesAt.Subtract(DateTime.Now);
                }

                return new TimeSpan(0, 0, 0, 0, 0);
            }
        }

        /// <summary>Gets or sets the duration.</summary>
        /// <value>The duration.</value>
        protected Timer Duration { get; set; }

        /// <summary>
        /// Called to determine if the effect is allowed to be applied.
        /// Default implementation always returns true, so override this
        /// to add any functionality.
        /// </summary>
        /// <param name="host">The thing that the effect is being targeted at</param>
        /// <param name="creator">The thing that is creating the effect</param>
        /// <returns>Whether the effect is permitted</returns>
        public virtual bool IsAllowed(Thing host, Thing creator)
        {
            return true;
        }

        /// <summary>Begins this effect's effect.</summary>
        /// <remarks>This can be used to increase stats or whatever. Always call the base when overriding.</remarks>
        /// <param name="duration">The duration for the effect.</param>
        public virtual void Apply(TimeSpan duration)
        {
            Duration = new Timer(TickElapsed, null, (int)duration.TotalMilliseconds, Timeout.Infinite);
            completesAt = DateTime.Now.Add(duration);
        }

        /// <summary>The method that is called when an effect is to be removed.</summary>
        /// <remarks>It is effectively the cleanup operation IE reduce stats to normal level.</remarks>
        public abstract void Remove();

        /// <summary>Called when the effect expires.</summary>
        /// <param name="state">TODO: What is this?</param>
        protected abstract void TickElapsed(object state);

        /// <summary>Raises the Effect Elapsed event.</summary>
        protected void RaiseEffectElapsed()
        {
            Duration.Change(Timeout.Infinite, Timeout.Infinite);
            if (EffectElapsed != null)
            {
                EffectElapsed(this);
            }
        }
    }
}
*/

// Removed EffectsManager.cs:
/*
namespace WheelMUD.Effects
{
    /// <summary>The effects manager manages the effects that a "thing" can currently have influencing them.</summary>
    public class EffectsManager
    {
        /// <summary>The host that this effects manager is attached to.</summary>
        private readonly Thing host;
        
        /// <summary>The list of effects currently attached to this host.</summary>
        private readonly Dictionary<string, IEffect> effects = new Dictionary<string, IEffect>();

        /// <summary>The effects found at runtime in the assembly that can be used.</summary>
        private static readonly List<Type> availableEffects = new List<Type>(); 

        /// <summary>The synchronization locking object.</summary>
        private readonly object lockObject = new object();

        /// <summary>Initializes static members of the EffectsManager class.</summary>
        static EffectsManager()
        {
            GatherEffects();
        }

        /// <summary>Initializes a new instance of the EffectsManager class.</summary>
        /// <param name="thing">The host of the effects manager.</param>
        public EffectsManager(Thing thing)
        {
            host = thing;
        }

        /// <summary>Gets the count of the number of effects effecting this Thing.</summary>
        /// <returns>The number of effects in the effects manager.</returns>
        public int Count
        {
            get { return effects.Count; }
        }

        /// <summary>Tries to create a new effect and add it to the collection of effects applied to this thing.</summary>
        /// <remarks>Modeled after the .NET framework pattern for methods that may fail, e.g. int.TryParse and Dictionary.TryGetValue.</remarks>
        /// <typeparam name="T">The type of the effect to be created.</typeparam>
        /// <param name="creator">The creator of the effect.</param>
        /// <param name="effect">An instance of the requested effect or null, if the effect is not allowed.</param>
        /// <returns>Whether the effect could be created or not</returns>
        public bool TryCreateEffect<T>(Thing creator, out T effect) where T : class, IEffect, new()
        {
            // instantiated to avoid confusing anyone who expects members to be initialised within IsAllowed
            effect = new T();

            if (availableEffects.Contains(effect.GetType()))
            {
                if (effect.IsAllowed(host, creator))
                {
                    effect.Host = host;
                    effect.Creator = creator;

                    effect.EffectElapsed += EffectElapsed;

                    lock (lockObject)
                    {
                        effects.Add(Guid.NewGuid().ToString(), effect);
                    }

                    return true;
                }
                else
                {
                    // contracted to return null when the effect is not allowed, 
                    // unless we start using the Null-Object Pattern
                    effect = null;
                    return false;
                }
            }

            throw new Exception("The requested effect is not available");
        }

        /// <summary>Gathers the different effects in the project into a collection so we can use them.</summary>
        private static void GatherEffects()
        {
            Assembly effectsDll = Assembly.GetAssembly(typeof(EffectsManager));

            Type[] types = effectsDll.GetTypes();

            foreach (Type type in types)
            {
                if (type.GetInterface(typeof(IEffect).Name) != null)
                {
                    availableEffects.Add(type);
                }
            }
        }

        /// <summary>Removes an effect when it has elapsed</summary>
        /// <param name="effect">The effect to remove.</param>
        private void EffectElapsed(IEffect effect)
        {
            CancelEffect(effect);
        }

        /// <summary>Cancels an effect.</summary>
        /// <param name="effect">The effect to cancel.</param>
        private void CancelEffect(IEffect effect)
        {
            lock (lockObject)
            {
                effects.Remove(effect.Name.ToLower());
            }
        }
    }
}*/

// Removed DoorOpenReaction.cs:
/* TODO: If desired to re-implement, consider as SuctionDoorBehavior or whatnot.
namespace WheelMUD.EventReactions
{
    /// <summary>Example of a reaction script, this sucks the player that opened a door out of the room and through the exit.</summary>
    public class DoorOpenReaction : IEventReaction
    {
        /// <summary>The main execution point for reacting to an event.</summary>
        /// <param name="theEvent">The event to react to</param>
        /// <param name="sender">The thing executing this reaction</param>
        public void Execute(IEvent theEvent, Thing sender)
        {
            ////// We only want to consume openclose events.
            ////if (theEvent.GetType().Name == "OpenCloseEvent" && theEvent.Noun == "opens")
            ////{
            ////    // It is a door that is consuming this event.
            ////    // Check that the door is actually in an exit.
            ////    if (sender.Parent.GetType().Name == "Exit")
            ////    {
            ////        IExit exit = (IExit)sender.Parent;
                    
            ////        // Check that the thing that generated the event was a player
            ////        if (theEvent.ActiveThing.GetType().Name == "Player")
            ////        {
            ////            IPlayer player = (IPlayer)theEvent.ActiveThing;
            ////            string direction = GetDirectionOfExit(player, exit);
            ////            if (direction != string.Empty)
            ////            {
            ////                Room oldRoom = player.Parent;

            ////                if (player.MoveInDirection(direction))
            ////                {
            ////                    // Generate our leaving event
            ////                    MoveEvent leaveEv = new MoveEvent();
            ////                    leaveEv.EnteringRoom = player.Parent;
            ////                    leaveEv.ActiveThing = player;
            ////                    leaveEv.LeavingRoom = oldRoom;
            ////                    leaveEv.Noun = "is sucked out of the room";

            ////                    oldRoom.EventBroadcaster.Broadcast(leaveEv);

            ////                    // Generate our entering event
            ////                    MoveEvent enterEv = new MoveEvent();
            ////                    enterEv.EnteringRoom = player.Parent;
            ////                    enterEv.ActiveThing = player;
            ////                    enterEv.LeavingRoom = oldRoom;
            ////                    enterEv.Noun = "is sucked into the room";

            ////                    player.Parent.EventBroadcaster.Broadcast(enterEv);

            ////                    player.Controller.Write("You are sucked through the door");
            ////                    player.Controller.Write(player.Look());
            ////                }
            ////            }
            ////        }
            ////    }
            ////}
        }

        /// <summary>Gets the direction of the exit passed in relation to the player.</summary>
        /// <param name="player">The player we are checking.</param>
        /// <param name="exit">The exit we are checking.</param>
        /// <returns>A string containing the direction of the exit.</returns>
        private static string GetDirectionOfExit(Thing player, Thing exit)
        {
            // TODO: FIX?
            //foreach (KeyValuePair<string, Exit> kvp in player.Parent.Exits)
            //{
            //    if (kvp.Value.Door.Id == exit.Door.Id)
            //    {
            //        return kvp.Key;
            //    }
            //}
            
            return string.Empty;
        }
    }
}
*/

// Removed Item.cs:
/*
namespace WheelMUD.Universe
{
    /// <summary>The base class for an item, an item can inherit from this object rather than implement Item and then override bits.</summary>
    public class Item : Thing
    {
        /// <summary>Initializes a new instance of the Item class.</summary>
        public Item()
            : base()
        {
            Id = 0;
            KeyWords = new List<string>();
            SingularPrefix = string.Empty;
            PluralSuffix = string.Empty;
            BehaviorManager = new BehaviorManager(this);
            SingularPrefix = "a ";
            PluralSuffix = "s";
        }

        /// <summary>Initializes a new instance of the Item class, based on the specified item template.</summary>
        /// <param name="itemTemplateID">The item template whose data we are to base the item upon.</param>
        protected Item(long itemTemplateID)
            : this()
        {
        }

        /// <summary>Gets or sets the item type.</summary>
        public long MudObjectTypeID { get; set; }

        /// <summary>Gets the name with prepended with its prefix and appended with its suffix.</summary>
        public override string FullName
        {
            get { return string.Format("{0} {1} {2}", SingularPrefix, Name, PluralSuffix).Trim(); }
        }

        /* TODO: Make base thing save then extract this example
        /// <summary>Saves the item record.  Written this way to make sure it isn't called directly.</summary>
        public override void Save()
        {
            ItemRepository itemRepository = new ItemRepository();

            if (DataRecord.ID == 0)
            {
                itemRepository.Add(DataRecord);

                Id = DataRecord.ID;
            }
            else
            {
                itemRepository.Update(DataRecord);
            }

            BehaviorManager.ItemId = Id;
            BehaviorManager.Save();
        }
    }
}*/

// Removed RelayEvent.cs:
// <summary>
// TODO: A temporary example of a reaction script, that simply broadcasts to entities in the room
// that it has detected an event.
// </summary>
/* TODO: Build a RelaySenseBehavior or whatnot...
namespace WheelMUD.EventReactions
{
    /// <summary>Example of a reaction script, that simply broadcasts to entities in the room that it has detected an event.</summary>
    public class RelayEvent : IEventReaction
    {
        /// <summary>The main execution point for reacting to an event.</summary>
        /// <param name="theEvent">The event to react to.</param>
        /// <param name="sender">The thing executing this reaction.</param>
        public void Execute(IEvent theEvent, Thing sender)
        {
            ////IListeningDevice device = (IListeningDevice)sender;
            ////Entity owner = bridge.Players.FindPlayer(device.OwnerName.ToLower(), true);
            ////if (owner != null)
            ////{
            ////    owner.Controller.Write("Listening Device detected event : " + theEvent.Noun);
            ////}

            foreach (Thing thing in sender.Parent.Children)
            {
                // TODO: Use EntityBehavior or whatnot instead of 'as Entity'
                var entity = thing as Entity;
                if (entity != null)
                {
                    entity.Controller.Write("Listening Device detected event : " + theEvent.Name);
                }
            }
        }
    }
}*/

// Removed ListeningDevice.cs:
// <summary>TODO: A temporary example of an item that responds to world events.</summary>
/*
namespace WheelMUD.Universe.Things
{
    /// <summary>TODO: A temporary example of an item that responds to world events.</summary>
    TODO: Implement as a more generic RelaysSenseBehavior or whatnot...
    public class ListeningDevice : Item
    {
        public ListeningDevice()
            : base()
        {
            OwnerName = string.Empty;
        }

        /// <summary>Initializes a new instance of the ListeningDevice class.</summary>
        internal ListeningDevice()
            : base(new ItemRecord()
            {
                Description = "A tiny listening device",
                Name = "listening device",
                KeyWords = new List<string> { "device", "listening device", "listeningdevice", "bug" },
                SingularPrefix = string.Empty,
                PluralSuffix = string.Empty,
                Reactions = new List<string>()
            })
        {
            OwnerName = string.Empty;
        }

        /// <summary>Gets or sets the owner name.</summary>
        public string OwnerName { get; set; }
    }
}*/

// Removed StackableItem.cs:
/*
namespace WheelMUD.Universe.Things
{
    /// <summary>Base class for Stackable items.</summary>
    public abstract class StackableItem : Item, IStackableItem
    {
        /// <summary>Gets the full name of the stack of items.</summary>
        public override string FullName
        {
            get
            {
                if (Count > 1)
                {
                    return string.Format("{0}{1}{2}", SingularPrefix, Name, PluralSuffix);
                }

                return base.FullName;
            }
        }

        /// <summary>Gets or sets the prefix for this item stack.</summary>
        public override string SingularPrefix
        {
            get
            {
                if (Count == 1)
                {
                    return base.SingularPrefix;
                }

                return Count + " ";
            }

            set
            {
                base.SingularPrefix = value;
            }
        }

        /// <summary>Gets or sets the suffix for this item stack.</summary>
        public override string PluralSuffix
        {
            get
            {
                if (Count == 1)
                {
                    return base.PluralSuffix;
                }

                return "s";
            }

            set
            {
                base.PluralSuffix = value;
            }
        }

        /// <summary>Determine if the specified stackable item can stack with this stackable item.</summary>
        /// <param name="stackableItem">The stackable item to check for stackability.</param>
        /// <returns>True if the items can be combined/stacked, else false.</returns>
        public bool CanStack(IStackableItem stackableItem)
        {
            if (TemplateID != null && stackableItem.TemplateID == TemplateID)
            {
                return true;
            }

            return false;
        }


        /// <summary>Remove an amount of items from this stack, when removing from a container.</summary>
        /// <param name="numberToRemove">The quantity of item to remove.</param>
        /// <returns>True if successful, else false.</returns>
        /// <remarks>ATM, if numberToRemove is greater than Count, we remove the whole stack and return success.
        /// This might not make sense for some actions, IE dealing with Currency stackd during shop buying 
        /// operations; one would have to add explicit checks for Count prior to RemoveFromContainer.</remarks>
        public bool RemoveFromContainer(long numberToRemove)
        {
            if (numberToRemove < Count)
            {
                IStackableItem stackableItem = SplitStack(Count - numberToRemove);
                IItemCollection container = Container;
                RemoveFromContainer();
                container.AddItem(stackableItem);
                return true;
            }

            return RemoveFromContainer();
        }

        // RemoveFromContainer() is not overrided because if no count is specified,
        // then the whole item (stack) is to be removed anyway.

        /// <summary>Splits the stack into two stacks; creating a new stack and removing part from the original.</summary>
        /// <param name="numberToSplitOff">How many items to split into the new stack.</param>
        /// <returns>The new stack of items.</returns>
        private IStackableItem SplitStack(long numberToSplitOff)
        {
            // Ensure that we can't make negative count on either item.  Ensure the null
            // return case is handled anywhere SplitStack is used if count was not already
            // verified by the caller.
            if (numberToSplitOff >= Count)
            {
                return null;
            }

            StackableItem newStack = (StackableItem)Clone();
            newStack.Count = numberToSplitOff;
            Count -= numberToSplitOff;
            return newStack;
        }
    }
}
*/

// Removed Potion.cs:
// <summary>
//   A potion item which creates effects when sipped/quaffed.
//   A Potion is now an easy way to work with an Item whose primary purpose is 
//   to house PotionItemBehavior;  note that other items which are not primarily 
//   considered "potions" could also have PotionItemBehavior to achieve the 
//   same functionality as normal potions.
// </summary>
/*
namespace WheelMUD.Universe.Things
{
    /// <summary>A potion item which creates effects when sipped/quaffed.</summary>
    public class Potion : Item, ICloneableItem
    {
        /// <summary>A reference to the potion item behavior for this item.</summary>
        private PotionItemBehavior potionBehavior;
        
        /// <summary>Initializes a new instance of the Potion class.</summary>
        /// <param name="ids">The item data structure to base the new item upon.</param>
        public Potion(ItemRecord ids)
            : base(ids)
        {            
            // Keep a reference of this item's potion behavior.
            potionBehavior = BehaviorManager.FindFirst<PotionItemBehavior>();
            if (potionBehavior == null)
            {
                // TODO: Loud warning if a Potion doesn't have an associated PotionItemBehavior, 
                //       but for now, until DAL reflects Behaviors properly, we'll default a test one.
                potionBehavior = new PotionItemBehavior();
                potionBehavior.MaxSips = 5;
                potionBehavior.SipsLeft = 2;
                potionBehavior.PotionType = "health";
                potionBehavior.Modifier = 100;
                potionBehavior.Duration = new TimeSpan(0, 0, 0, 15);
            }

            base.Description = "A " + PotionType + " potion.  It appears to have around %numSipsLeft% sips left";
        }

        /// <summary>Ges or sets the description of this potion.</summary>
        public override string Description
        {
            get { return base.Description.Replace("%numSipsLeft%", SipsLeft.ToString()); }
            set { base.Description = value; }
        }
    }
}*/

// Removed Container.cs:
/*
namespace WheelMUD.Universe.Things
{
    /// <summary>A base item that can be used for bags and such.</summary>
    public class Container : AbstractContainer
    {
        /// <summary>Gets or sets the description of this container.</summary>
        public new string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (OpenState == OpenState.Open)
                {
                    sb.AppendLine(base.Description);

                    if (HoldsLiquid)
                    {
                        sb.AppendFormat("It appears that this {0} will hold liquids.", Name);
                        sb.AppendAnsiLine();
                    }

                    if (Count > 0)
                    {
                        sb.AppendLine("Inside, you see:");
                        foreach (Item item in container)
                        {
                            sb.Append(item.Id.ToString().PadRight(20));
                            sb.AppendLine(item.Description);
                        }
                    }
                    else
                    {
                        sb.AppendFormat("The {0} ({1}) is empty", Name, Id);
                        sb.AppendAnsiLine();
                    }
                }
                else
                {
                    sb.AppendLine(base.Description + " (closed)");

                    if (HoldsLiquid)
                    {
                        sb.AppendFormat("It appears that this {0} will hold liquids.", Name);
                        sb.AppendAnsiLine();
                    }
                }

                return sb.ToString();
            }

            set
            {
                base.Description = value;
            }
        }

        /// <summary>Subscribe to the specified broadcaster.</summary>
        /// <param name="broadcaster">The broadcaster to be subscribed to.</param>
        public override void Subscribe(IEventBroadcaster broadcaster)
        {
            // We need to subscribe each of our items to the broadcaster,
            // as well as this item.
            broadcaster.Subscribe(this);
            foreach (Item item in container)
            {
                item.Subscribe(broadcaster);
            }
        }

        /// <summary>Unsubscribe from the specified broadcaster.</summary>
        /// <param name="broadcaster">The broadcaster to be unsubscribed from.</param>
        public override void UnSubscribe(IEventBroadcaster broadcaster)
        {
            // We need to unsubscribe each of our items to the broadcaster.
            // As well as this item
            broadcaster.UnSubscribe(this);
            foreach (Item item in container)
            {
                item.UnSubscribe(broadcaster);
            }
        }
    }
}*/

// Removed AbstractContainer.cs:
/*
namespace WheelMUD.Universe.Things
{
    /// <summary>A base item that can be used for bags and such.</summary>
    public class AbstractContainer : Item
    {
        /// <summary>Private container item behavior object</summary>
        private ContainerBehavior containerBehavior;

        /// <summary>Initializes a new instance of the AbstractContainer class.</summary>
        /// <param name="ids">Item Data Structure.</param>
        internal AbstractContainer(IItemDataStructure ids) :
            base(ids)
        {
            container = new ItemCollection(this);
            containerBehavior = BehaviorManager.FindFirst<ContainerBehavior>();
            if (containerBehavior == null)
            {
                containerBehavior = new ContainerBehavior()
                {
                    Id = 0,
                    HoldsLiquid = false,
                    IsClosable = true,
                    IsOpen = true,
                    Volume = 0,
                    VolumeUnitOfMeasurement = null
                };

                BehaviorManager.Add(containerBehavior);
            }
        }

        /// <summary>Initializes a new instance of the AbstractContainer class.</summary>
        /// <param name="ids">Item Data Structure.</param>
        internal AbstractContainer(ItemRecord ids) :
            base(ids)
        {
            container = new ItemCollection(this);
            containerBehavior = BehaviorManager.FindFirst<ContainerBehavior>();           
            if (containerBehavior == null)
            {
                containerBehavior = new ContainerBehavior()
                                             {
                                                 Id = 0,
                                                 HoldsLiquid = false,
                                                 IsClosable = true,
                                                 IsOpen = true,
                                                 Volume = 0,
                                                 VolumeUnitOfMeasurement = null
                                             };

                BehaviorManager.Add(containerBehavior);
            }
        }

        /// <summary>Gets the count of items in the container.</summary>
        public int Count
        {
            get { return container.Count; }
        }

        /// <summary>Gets or sets a value indicating whether the thing is open or not.</summary>
        new public OpenState OpenState
        {
            get
            {
                if (containerBehavior != null)
                {
                    if (containerBehavior.IsOpen)
                    {
                        return OpenState.Open;
                    }
                 
                    return OpenState.Closed;
                }

                return OpenState.Open;
            }

            set
            {
                if (containerBehavior != null)
                {
                    if (value == OpenState.Open)
                    {
                        containerBehavior.IsOpen = true;
                    }
                    else
                    {
                        containerBehavior.IsOpen = false;
                    }
                }
            }
        }

        /// <summary>Gets or sets a reference to the container item behavior for this item.</summary>
        protected ContainerBehavior ContainerBehavior
        {
            get
            {
                return containerBehavior;
            }

            set
            {
                if (containerBehavior != null)
                {
                    BehaviorManager.ManagedBehaviors.Remove(containerBehavior);
                }

                containerBehavior = value;
                BehaviorManager.Add(value);
            }
        }

        /// <summary>Returns an item from the collection.</summary>
        /// <param name="index">Index of the item that is requested.</param>
        public Item this[int index]
        {
            get { return container[index]; }
        }

        /// <summary>Subscribe to the specified broadcaster.</summary>
        /// <param name="broadcaster">The broadcaster to be subscribed to.</param>
        public override void Subscribe(IEventBroadcaster broadcaster)
        {
            // We need to subscribe each of our items to the broadcaster,
            // as well as this item.
            broadcaster.Subscribe(this);
            foreach (Item item in ContainerBehavior)
            {
                item.Subscribe(broadcaster);
            }
        }

        /// <summary>Unsubscribe from the specified broadcaster.</summary>
        /// <param name="broadcaster">The broadcaster to be unsubscribed from.</param>
        public override void UnSubscribe(IEventBroadcaster broadcaster)
        {
            // We need to unsubscribe each of our items to the broadcaster.
            // As well as this item
            broadcaster.UnSubscribe(this);
            foreach (Item item in ContainerBehavior)
            {
                item.UnSubscribe(broadcaster);
            }
        }
    }
}
*/

// Removed Consumable.cs:
/*
namespace WheelMUD.Universe.Things
{
    /// <summary>A consumable. TODO: Replace with Items which merely have a ConsumableBehavior.</summary>
    public class Consumable : Thing
    {
        /// <summary>The type of consumable this Thing is.</summary>
        private ConsumableType consumableType;

        /// <summary>Initializes a new instance of the Consumable class.</summary>
        public Consumable()
            : base()
        {
        }

        /// <summary>Initializes a new instance of the Consumable class.</summary>
        /// <param name="consumableType">TODO: DESCRIBE</param>
        public Consumable(ConsumableType consumableType)
            : base()
        {
            consumableType = consumableType;
            Description = string.Format("A piece of {0}", consumableType.ToString());
            IsAdornment = false;
            Name = consumableType.ToString();

            // TODO: Implement.
        }
    }
}
*/

// Removed Tree.cs:
/*
namespace WheelMUD.Universe.Things
{
    /// <summary>A tree that can be chopped for resources.</summary>
    public class Tree : Thing
    {
        /// <summary>Initializes a new instance of the Tree class.</summary>
        /// <param name="numberResources">Number of resources for the tree to start with.</param>
        public Tree(int numberResources)
            : base()
        {
            NumberOfResources = numberResources;
            ResourceType = string.Empty;
        }

        /// <summary>Prevents a default instance of the Tree class from being created.</summary>
        private Tree()
            : base()
        {
            ////IItemDataStructure ids = new ItemDataStructure
            ////{
            ////    Description = "A tree",
            ////    Name = "tree",
            ////    Nouns = new List<string> { "tree" },
            ////    Prefix = "a",
            ////    Suffix = string.Empty,
            ////    Reactions = new List<string>()
            ////};
        }

        /// <summary>Gets the resource type of this tree.</summary>
        public string ResourceType { get; private set; }

        /// <summary>Gets the number of resources left in this tree.</summary>
        public int NumberOfResources { get; private set; }

        /// <summary>Override to stop you from picking a tree up.</summary>
        /// <returns>False to indicate you can't get this object.</returns>
        // TODO: Instead, a Tree should be given a CannotPickUpBehavior or whatnot (by default), as it 
        //       should be possible to have a potted baby Tree or whatnot that you CAN pick up.  Maybe
        //       said tree is still choppable but has a very small yield? (but that's beside the point)
        //public override bool RemoveFromContainer()
        //{
        //    return false;
        //}

        /// <summary>Chop at the tree to extract a Consumable from it.</summary>
        /// <returns>A new instance of a Consumable that came from the tree.</returns>
        public Thing Chop()
        {
            if (NumberOfResources > 0)
            {
                NumberOfResources--;
                return new Consumable(ConsumableType.Wood);
            }
            
            return null;
        }
    }
}
*/

// Removed ExitEnd.cs:
/*
namespace WheelMUD.Universe.Things
{
    /// <summary>An exit's endpoint.</summary>
    public class ExitEnd : Thing
    {
        /// <summary>Initializes a new instance of the ExitEnd class.</summary>
        /// <param name="room">The room of thie ExitEnd.</param>
        /// <param name="direction">The direction of the ExitEnd.</param>
        public ExitEnd(Thing room, string direction)
        {
            Room = room;
            Direction = direction;
        }

        /// <summary>Gets the direction this exit end faces.</summary>
        public string Direction { get; private set; }

        /// <summary>Gets the room this exit end is connected to.</summary>
        public Thing Room { get; private set; }
    }
}
*/

// Removed SingleDialCombinationLock.cs:
/*
namespace WheelMUD.Universe.Things.Locks
{
    /// <summary>Single dial combination lock (like on a safe)</summary>
    public class SingleDialCombinationLock : AbstractLock
    {
        /// <summary>Gets or sets the combination of Combination Entries required to unlock this lock.</summary>
        public List<SingleDialCombinationEntry> Combination { get; set; }

        /// <summary>Attempt to unlock the single dial combination lock.</summary>
        /// <param name="key">A string that contains the values of the dials</param>
        /// <returns>Returns true for succesful unlock, false for failure.</returns>
        public override bool AttemptUnlock(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>Attempt to lock the single dial combination lock.</summary>
        /// <param name="key">pass in the value to set the lock to as a string.</param>
        /// <returns>Returns true for success and false for failure.</returns>
        public override bool AttemptLock(string key)
        {
            throw new NotImplementedException();
        }
    }
}
*/

// Removed SingleDialCombinationEntry.cs:
/*
namespace WheelMUD.Universe.Things.Locks
{
    /// <summary>Direction to spin the dial.</summary>
    public enum DialDirection
    {
        /// <summary>Dial is spun to the left (Counter Clockwise).</summary>
        Left = 0,

        /// <summary>Dial is spun to the right (Clockwise).</summary>
        Right = 1
    }

    /// <summary>Single Combination value.</summary>
    public class SingleDialCombinationEntry
    {
        /// <summary>Gets or sets the direction to spin the dial.</summary>
        public DialDirection DialDirection { get; set; }

        /// <summary>Gets or sets the value that the combination must be stopped on.</summary>
        public string CombinationValue { get; set; }
    }
}
*/

// Removed MultiDialDisc.cs:
/*
namespace WheelMUD.Universe.Things.Locks
{
    /// <summary>A single dial in a multi dial combination lock.</summary>
    public class MultiDialDisc
    {
        /// <summary>Gets or sets the possible values for the dial.  Each dial can have a different list of values.</summary>
        public List<string> Values { get; set; }

        /// <summary>Gets or sets the value required to unlock this dial in a multi dial combination.</summary>
        public string CombinationEntry { get; set; }
    }
}
*/

// Removed MultiDialCombinationLock.cs:
/*
namespace WheelMUD.Universe.Things.Locks
{
    /// <summary>A multiple dial combination lock like a cryptext.</summary>
    public class MultiDialCombinationLock : AbstractLock
    {
        /// <summary>Gets or sets the collection of dials that make up the lock.</summary>
        public List<MultiDialDisc> Dials { get; set; }

        /// <summary>Attempt to unlock the multi dial combination lock.</summary>
        /// <param name="key">A string that contains the values of the dials</param>
        /// <returns>Returns true for succesful unlock, false for failure.</returns>
        public override bool AttemptUnlock(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>Attempt to lock the multi dial combination lock.</summary>
        /// <param name="key">pass in the value to set the lock to as a string.</param>
        /// <returns>Returns true for success and false for failure.</returns>
        public override bool AttemptLock(string key)
        {
            throw new NotImplementedException();
        }
    }
}
*/

// Removed KeyLock.cs:
/*
namespace WheelMUD.Universe.Things.Locks
{
    /// <summary>Typical lock where a key is insert to act on the lock.</summary>
    public class KeyLock : AbstractLock
    {
        /// <summary>Gets or sets the identifier for the key that goes with this lock.</summary>
        public long KeyItemId { get; set; }

        /// <summary>Attempt to unlock the multi dial combination lock.</summary>
        /// <param name="key">A string that contains the values of the dials</param>
        /// <returns>Returns true for succesful unlock, false for failure.</returns>
        public override bool AttemptUnlock(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>Attempt to lock the multi dial combination lock.</summary>
        /// <param name="key">pass in the value to set the lock to as a string.</param>
        /// <returns>Returns true for success and false for failure.</returns>
        public override bool AttemptLock(string key)
        {
            throw new NotImplementedException();
        }
    }
}*/

// Remvoed Key.cs:
/*
namespace WheelMUD.Universe.Things.Locks
{
    /// <summary>An openable and closable door.</summary>
    public class Key
    {
        /// <summary>Gets or sets the Lock ID that this key goes with.</summary>
        public long KeyLockItemId { get; set; }
    }
}*/

// Removed AbstractLock.cs:
/*
namespace WheelMUD.Universe.Things.Locks
{
    /// <summary>Base class for locks.</summary>
    /// <typeparam name="T"> Type of key </typeparam>
    /// <remarks>TODO: Replace with LocksUnlocksBehavior which can attach to any Thing we wish to be lockable...</remarks>
    public abstract class AbstractLock : Thing, IThingAdornment
    {
        /// <summary>Gets or sets a value indicating whether the lock is open or closed.</summary>
        public bool IsLocked { get; set; }

        /// <summary>Method to try to open the lock.</summary>
        /// <param name="key">Object that is to be used as a key.</param>
        /// <returns>Returns the success or failure of the attempt.</returns>
        public abstract bool AttemptUnlock(string key);

        /// <summary>Method to try to close the lock.</summary>
        /// <param name="key">Object that is to be used as a key.</param>
        /// <returns>Returns the success or failure of the attempt.</returns>
        public abstract bool AttemptLock(string key);
    }
}
*/

// Removed EnterPortal.cs:
/* TODO: REMOVE - Enter.cs instead should suffice
namespace WheelMUD.Actions
{
    /// <summary>A command that allows a player to enter a portal item.</summary>
    [ActionPrimaryAlias("enter portal", CommandCategory.Travel)]
    [ActionDescription("Enter a portal.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    internal class EnterPortal : Action
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive, 
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.InitiatorMustBeBalanced,
            CommonGuards.InitiatorMustBeMobile,
            CommonGuards.RequiresAtLeastOneArgument
        };

        /// <summary>The portal that we are to enter.</summary>
        private PortalBehavior portalBehavior = null;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            portalBehavior.Use(sender.Thing, bridge.World);
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            string commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            // Rule: Is there a portal in the room?
            // TODO: This sort of find pattern may become common; maybe we need to simplify 
            //       to having a Thing method which does this?  IE "List<Thing> FindChildren<T>()"?
            // TODO: Only target a portal which is a keyword match rather than iterating portals
            // TODO: Use a disambiguation targeting system should multiple matches be found.
            //       Note that the keyword really shouldn't have to be "portal" nor should "enter"
            //       command be tied only to Portals.  Maybe use an EnterableBehavior which has a
            //       configurable parameter to describe the transport, etc?
            Predicate<Thing> findPortalsPredicate = (Thing t) => t.BehaviorManager.FindFirst<PortalBehavior>() != null;
            List<Thing> portals = sender.Thing.Parent.FindAllChildren(findPortalsPredicate);
            
            foreach (Thing portal in portals)
            {
                // If we can find an actual portal that is rigged right now, use that one.
                portalBehavior = portal.BehaviorManager.FindFirst<PortalBehavior>();
                if (portalBehavior != null)
                {
                    return null;
                }
            }

            // If we got this far, we couldn't find an appropriate portal in the room.
            return "You can't see a portal.";
        }
    }
}*/

// Removed Deaf.cs
/* TODO: Re-implement as an AlterSenseEffect : EffectBase : Behavior w/duration
namespace WheelMUD.Effects
{
    public class DeafEffect : EffectBase
    {
        /// <summary>Applies the effect.</summary>
        /// <param name="duration">duration of the effect</param>
        public override void Apply(TimeSpan duration)
        {
            if (Host.Senses.Contains(SensoryType.Hearing))
            {
                Host.Senses[SensoryType.Hearing].Enabled = false;
            }

            base.Apply(duration);
        }

        /// <summary>When effect is removed, cleanup.</summary>
        public override void Remove()
        {
            RestoreHearing();
        }

        /// <summary>What occurs when timer wears off.</summary>
        /// <param name="state">Not sure what this is.</param>
        protected override void TickElapsed(object state)
        {
            RestoreHearing();
            RaiseEffectElapsed();
        }

        /// <summary>Turn off the changes that the effect caused.</summary>
        private void RestoreHearing()
        {
            if (Host.Senses.Contains(SensoryType.Hearing))
            {
                Host.Senses[SensoryType.Hearing].Enabled = true;
            }
        }
    }
}
*/

// Removed BasicGuardianBuilder.cs:
/* TODO: Extract the useful bits...
namespace WheelMUD.Universe.MobileBuilders
{
    /// <summary>The basic guardian builder.</summary>
    public class BasicGuardianBuilder : IMobileBuilder
    {
        /// <summary>The mobile data structure.</summary>
        private readonly MobRecord mds;
        
        /// <summary>The mobile's brain.</summary>
        private readonly BasicGuardMobBrain brain = new BasicGuardMobBrain();
        
        /// <summary>The instanciated mobile.</summary>
        private Mobile mob;

        /// <summary>Initializes a new instance of the BasicGuardianBuilder class.</summary>
        /// <param name="mob">The mobile data structure.</param>
        public BasicGuardianBuilder(MobRecord mob)
        {
            mds = mob;
        }

        /// <summary>Use the Mobile constructor to create a mobile. TODO: This seems awkward...</summary>
        public void UseMobileConstructor(Dictionary<string, object> args)
        {
            mob = new Mobile(brain, CoreManager.Instance.PlacesManager.World, mds);
            brain.Entity = mob;
        }

        /// <summary>Configures the Mobile.</summary>
        public void ConfigureMobile()
        {
            mob.Name = mds.Name;
            mob.Description = mds.Description;
            mob.Id = mds.ID;
            brain.Start();
        }

        /// <summary>Creates an instanciated Mobile.</summary>
        /// <returns>The instanciated Mobile.</returns>
        public IMobile GetInstanciatedMobile()
        {
            return mob;
        }
    }
}
*/

// Removed MobileDirector.cs:
/*
namespace WheelMUD.Universe.MobileBuilders
{
    /// <summary>A mobile director.</summary>
    public class MobileDirector
    {
        /// <summary>Constructs a mobile.</summary>
        /// <param name="builder">The mobile builder.</param>
        /// <param name="args">The arguments.</param>
        public void Construct(IMobileBuilder builder, Dictionary<string, object> args)
        {
            builder.UseMobileConstructor(args);
            builder.ConfigureMobile();
        }
    }
}*/

// Removed MobileBuilderController.cs:
/*
namespace WheelMUD.Universe.MobileBuilders
{
    /// <summary>The mobile builder controller class.</summary>
    public class MobileBuilderController
    {
        /// <summary>Creates a collection of mobs for the specified room.</summary>
        /// <param name="roomId">The room ID.</param>
        /// <returns>An entity collection of mobiles.</returns>
        public EntityCollection GetMobsForRoom(long roomId)
        {
            var repository = new MobRepository();
            var mobiles = new EntityCollection();
            var mobs = repository.GetMobsForRoom(roomId);
            ProcessMobList(mobs, mobiles);
            return mobiles;
        }

        /// <summary>Processes an enumerable list of mobile data structures.</summary>
        /// <param name="mobs">The enumerable list of mobiles.</param>
        /// <param name="mobList">The entity container of mobiles.</param>
        private static void ProcessMobList(IEnumerable<MobRecord> mobs, IEntityContainer mobList)
        {
            // TODO: Remove - mobile shouldn't need a BuilderController.
            //var director = new MobileDirector();
            //foreach (MobRecord mds in mobs)
            //{
            //    switch (mds.MobTypeID)
            //    {
            //        case (int)MobRecord.MobTypes.BasicGuard:
            //            var builder = new BasicGuardianBuilder(mds);
            //            director.Construct(builder, null);
            //            mobList.AddEntity(builder.GetInstanciatedMobile());
            //            CoreManager.Instance.MobileManager.RegisterMobile(builder.GetInstanciatedMobile());
            //            break;
            //    }
            //}
        }
    }
}
*/

/* REMOVED FROM IEventObserver.cs:
public interface IEventObserver
{
    /// <summary>Subscribes the observer to a broadcaster.</summary>
    /// <param name="broadcaster">The broadcaster to subscribe to</param>
    void Subscribe(IEventBroadcaster broadcaster);

    /// <summary>Unsubscribes the observer from a broadcaster.</summary>
    /// <param name="broadcaster">The broadcaster to unsubscribe from</param>
    void UnSubscribe(IEventBroadcaster broadcaster);

    /// <summary>The entry point for a broadcaster to update this observer with an event.</summary>
    /// <param name="theEvent">The event to pass to the observer.</param>
    void Receive(IEvent theEvent);
}*/

/* REMOVED FROM Thing.cs: Need to re-implement Adornments through something like AdornmentBehavior
 *                        which has a Thing sub-property or whatnot.
        /// <summary>Method that will go through all adornments and save them.</summary>
        protected void SaveAdornments()
        {
            foreach (IThingAdornment adornment in adornments)
            {
                var thing = adornment as Thing;
                if (thing != null)
                {
                    thing.Parent = this;
                    thing.IsAdornment = true;
                    thing.Save();
                }
            }
        }
        /// <summary>Items that are separate, but act as part of the parent.</summary>
        private List<IThingAdornment> adornments = new List<IThingAdornment>();
        /// <summary>Gets or sets a value indicating whether this item is an adornment.</summary>
        public bool IsAdornment { get; set; }
*/

/* REMOVED FROM Mobile.cs:
    /// <summary>A mobile entity.</summary>
    public class Mobile : Entity
    {
        /// <summary>The synchronization locking object.</summary>
        private readonly object lockObject = new object();

        /// <summary>The data record for this mobile.</summary>
        private readonly MobRecord DataRecord = new MobRecord();

        /// <summary>Initializes a new instance of the Mobile class.</summary>
        /// <param name="controller">The controller of the mobile.</param>
        /// <param name="world">Representation of the world.</param>
        /// <param name="mobDataRecord">The mob attributes loaded from the database.</param>
        public Mobile(IController controller, World world, MobRecord mobDataRecord)
            : base(controller, world)
        {
            Id = mobDataRecord.ID;
            DataRecord = mobDataRecord;
            brain.ActionReceived += Brain_ActionReceived;
        }

    }
*/

/* REMOVED FROM ItemBehaviorManager.cs:
        /// <summary>Saves the item behaviors</summary>
        public override void Save()
        {
            // Save the Item Behaviors
            ItemBehaviorRepository itemBehaviorRepository = new ItemBehaviorRepository();

            // Save the Item Behavior properties
            ItemBehaviorPropertyRepository itemBehaviorPropertyRepository =
                new ItemBehaviorPropertyRepository();

            foreach (Behavior behavior in ManagedBehaviors)
            {
                long behaviorId = behavior.Id;

                if (behaviorId == 0)
                {
                    ItemBehaviorRecord itemBehaviorRecord = new ItemBehaviorRecord
                    {
                        // TODO: Track behavior by its name for reflection/MEF usage.
                        //ItemBehaviorTypeID = (long)behavior.ItemBehaviorType,
                        ItemID = ItemId
                    };

                    itemBehaviorRepository.Add(itemBehaviorRecord);

                    behaviorId = itemBehaviorRecord.ID;
                    behavior.Id = behaviorId;
                }

                List<ItemBehaviorPropertyRecord> records = behavior.GetRecords();
                foreach (ItemBehaviorPropertyRecord record in records)
                {
                    record.ItemBehaviorID = behaviorId;

                    if (record.ID == 0)
                    {
                        itemBehaviorPropertyRepository.Add(record);                       
                    }
                    else
                    {
                        itemBehaviorPropertyRepository.Update(record);                       
                    }
                }
            }
        }
*/