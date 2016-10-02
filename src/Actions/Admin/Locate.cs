//-----------------------------------------------------------------------------
// <copyright file="Locate.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command that allows an admin to locate an entity.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>A command that allows an admin to locate an entity.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("locate", CommandCategory.Admin)]
    [ActionAlias("whereis", CommandCategory.Admin)]
    [ActionAlias("where is", CommandCategory.Admin)]
    [ActionDescription("Locate the specified entity or item.")]
    [ActionSecurity(SecurityRole.fullAdmin | SecurityRole.minorAdmin)]
    public class Locate : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.RequiresAtLeastOneArgument
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            Thing entity = GameAction.GetPlayerOrMobile(sender.LastActionInput.Tail);

            if (entity != null)
            {
                sender.Write(string.Format("You see {0} at {1}, id {2}", entity.Name, entity.Parent.Name, entity.Parent.ID));
            }
            else
            {
                sender.Write("You cant find " + sender.LastActionInput.Tail);
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
    }
}