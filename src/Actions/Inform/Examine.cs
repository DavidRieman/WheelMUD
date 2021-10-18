//-----------------------------------------------------------------------------
// <copyright file="Examine.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>A command that allows a player to look at something.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("examine", CommandCategory.Inform)]
    [ActionAlias("ex", CommandCategory.Inform)]
    [ActionAlias("exa", CommandCategory.Inform)]
    [ActionAlias("info", CommandCategory.Inform)]
    [ActionDescription("Examine something closely.")]
    [ActionSecurity(SecurityRole.player)]
    public class Examine : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive,
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.RequiresAtLeastOneArgument
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var session = actionInput.Session;
            if (session == null) return; // Info command: Only makes sense to send for player sessions.

            var parent = actionInput.Actor.Parent;
            var searchString = actionInput.Tail.Trim().ToLower();

            if (string.IsNullOrEmpty(searchString))
            {
                session.WriteLine("You must specify something to search for.");
                return;
            }

            // Unique case. Try to perceive the room (and its contents) instead; same as "look".
            if (searchString == "here")
            {
                session.Write(Renderer.Instance.RenderPerceivedRoom(session.TerminalOptions, actionInput.Actor, parent));
                return;
            }

            // First check the place where the sender is located (like a room) for the target,
            // and if not found, search the sender's children (like inventory) for the target.
            var thing = parent.FindChild(searchString) ?? actionInput.Actor.FindChild(searchString);

            if (thing != null)
            {
                session.Write(Renderer.Instance.RenderPerceivedThing(session.TerminalOptions, actionInput.Actor, thing));
            }
            else
            {
                session.WriteLine($"You cannot find {searchString}.");
            }
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            return VerifyCommonGuards(actionInput, ActionGuards);
        }
    }
}