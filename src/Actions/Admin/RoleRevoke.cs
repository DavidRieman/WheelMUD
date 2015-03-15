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

    /// <summary>
    /// An action to grant a role to a player.
    /// </summary>
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
            string[] normalizedParams = this.NormalizeParameters(sender);
            string role = normalizedParams[0];
            string playerName = normalizedParams[1];

            Thing player = GameAction.GetPlayerOrMobile(playerName);
            if (player == null)
            {
                // If the player is not online, then load the player from the database
                //player = PlayerBehavior.Load(playerName);
            }

            // Rule: Does the player exist in our Universe?
            // @@@ TODO: Add code to make sure the player exists.

            /* @@@ FIX
            if (Extensions.Contains(player.Roles, role))
            {
                var roleRepository = new RoleRepository();

                // @@@ TODO: The role.ToUpper is a hack. Need to create a case insensitive method for the RoleRepository.NoGen.cs class.
                RoleRecord record = roleRepository.GetByName(role.ToUpper());
                RoleRecord toDelete = null;

                foreach (var currRole in player.RoleRecords)
                {
                    if (currRole.Name == record.Name)
                    {
                        toDelete = currRole;
                    }
                }

                player.RoleRecords.Remove(toDelete);
                player.RoleRecords.TrimExcess();
                player.UpdateRoles();
                player.Save();

                sender.Write(string.Format("{0} had the {1} role revoked.", player.Name, role), true);
            }*/
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

            string[] normalizedParams = this.NormalizeParameters(actionInput.Controller);
            string role = normalizedParams[0];
            string playerName = normalizedParams[1];

            Thing player = GameAction.GetPlayerOrMobile(playerName);
            if (player == null)
            {
                // If the player is not online, then load the player from the database.
                //player = PlayerBehavior.Load(playerName);
            }

            // Rule: The targeted player must exist.
            if (player == null)
            {
                return "The player " + playerName + " does not exist.";
            }

            // Rule: The 

            // Rule: Does player already have role?
            var userControlledBehavior = player.Behaviors.FindFirst<UserControlledBehavior>();
            var existingRole = (from r in userControlledBehavior.Roles where r.Name == role select r).FirstOrDefault();
            if (existingRole == null)
            {
                return player.Name + " does not have the " + role + " role.";
            }

            return null;
        }

        /// <summary>
        /// Cleans up the parameters, so that it is easier to work with.
        /// </summary>
        /// <param name="sender">The IController that has the MUD command parameters that will be cleaned up.</param>
        /// <returns>Returns a string array that has been pasteurized.</returns>
        private string[] NormalizeParameters(IController sender)
        {
            string normalizedInput = sender.LastActionInput.Tail.Replace("revoke", string.Empty).Trim();
            string[] normalizedParams = normalizedInput.Split(' ');

            return normalizedParams;
        }
    }
}