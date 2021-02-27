//-----------------------------------------------------------------------------
// <copyright file="RoleGrant.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Core.Interfaces;

namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;

    /// <summary>An administrative action which gives a role to a player.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("role grant", CommandCategory.Admin)]
    [ActionAlias("rolegrant", CommandCategory.Admin)]
    [ActionDescription("Adds a role to a player.")]
    [ActionSecurity(SecurityRole.fullAdmin)]
    public class RoleGrant : GameAction
    {
        private Thing player;
        private SecurityRole role;

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
            var userControlledBehavior = player.Behaviors.FindFirst<UserControlledBehavior>();
            userControlledBehavior.SecurityRoles |= role;
            sender.Write($"{player.Name} has been granted the {role.ToString()} role.");
            sender.Write($"{player.Name} is now: {userControlledBehavior.SecurityRoles}.");
            // TODO: Should this notify the target user too?
            player.FindBehavior<PlayerBehavior>()?.SavePlayer();
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

            // Rule: The targeted player must exist and be online. (For safety, this must be a full name match only.)
            // TODO: Consider a mode where the player document exists in the DB is enough; add ability to modify said doc.
            player = PlayerManager.Instance.FindLoadedPlayerByName(playerName, false);
            if (player == null)
            {
                return $"The player '{playerName}' does not seem to be online. (Exact name must be used for role changes.)";
            }

            // Rule: The roleName must be a valid role.
            if (!Enum.TryParse(roleName, true, out role))
            {
                string rolesList = string.Join(", ", SecurityRoleHelpers.IndividualSecurityRoles);
                return $"The role '{roleName}' is not a valid role. Try one of: {rolesList}";
            }

            return null;
        }

        /// <summary>Cleans up the parameters, so that it is easier to work with.</summary>
        /// <param name="sender">The IController that has the MUD command parameters that will be cleaned up.</param>
        /// <returns>Returns a string array that has been pasteurized.</returns>
        private static string[] NormalizeParameters(IController sender)
        {
            string normalizedInput = sender.LastActionInput.Tail.Replace("grant", string.Empty).Trim();
            string[] normalizedParams = normalizedInput.Split(' ');
            return normalizedParams;
        }
    }
}