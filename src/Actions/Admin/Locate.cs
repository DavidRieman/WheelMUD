//-----------------------------------------------------------------------------
// <copyright file="Locate.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>A command that allows an admin to locate an entity.</summary>
    [CoreExports.GameAction(0)]
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
            CommonGuards.RequiresAtLeastOneArgument,
            CommonGuards.InitiatorMustBeAPlayer
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var entity = GetPlayerOrMobile(actionInput.Params[0]);

            actionInput.Session?.WriteLine(
                entity != null ?
                    $"{entity.Name} is at {entity.Parent.Name} (ID {entity.Parent.Id})" :
                    $"You cant find {actionInput.Params[0]}.");
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