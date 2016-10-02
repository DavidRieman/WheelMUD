//-----------------------------------------------------------------------------
// <copyright file="RoleGrant.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An action to give a role to a player.
//   Implemented by Fastalanasa
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using System.Linq;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Data.Entities;
    using WheelMUD.Data.Repositories;
    using WheelMUD.Interfaces;

    /// <summary>
    /// An action to give a role to a player.
    /// </summary>
    [ExportGameAction]
    [ActionPrimaryAlias("role grant", CommandCategory.Admin)]
    [ActionAlias("rolegrant", CommandCategory.Admin)]
    [ActionDescription("Adds a role to a player.")]
    [ActionSecurity(SecurityRole.fullAdmin)]
    public class RoleGrant : GameAction
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
                ////player = PlayerBehavior.Load(playerName);
            }
            
            var userControlledBehavior = player.Behaviors.FindFirst<UserControlledBehavior>();
            var existingRole = (from r in userControlledBehavior.Roles where r.Name == role select r).FirstOrDefault();
            if (existingRole == null)
            {
                ////var roleRepository = new RoleRepository();

                // @@@ TODO: The role.ToUpper is a hack. Need to create a case insensitive method for the RoleRepository.NoGen.cs class.
                ////RoleRecord record = roleRepository.GetByName(role.ToUpper());
                ////userControlledBehavior.RoleRecords.Add(record);
                ////userControlledBehavior.UpdateRoles();
                player.Save();

                ////sender.Write(player.Name + " has been granted the " + role + " role.", true);
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

            string[] normalizedParams = this.NormalizeParameters(actionInput.Controller);
            ////string role = normalizedParams[0];
            string playerName = normalizedParams[1];

            Thing player = GameAction.GetPlayerOrMobile(playerName);
            if (player == null)
            {
                // If the player is not online, then load the player from the database
                ////player = PlayerBehavior.Load(playerName);
            }

            // Rule: Does the player exist in our Universe?
            // @@@ TODO: Add code to make sure the player exists.

            // Rule: Does player already have role?
            /* @@@ FIX
            if (Contains(player.Roles, role))
            {
                return player.Name + " already has the " + role + " role.";
            }*/

            return null;
        }

        /// <summary>Custom Contains function for a generic list.</summary>
        /// <remarks>Added since the .NET framework doesn't have one for generic lists.</remarks>
        /// <param name="list">The list to process.</param>
        /// <param name="value">The string value to search for.</param>
        /// <returns>True if the list contains the value, false if it doesn't.</returns>
        private static bool Contains(List<string> list, string value)
        {
            var v = value.ToLower();
            return null != list.Find(str => str.ToLower().Equals(v));
        }
 
        /// <summary>Cleans up the parameters, so that it is easier to work with.</summary>
        /// <param name="sender">The IController that has the MUD command parameters that will be cleaned up.</param>
        /// <returns>Returns a string array that has been pasteurized.</returns>
        private string[] NormalizeParameters(IController sender)
        {
            string normalizedInput = sender.LastActionInput.Tail.Replace("grant", string.Empty).Trim();
            string[] normalizedParams = normalizedInput.Split(' ');
            return normalizedParams;
        }
    }
}