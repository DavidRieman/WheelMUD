//-----------------------------------------------------------------------------
// <copyright file="Inventory.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Script to list the items in a player's inventory.
//   Created: November 2006 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>A command to list the items in a player's inventory.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("inventory", CommandCategory.Item)]
    [ActionAlias("inv", CommandCategory.Item)]
    [ActionAlias("i", CommandCategory.Item)]
    [ActionDescription("Review the items you are carrying.")]
    [ActionSecurity(SecurityRole.player)]
    public class Inventory : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive, 
            CommonGuards.InitiatorMustBeConscious
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            StringBuilder sb = new StringBuilder();
            sb.Append("Your Inventory");
            sb.Append(Environment.NewLine);

            foreach (Thing item in sender.Thing.Children)
            {
                sb.Append(item.ID.ToString().PadRight(20));
                sb.AppendLine(item.FullName);
            }

            sender.Write(sb.ToString().Trim());
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
    }
}