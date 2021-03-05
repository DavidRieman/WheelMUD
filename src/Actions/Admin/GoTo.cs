//-----------------------------------------------------------------------------
// <copyright file="GoTo.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;
using WheelMUD.Server;

namespace WheelMUD.Actions
{
    /// <summary>A command that allows a player move to the room Id or entity specified.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("goto", CommandCategory.Admin)]
    [ActionAlias("go to", CommandCategory.Admin)]
    [ActionDescription("Go to the specified entity or room number.")]
    [ActionSecurity(SecurityRole.fullAdmin | SecurityRole.mobile)]
    public class GoTo : GameAction
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

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            if (!(actionInput.Controller is Session session)) return;

            // If input is a simple number, assume we mean a room
            var targetPlace = int.TryParse(actionInput.Tail, out var roomNum) ? ThingManager.Instance.FindThing("room/" + roomNum) : ThingManager.Instance.FindThingByName(actionInput.Tail, false, true);

            if (targetPlace == null)
            {
                actionInput.Controller.Write(new OutputBuilder(session.TerminalOptions).SingleLine("Room or Entity not found."));
                return;
            }

            if (targetPlace.FindBehavior<RoomBehavior>() == null)
            {
                // If the target's parent is a room, go there instead
                if (targetPlace.Parent != null && targetPlace.Parent.FindBehavior<RoomBehavior>() != null)
                {
                    targetPlace = targetPlace.Parent;
                }
                else
                {
                    actionInput.Controller.Write(new OutputBuilder(session.TerminalOptions).SingleLine("Target is not a room and is not in a room!"));
                    return;
                }
            }

            var adminName = actionInput.Controller.Thing.Name;
            var leaveContextMessage = new ContextualString(actionInput.Controller.Thing, actionInput.Controller.Thing.Parent)
            {
                ToOriginator = null,
                ToReceiver = $"{adminName} disappears into nothingness.",
                ToOthers = $"{adminName} disappears into nothingness.",
            };
            var arriveContextMessage = new ContextualString(actionInput.Controller.Thing, targetPlace)
            {
                ToOriginator = $"You teleported to {targetPlace.Name}.",
                ToReceiver = $"{adminName} appears from nothingness.",
                ToOthers = $"{adminName} appears from nothingness.",
            };
            var leaveMessage = new SensoryMessage(SensoryType.Sight, 100, leaveContextMessage);
            var arriveMessage = new SensoryMessage(SensoryType.Sight, 100, arriveContextMessage);

            // If we successfully move (IE the move may get cancelled if the user doesn't have permission
            // to enter a particular location, some other behavior cancels it, etc), then perform a 'look'
            // command to get immediate feedback about the new location.
            // TODO: This should not 'enqueue' a command since, should the player have a bunch of 
            //     other commands entered, the 'look' feedback will not immediately accompany the 'goto' 
            //     command results like it should.
            var movableBehavior = actionInput.Controller.Thing.FindBehavior<MovableBehavior>();

            if (movableBehavior != null && movableBehavior.Move(targetPlace, actionInput.Controller.Thing, leaveMessage, arriveMessage))
            {
                CommandManager.Instance.EnqueueAction(new ActionInput("look", actionInput.Controller));
            }
        }

        /// <summary>Prepare for, and determine if the command's prerequisites have been met.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            return VerifyCommonGuards(actionInput, ActionGuards);
        }
    }
}