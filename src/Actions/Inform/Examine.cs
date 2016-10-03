//-----------------------------------------------------------------------------
// <copyright file="Examine.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A script that allows a player to look at something.
//   Created: October 2006 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using System.Text;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>A command that allows a player to look at something.</summary>
    [ExportGameAction]
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
            IController sender = actionInput.Controller;
            Thing parent = sender.Thing.Parent;
            string searchString = actionInput.Tail.Trim().ToLower();

            if (string.IsNullOrEmpty(searchString))
            {
                sender.Write("You must specify something to search for.");
                return;
            }

            // Unique case. Use 'here' to list the contents of the room.
            if (searchString == "here")
            {
                sender.Write(this.ListRoomItems(parent));
                return;
            }

            // First check the place where the sender is located (like a room) for the target,
            // and if not found, search the sender's children (like inventory) for the target.
            Thing thing = parent.FindChild(searchString) ?? sender.Thing.FindChild(searchString);
            if (thing != null)
            {
                // @@@ TODO: Send a SensoryEvent?
                sender.Write(thing.Description);
            }
            else
            {
                sender.Write("You cannot find " + searchString + ".");
            }
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

            return null;
        }

        /// <summary>Builds a string listing the items that are in the specified room.</summary>
        /// <param name="room">The room whose items we care about.</param>
        /// <returns>A string listing the items that are in the specified room.</returns>
        private string ListRoomItems(Thing room)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Thing thing in room.Children)
            {
                // @@@ TODO: Only list Items here...? ItemBehavior? CarryableBehavior?
                sb.Append(thing.ID.ToString().PadRight(20));
                sb.AppendLine(thing.FullName);
            }
            
            return sb.ToString();
        }
    }
}