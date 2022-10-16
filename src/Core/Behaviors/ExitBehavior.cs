//-----------------------------------------------------------------------------
// <copyright file="ExitBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WheelMUD.Core.Behaviors;
using WheelMUD.Utilities;

namespace WheelMUD.Core
{
    /// <summary>
    /// An ExitBehavior allows a Thing to be used as an exit to another location.
    /// ExitBehavior can be used in a couple of ways to recreate these typical MUD exit behaviors:
    /// 1) To make a one-way exit: place ExitBehavior on a Thing which resides at the source location,
    ///    specifying the [direction, destinationID] pair.
    /// 2) To make a two-way exit: place ExitBehavior and a MultipleParentsBehavior on a Thing in order
    ///    to place the exit in both locations, and specify two [direction, destinationID] pairs which 
    ///    point to their respective target locations.
    /// </summary>
    /// <remarks>
    /// Note that ExitBehavior persistence is customized to reduce the scope and timing implications of chained world
    /// loading. We will retain cached weak references to target exits, but the destination Thing Id fields will be
    /// what we persist. (We don't want to accidentally force referenced rooms to persist and load with us even though
    /// such a room might have Persistence=false to be ephemeral. Thus, if an ExitBehavior has a destination that can
    /// not gain an Id then that destination will not be linked back up from deserializing the ExitBehavior.
    /// TODO: Also note what happens at runtime when trying to use an exit that cannot find the target at runtime,
    ///       perhaps this should give the user a message like "You cannot find your way to the destination..." which
    ///       could happen if the ID was noted but said thing has since been deleted or failed to load with the world.
    /// </remarks>
    public class ExitBehavior : Behavior
    {
        /// <summary>The context command handler for this exit.</summary>
        private readonly ExitBehaviorCommands commands;

        private List<ExitDestinationInfo> Destinations { get; set; } = new List<ExitDestinationInfo>();

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            var invalidDestinations = (from d in Destinations where d.TargetID == null select d).ToList();
            foreach (var invalid in invalidDestinations)
            {
                Destinations.Remove(invalid);
            }
        }

        /// <summary>Initializes a new instance of the ExitBehavior class.</summary>
        public ExitBehavior() : base(null)
        {
            commands = new ExitBehaviorCommands(this);
        }

        /// <summary>Initializes a new instance of the ExitBehavior class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public ExitBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            commands = new ExitBehaviorCommands(this);
            ID = instanceID;
        }

        /// <summary>Adds the destination.</summary>
        /// <param name="movementCommand">The movement command.</param>
        /// <param name="destination">The destination Thing.</param>
        public void AddDestination(string movementCommand, Thing destination)
        {
            movementCommand = NormalizeDirection(movementCommand);

            // For simplicity, a standard two-way exit will only support up to two destinations, which must be mirrors.
            // E.G. if you want two rooms where you go "east" from A to B but then still "east" to get from B to A, you
            // will want to use a different solution (such as one-way exits). If we try to support too many weird edge
            // cases for a single ExitBehavior instance, the code would get really hard to reason about and maintain.
            // (If you REALLY need this because, for example, you also need a shared door on the weird exit, perhaps
            // you can create and add maintain a one-off version of ExitBehavior supporting the scenarios you need, in
            // a game-specific code area.)
            // So, first, figure out if we need to replace an existing destination (by removing the old one first).
            // * We do this if this behavior already as the same direction added. E.G. AddDestination of "east" when we
            //   already have an "east" for this behavior will replace the old link.
            // * We do this if the target destination is already in the list as well. E.G. if we already had an "east"
            //   going to "thing/a", but are adding a "south" to "thing/a", this behavior will replace the old link.
            var existingDestination = (from d in Destinations
                                       where d.ExitCommand == movementCommand || d.CachedTarget.Target == destination
                                       select d).FirstOrDefault();
            if (existingDestination == null)
            {
                Destinations.Remove(existingDestination);
            }

            // Then only add the new destination if it is not exceeding our supported range (1-2 targets).
            if (Destinations.Count < 2)
            {
                Destinations.Add(new ExitDestinationInfo(movementCommand.ToLower(), destination));
            }
        }

        /// <summary>Gets the destination.</summary>
        /// <param name="fromLocation">From location.</param>
        /// <returns>Returns a Thing object.</returns>
        public Thing GetDestination(Thing fromLocation)
        {
            // Find the first destination info that doesn't match this location.
            var destinationInfo = (from d in Destinations
                                   where d.TargetID != fromLocation.Id
                                   select d).FirstOrDefault();

            if (destinationInfo == null)
            {
                return null;
            }

            if (destinationInfo.TargetID != fromLocation.Id)
            {
                // Get and return the cached Thing itself from this ID.  If it's not cached
                // right now, try to find it, and save it in the cache before returning it.
                Thing destination = destinationInfo.CachedTarget.Target;
                if (destination != null)
                {
                    return destination;
                }

                destination = ThingManager.Instance.FindThing(destinationInfo.TargetID);
                if (destination != null)
                {
                    destinationInfo.CachedTarget = new SimpleWeakReference<Thing>(destination);
                }

                return destination;
            }

            return null;
        }

        /// <summary>Moves the Thing through.</summary>
        /// <param name="thingToMove">The thing to move.</param>
        /// <returns>Returns true if the move was successful, false if not.</returns>
        public bool MoveThrough(Thing thingToMove)
        {
            // If the thing isn't currently mobile, bail.
            var movableBehavior = thingToMove.FindBehavior<MovableBehavior>();
            if (movableBehavior == null)
            {
                // TODO: Add messaging to thingToMove?
                return false;
            }

            // Find the target location to be reached from here.
            var destinationInfo = GetDestinationFrom(thingToMove.Parent.Id);
            if (destinationInfo == null)
            {
                // There was no destination reachable from the thing's starting location.
                return false;
            }

            // If the target location hasn't been cached already, try to do so now.
            if (destinationInfo.CachedTarget == null || destinationInfo.CachedTarget.Target == null)
            {
                Thing newTarget = ThingManager.Instance.FindThing(destinationInfo.TargetID);
                destinationInfo.CachedTarget = new SimpleWeakReference<Thing>(newTarget);
            }

            // If the destination can't be found, abort.
            Thing destination = destinationInfo.CachedTarget.Target;
            if (destination == null)
            {
                // TODO: Add messaging to thingToMove?
                return false;
            }

            string dir = destinationInfo.ExitCommand;
            var leaveContextMessage = new ContextualString(thingToMove, thingToMove.Parent)
            {
                ToOriginator = null,
                ToReceiver = $"{thingToMove.Name} moves {dir}.",
                ToOthers = $"{thingToMove.Name} moves {dir}.",
            };
            var arriveContextMessage = new ContextualString(thingToMove, destination)
            {
                ToOriginator = $"You move {dir} to {destination.Name}.",
                ToReceiver = $"{thingToMove.Name} arrives, heading {dir}.",
                ToOthers = $"{thingToMove.Name} arrives, heading {dir}.",
            };
            var leaveMessage = new SensoryMessage(SensoryType.Sight, 100, leaveContextMessage);
            var arriveMessage = new SensoryMessage(SensoryType.Sight, 100, arriveContextMessage);

            return movableBehavior.Move(destination, Parent, leaveMessage, arriveMessage);
        }

        /// <summary>Gets the exit command from.</summary>
        /// <param name="fromLocation">From location.</param>
        /// <returns>Returns the exit direction.</returns>
        public string GetExitCommandFrom(Thing fromLocation)
        {
            var destination = GetDestinationFrom(fromLocation.Id);
            return destination?.ExitCommand;
        }

        /// <summary>Called when a parent has just been assigned to this behavior. (Refer to Parent)</summary>
        protected override void OnAddBehavior()
        {
            // TODO: Greatly simplify: use shared logic with ParentMovementEvent! React to OnRemoveBehavior too!
            // When adding this behavior to an exit Thing, if that thing has a parent, rig up the appropriate
            // context command for that place to reach the other.
            if (Parent.Parent != null)
            {
                var e = new AddChildEvent(Parent, Parent.Parent);
                ParentMovementEventHandler(Parent, e);
            }

            // Rig up to the parent (exit) Thing's 'moved' events so we can fix the exit targets back up (or
            // rig them up the first time if it didn't yet have such a parent).
            Parent.Eventing.MovementEvent += ParentMovementEventHandler;

            base.OnAddBehavior();
        }

        /// <summary>Called when the current parent of this behavior is about to be removed. (Refer to Parent)</summary>
        protected override void OnRemoveBehavior()
        {
            // When removing this behavior from a thing, we need to also remove any context commands we added to it.
            string commandText = GetExitCommandFrom(Parent);
            if (!string.IsNullOrEmpty(commandText))
            {
                Parent.Commands.Remove(commandText);
            }

            base.OnRemoveBehavior();
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
        }

        /// <summary>Handle the events of our parent moving; need to adjust our exit context commands and such.</summary>
        /// <param name="root">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void ParentMovementEventHandler(Thing root, GameEvent e)
        {
            // If our parent (the thing with exit behavior) was removed from something (like a room)...
            var removeChildEvent = e as RemoveChildEvent;
            if (removeChildEvent != null &&
                removeChildEvent.ActiveThing == Parent &&
                removeChildEvent.OldParent != null)
            {
                // Remove the old exit command, if one was rigged up to the old location.
                string oldExitCommand = GetExitCommandFrom(removeChildEvent.OldParent);
                removeChildEvent.OldParent.Commands.Remove(oldExitCommand);
            }

            // If our parent (the thing with exit behavior) was placed in something (like a room)...
            var addChildEvent = e as AddChildEvent;
            if (addChildEvent != null &&
                addChildEvent.ActiveThing == Parent &&
                addChildEvent.NewParent != null)
            {
                // Add the appropriate exit command for the new location.
                AddExitContextCommands(addChildEvent.NewParent);
            }
        }

        /// <summary>Add the appropriate exit context command(s) to the specified location.</summary>
        /// <param name="location">The location to add exit context command(s) to.</param>
        private void AddExitContextCommands(Thing location)
        {
            var mainExitCommand = GetExitCommandFrom(location);
            var secondExitAlias = GetSecondaryExitAlias(mainExitCommand);
            if (!string.IsNullOrEmpty(mainExitCommand))
            {
                var contextCommand = new ContextCommand(commands, mainExitCommand, ContextAvailability.ToChildren, SecurityRole.all);
                // The process for repairing the parents of loaded behaviors can result in attempting to double-attach
                // in some contexts (but not others). If the location already has the same command, for now we will
                // assume it was also for this behavior and we don't need to attach again.
                if (!location.Commands.ContainsKey(mainExitCommand))
                {
                    location.Commands.Add(mainExitCommand, contextCommand);
                }
                if (!string.IsNullOrEmpty(secondExitAlias) && !location.Commands.ContainsKey(secondExitAlias))
                {
                    location.Commands.Add(secondExitAlias, contextCommand);
                }
            }
        }

        public static readonly Dictionary<string, string> PrimaryToSecondaryCommandMap = new Dictionary<string, string>()
        {
            { "north", "n" },
            { "northeast", "ne" },
            { "east", "e" },
            { "southeast", "se" },
            { "south", "s" },
            { "southwest", "sw" },
            { "west", "w" },
            { "northwest", "nw" },
            { "up", "u" },
            { "down", "d" },
            { "enter", "en" },
            { "exit", "ex" }
        };

        public static readonly Dictionary<string, string> MirrorDirectionMap = new Dictionary<string, string>()
        {
            { "north", "south" },
            { "northeast", "southwest" },
            { "east", "west" },
            { "southeast", "northwest" },
            { "south", "north" },
            { "southwest", "northeast" },
            { "west", "east" },
            { "northwest", "southeast" },
            { "up", "down" },
            { "down", "up" },
            { "enter", "exit" },
            { "exit", "enter" }
        };

        /// <summary>If the creator gives us shorthand such as "ne", normalize it to "northeast". Else keep it as-is.</summary>
        public static string NormalizeDirection(string direction)
        {
            var foundPrimary = (from kvp in PrimaryToSecondaryCommandMap
                                where kvp.Value.Equals(direction, StringComparison.OrdinalIgnoreCase)
                                select kvp.Key).FirstOrDefault();
            return foundPrimary ?? direction;
        }

        /// <summary>Get up to one common secondary exit alias for a full exit command.</summary>
        /// <param name="primaryExitCommand">The primary exit command to get an alias for.</param>
        /// <returns>A common secondary exit alias for the primary exit command, if one exists.</returns>
        private string GetSecondaryExitAlias(string primaryExitCommand)
        {
            switch (primaryExitCommand)
            {
                case "north": return "n";
                case "east": return "e";
                case "south": return "s";
                case "west": return "w";
                case "northeast": return "ne";
                case "southeast": return "se";
                case "southwest": return "sw";
                case "northwest": return "nw";
                case "up": return "u";
                case "down": return "d";
            }

            return null;
        }

        /// <summary>Get the DestinationInfo for the destination, as if going through this exit from the specified ID.</summary>
        /// <param name="originID">The origin ID to find a destination for.</param>
        /// <returns>A destination for the specified origin Thing ID.</returns>
        private ExitDestinationInfo GetDestinationFrom(string originID)
        {
            return (from d in Destinations where originID != d.TargetID select d).FirstOrDefault();
        }

        /// <summary>A command handler for this exit's context commands.</summary>
        private class ExitBehaviorCommands : GameAction
        {
            /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
            private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
            {
                CommonGuards.InitiatorMustBeAlive,
                CommonGuards.InitiatorMustBeConscious,
                CommonGuards.InitiatorMustBeBalanced,
                CommonGuards.InitiatorMustBeMobile
            };

            /// <summary>The ExitBehavior this class belongs to.</summary>
            private readonly ExitBehavior exitBehavior;

            /// <summary>Initializes a new instance of the ExitBehaviorCommands class.</summary>
            /// <param name="exitBehavior">The ExitBehavior this class belongs to.</param>
            public ExitBehaviorCommands(ExitBehavior exitBehavior)
                : base()
            {
                this.exitBehavior = exitBehavior;
            }

            /// <summary>Execute the action.</summary>
            /// <param name="actionInput">The full input specified for executing the command.</param>
            public override void Execute(ActionInput actionInput)
            {
                // If the user invoked the context command, move them through this exit.
                exitBehavior.MoveThrough(actionInput.Actor);
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

                if (actionInput.Actor.FindBehavior<MovableBehavior>() == null)
                {
                    return "You do not have the ability to move.";
                }

                return null;
            }
        }
    }
}