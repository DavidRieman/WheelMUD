//-----------------------------------------------------------------------------
// <copyright file="RoleRevoke.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An action to remove a role from a player.
//   Implemented by Fastalanasa - 7/16/2009
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using System.Linq;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>An action to grant a role to a player.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("role revoke", CommandCategory.Admin)]
    [ActionAlias("rolerevoke", CommandCategory.Admin)]
    [ActionDescription("Removes a role from a player.")]
    [ActionSecurity(SecurityRole.fullAdmin)]
    public class RoleRevoke : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.RequiresAtLeastTwoArguments
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            string[] normalizedParams = NormalizeParameters(sender);
            string roleName = normalizedParams[0];
            string playerName = normalizedParams[1];

            Thing player = GameAction.GetPlayerOrMobile(playerName);
            if (player == null)
            {
                // If the player is not online, then try to load the player from the database.
                ////player = PlayerBehavior.Load(playerName);
            }

            var userControlledBehavior = player.Behaviors.FindFirst<UserControlledBehavior>();
            var existingRole = userControlledBehavior.FindRole(roleName);
            if (existingRole != null)
            {
                userControlledBehavior.Roles.Remove(existingRole);
                player.Save();
                sender.Write(string.Format("{0} had the {1} role revoked.", player.Name, existingRole.Name), true);
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

            string[] normalizedParams = NormalizeParameters(actionInput.Controller);
            string roleName = normalizedParams[0];
            string playerName = normalizedParams[1];

            Thing player = GameAction.GetPlayerOrMobile(playerName);
            if (player == null)
            {
                // If the player is not online, then load the player from the database.
                ////player = PlayerBehavior.Load(playerName);
            }

            // Rule: The targeted player must exist.
            if (player == null)
            {
                return string.Format("The player {0} does not exist.", playerName);
            }
            
            // Rule: The player cannot already have the role.
            var userControlledBehavior = player.Behaviors.FindFirst<UserControlledBehavior>();
            var existingRole = userControlledBehavior.FindRole(roleName);
            if (existingRole == null)
            {
                return string.Format("{0} does not have the {1} role.", player.Name, roleName);
            }

            return null;
        }

        /// <summary>Cleans up the parameters, so that it is easier to work with.</summary>
        /// <param name="sender">The IController that has the MUD command parameters that will be cleaned up.</param>
        /// <returns>Returns a string array that has been pasteurized.</returns>
        private static string[] NormalizeParameters(IController sender)
        {
            string normalizedInput = sender.LastActionInput.Tail.Replace("revoke", string.Empty).Trim();
            string[] normalizedParams = normalizedInput.Split(' ');
            return normalizedParams;
        }
    }
}