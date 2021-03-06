// -----------------------------------------------------------------------
// <copyright file="CreateWeapon.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using WarriorRogueMage.Behaviors;
using WheelMUD.Core;
using WheelMUD.Interfaces;
using WheelMUD.Server;

namespace WheelMUD.Actions.Temporary
{
    /// <summary>A command that allows an admin to create a simple weapon for testing.</summary>
    [ExportGameAction(100)]
    [ActionPrimaryAlias("create weapon", CommandCategory.Admin)]
    [ActionDescription("Temporary command to create a simple weapon for testing.")]
    [ActionSecurity(SecurityRole.fullAdmin)]
    public class CreateWeapon : GameAction
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
            var sender = actionInput.Controller;

            // Remove "weapon" from input tail and use the rest as the name.
            var weaponName = actionInput.Tail[6..].Trim().ToLower();

            var weaponItem = new Thing(new WieldableBehavior(), new MovableBehavior())
            {
                Name = weaponName, SingularPrefix = "a", Id = "0"
            };
            //weaponItem.ID = Guid.NewGuid().ToString();

            var wasAdded = sender.Thing.Parent.Add(weaponItem);

            var userControlledBehavior = sender.Thing.Behaviors.FindFirst<UserControlledBehavior>();
            if (wasAdded)
            {
                userControlledBehavior.Controller.Write(new OutputBuilder().AppendLine($"You create a weapon called {weaponItem.Name}."));
            }
            else
            {
                userControlledBehavior.Controller.Write(new OutputBuilder().AppendLine($"Could not add {weaponItem.Name} to the room!"));
            }
        }

        /// <summary>Prepare for, and determine if the command's prerequisites have been met.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            return VerifyCommonGuards(actionInput, ActionGuards);
        }
    }
}
