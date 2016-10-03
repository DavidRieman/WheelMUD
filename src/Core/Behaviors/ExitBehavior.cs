//-----------------------------------------------------------------------------
// <copyright file="ExitBehavior.cs" company="WheelMUD Development Team">
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
    using System.Linq;
    using WheelMUD.Actions;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Events;
    using WheelMUD.Utilities;

    /// <summary>
    /// An ExitBehavior allows a Thing to be used as an exit to another location.  ExitBehavior can be 
    /// used in a couple of ways to recreate typical MUD exits (in addition to other advanced usages):
    /// 1) To make a one-way exit: place ExitBehavior on a Thing which resides at the source location,
    ///    specifying the [direction, destinationID] pair.
    /// 2) To make a two-way exit: place ExitBehavior and a MultipleParentsBehavior on a Thing in order
    ///    to place the exit in both locations, and specify two [direction, destinationID] pairs which 
    ///    point to their respective target locations.
    /// </summary>
    public class ExitBehavior : Behavior
    {
        // @@@ Add attribute and persistence code to save certain marked private properties like this;
        //     IE we don't want to expose the whole dictionary publically since we have things to do
        //     while rigging up new destinations
        private List<DestinationInfo> destinations;

        /// <summary>The context command handler for this exit.</summary>
        private ExitBehaviorCommands commands;

        /// <summary>Initializes a new instance of the ExitBehavior class.</summary>
        public ExitBehavior()
            : base(null)
        {
            this.commands = new ExitBehaviorCommands(this);
        }

        /// <summary>Initializes a new instance of the ExitBehavior class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public ExitBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.commands = new ExitBehaviorCommands(this); 
            this.ID = instanceID;
        }

        /// <summary>Adds the destination.</summary>
        /// <param name="movementCommand">The movement command.</param>
        /// <param name="destinationID">The destination ID.</param>
        public void AddDestination(string movementCommand, string destinationID)
        {
            var existingDestination = (from d in this.destinations where d.TargetID == destinationID select d).FirstOrDefault();
            if (existingDestination == null)
            {
                this.destinations.Add(new DestinationInfo(movementCommand.ToLower(), destinationID));
            }
        }

        /// <summary>Gets the destination.</summary>
        /// <param name="fromLocation">From location.</param>
        /// <returns>Returns a Thing object.</returns>
        public Thing GetDestination(Thing fromLocation)
        {
            // Find the first destination info that doesn't match this location.
            var destinationInfo = (from d in this.destinations
                                   where d.TargetID != fromLocation.ID
                                   select d).FirstOrDefault();

            if (destinationInfo == null)
            {
                return null;
            }

            if (destinationInfo.TargetID != fromLocation.ID)
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
                    destinationInfo.CachedTarget = new WeakReference<Thing>(destination);
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
            var movableBehavior = thingToMove.Behaviors.FindFirst<MovableBehavior>();
            if (movableBehavior == null)
            {
                // @@@ TODO: Add messaging to thingToMove?
                return false;
            }

            // Find the target location to be reached from here.
            DestinationInfo destinationInfo = this.GetDestinationFrom(thingToMove.Parent.ID);
            if (destinationInfo == null)
            {
                // There was no destination reachable from the thing's starting location.
                return false;
            }
            
            // If the target location hasn't been cached already, try to do so now.
            if (destinationInfo.CachedTarget == null || destinationInfo.CachedTarget.Target == null)
            {
                Thing newTarget = ThingManager.Instance.FindThing(destinationInfo.TargetID);
                destinationInfo.CachedTarget = new WeakReference<Thing>(newTarget);
            }

            // If the destination can't be found, abort.
            Thing destination = destinationInfo.CachedTarget.Target;
            if (destination == null)
            {
                // @@@ TODO: Add messaging to thingToMove?
                return false;
            }

            string dir = destinationInfo.ExitCommand;
            var leaveContextMessage = new ContextualString(thingToMove, thingToMove.Parent)
            {
                ToOriginator = null,
                ToReceiver = @"$ActiveThing.Name moves " + dir + ".",
                ToOthers = @"$ActiveThing.Name moves " + dir + ".",
            };
            var arriveContextMessage = new ContextualString(thingToMove, destination)
            {
                ToOriginator = @"You move " + dir + " to $GoingTo.Name.",
                ToReceiver = @"$ActiveThing.Name arrives, heading " + dir + ".",
                ToOthers = @"$ActiveThing.Name arrives, heading " + dir + ".",
            };
            var leaveMessage = new SensoryMessage(SensoryType.Sight, 100, leaveContextMessage);
            var arriveMessage = new SensoryMessage(SensoryType.Sight, 100, arriveContextMessage);

            return movableBehavior.Move(destination, this.Parent, leaveMessage, arriveMessage);
        }

        /// <summary>Gets the exit command from.</summary>
        /// <param name="fromLocation">From location.</param>
        /// <returns>Returns the exit direction.</returns>
        public string GetExitCommandFrom(Thing fromLocation)
        {
            var destination = this.GetDestinationFrom(fromLocation.ID);
            return destination != null ? destination.ExitCommand : null;
        }

        /// <summary>Called when a parent has just been assigned to this behavior. (Refer to this.Parent)</summary>
        public override void OnAddBehavior()
        {
            // @@@ TODO: Greatly simplify: use shared logic with ParentMovementEvent! React to OnRemoveBehavior too!
            // When adding this behavior to an exit Thing, if that thing has a parent, rig up the appropriate
            // context command for that place to reach the other.
            if (this.Parent.Parent != null)
            {
                // @@@ TODO: Reuse the same functionality as the movement event handler for command rigging (if we can).
                this.ParentMovementEventHandler(this.Parent, null);
            }

            // Rig up to the parent (exit) Thing's 'moved' events so we can fix the exit targets back up (or
            // rig them up the first time if it didn't yet have such a parent).
            this.Parent.Eventing.MovementEvent += this.ParentMovementEventHandler;

            Thing initialPlace = this.Parent.Parent;
            if (initialPlace != null)
            {
                // If the thing we already added the behavior to is already within something, add the initial exit command(s).
                this.AddExitContextCommands(initialPlace);
            }

            base.OnAddBehavior();
        }

        /// <summary>Called when the current parent of this behavior is about to be removed. (Refer to this.Parent)</summary>
        public override void OnRemoveBehavior()
        {
            // When removing this behavior from a thing, we need to unrig any context commands we added to it.
            string commandText = this.GetExitCommandFrom(this.Parent);
            if (!string.IsNullOrEmpty(commandText))
            {
                this.Parent.Commands.Remove(commandText);
            }

            base.OnRemoveBehavior();
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            this.destinations = new List<DestinationInfo>();
        }

        /// <summary>Handle the events of our parent moving; need to adjust our exit context commands and such.</summary>
        /// <param name="root">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void ParentMovementEventHandler(Thing root, Events.GameEvent e)
        {
            // If our parent (the thing with exit behavior) was removed from something (like a room)...
            var removeChildEvent = e as RemoveChildEvent;
            if (removeChildEvent != null &&
                removeChildEvent.ActiveThing == this.Parent &&
                removeChildEvent.OldParent != null)
            {
                // Remove the old exit command, if one was rigged up to the old location.
                string oldExitCommand = this.GetExitCommandFrom(removeChildEvent.OldParent);
                removeChildEvent.OldParent.Commands.Remove(oldExitCommand);
            }
            
            // If our parent (the thing with exit behavior) was placed in something (like a room)...
            var addChildEvent = e as AddChildEvent;
            if (addChildEvent != null &&
                addChildEvent.ActiveThing == this.Parent &&
                addChildEvent.NewParent != null)
            {
                // Add the appropriate exit command for the new location.
                this.AddExitContextCommands(addChildEvent.NewParent);
            }
        }

        /// <summary>Add the appropriate exit context command(s) to the specified location.</summary>
        /// <param name="location">The location to add exit context command(s) to.</param>
        private void AddExitContextCommands(Thing location)
        {
            var mainExitCommand = this.GetExitCommandFrom(location);
            var secondExitAlias = this.GetSecondaryExitAlias(mainExitCommand);
            if (!string.IsNullOrEmpty(mainExitCommand))
            {
                var contextCommand = new ContextCommand(this.commands, mainExitCommand, ContextAvailability.ToChildren, SecurityRole.all);
                location.Commands.Add(mainExitCommand, contextCommand);
                if (!string.IsNullOrEmpty(secondExitAlias))
                {
                    location.Commands.Add(secondExitAlias, contextCommand);
                }
            }
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
        private DestinationInfo GetDestinationFrom(string originID)
        {
            return (from d in this.destinations where originID != d.TargetID select d).FirstOrDefault();
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
            private ExitBehavior exitBehavior;

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
                this.exitBehavior.MoveThrough(actionInput.Controller.Thing);
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

                if (actionInput.Controller.Thing.Behaviors.FindFirst<MovableBehavior>() == null)
                {
                    return "You do not have the ability to move.";
                }

                return null;
            }
        }

        /// <summary>Information about an exit's destination.</summary>
        private class DestinationInfo
        {
            /// <summary>Initializes a new instance of the DestinationInfo class.</summary>
            /// <param name="command">The command which is used to reach the target destination.</param>
            /// <param name="targetID">The ID of the target destination.</param>
            public DestinationInfo(string command, string targetID)
            {
                this.ExitCommand = command;
                this.TargetID = targetID;
                this.CachedTarget = new WeakReference<Thing>(null);
            }

            /// <summary>Gets or sets the command which is used to reach the target destination.</summary>
            public string ExitCommand { get; set; }

            /// <summary>Gets or sets the ID of the target destination.</summary>
            public string TargetID { get; set; }

            /// <summary>Gets or sets the cached destination thing.</summary>
            public WeakReference<Thing> CachedTarget { get; set; }
        }
    }
}