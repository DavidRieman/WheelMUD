//-----------------------------------------------------------------------------
// <copyright file="Look.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;
using WheelMUD.Server;

namespace WheelMUD.Actions
{
    /// <summary>A command that allows a player to look at things and their environment.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("look", CommandCategory.Inform)]
    [ActionAlias("l", CommandCategory.Inform)]
    [ActionDescription("Look at the room, item, person, or monster.")]
    [ActionSecurity(SecurityRole.player)]
    public class Look : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive,
            CommonGuards.InitiatorMustBeConscious
        };

        private SensesBehavior sensesBehavior;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var session = actionInput.Session;
            if (session == null) return; // This action is not currently built to be useful to non-players (such as mobile AI systems).

            if (!string.IsNullOrEmpty(actionInput.Tail))
            {
                if (!TryLookAtThing(session.TerminalOptions, actionInput.Tail, actionInput.Actor, out var found))
                {
                    found.AppendLine($"You cannot see {actionInput.Tail}.");
                }

                session.Write(found);
                return;
            }

            session.Write(LookAtRoom(session.TerminalOptions, actionInput.Actor));
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            var commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            sensesBehavior = actionInput.Actor.Behaviors.FindFirst<SensesBehavior>();
            return sensesBehavior == null ? "You do not have any senses to perceive with." : null;
        }

        /// <summary>Tries to look at a thing.</summary>
        /// <param name="terminalOptions">The terminal options to render the description with.</param>
        /// <param name="thingToLookAt">The thing to look at.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="found"></param>
        /// <returns>Returns the rendered view.</returns>
        private bool TryLookAtThing(TerminalOptions terminalOptions, string thingToLookAt, Thing sender, out OutputBuilder found)
        {
            found = new OutputBuilder();

            // Look for the target in the current room.
            var thing = sender.Parent.FindChild(thingToLookAt);
            if (thing != null && sensesBehavior.CanPerceiveThing(thing))
            {
                found = Renderer.Instance.RenderPerceivedThing(terminalOptions, sender, thing);
                return true;
            }

            // If no target was found, see if it matches any of the room's visuals.
            var room = sender.Parent.FindBehavior<RoomBehavior>();
            if (room != null)
            {
                var visual = room.FindVisual(thingToLookAt);
                if (!string.IsNullOrEmpty(visual))
                {
                    found = new OutputBuilder(visual.Length).AppendLine(visual);
                    return true;
                }
            }

            // Otherwise, see if it is a thing the player has.
            thing = sender.FindChild(thingToLookAt);
            if (thing != null && sensesBehavior.CanPerceiveThing(thing))
            {
                found = Renderer.Instance.RenderPerceivedThing(terminalOptions, sender, thing);
                return true;
            }

            // At this point, target was not found.
            return false;
        }

        /// <summary>Looks at room. TODO: Move to SensesBehavior?</summary>
        /// <param name="sender">The sender.</param>
        /// <returns>Returns the text of the rendered room template.</returns>
        private OutputBuilder LookAtRoom(TerminalOptions terminalOptions, Thing sender)
        {
            return Renderer.Instance.RenderPerceivedRoom(terminalOptions, sender, sender.Parent);
        }
    }
}
