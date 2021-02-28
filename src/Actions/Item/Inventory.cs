//-----------------------------------------------------------------------------
// <copyright file="Inventory.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Interfaces;

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;

    /// <summary>A command to list the items in a player's inventory.</summary>
    [ExportGameAction(0)]
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
            sender.Write(Renderer.Instance.RenderInventory(sender.Thing));
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