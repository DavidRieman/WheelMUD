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
    using WheelMUD.Interfaces;

    /// <summary>An administrative action which gives a role to a player.</summary>
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
            if (existingRole == null)
            {
                userControlledBehavior.Roles.Add(new Role()
                {
                    Name = roleName
                });
                player.Save();
                sender.Write(player.Name + " has been granted the " + roleName + " role.", true);
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
                // If the player is not online, then load the player from the database
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
            if (existingRole != null)
            {
                return string.Format("{0} already has the {1} role.", player.Name, roleName);
            }
            
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
        private static string[] NormalizeParameters(IController sender)
        {
            string normalizedInput = sender.LastActionInput.Tail.Replace("grant", string.Empty).Trim();
            string[] normalizedParams = normalizedInput.Split(' ');
            return normalizedParams;
        }
    }
}