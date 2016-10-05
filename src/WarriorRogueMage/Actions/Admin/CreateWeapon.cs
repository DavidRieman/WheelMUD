// -----------------------------------------------------------------------
// <copyright file="CreateWeapon.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command that allows an admin to create a simple weapon for testing.
// </summary>
// -----------------------------------------------------------------------

namespace WheelMUD.Actions.Temporary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using WarriorRogueMage.Behaviors;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>A command that allows an admin to create a simple weapon for testing.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("create weapon", CommandCategory.Admin)]
    [ActionDescription("@@@ Temp command to create a simple weapon for testing.")]
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
            IController sender = actionInput.Controller;

            // Remove "weapon" from input tail and use the rest as the name.
            string weaponName = actionInput.Tail.Substring(6).Trim().ToLower();

            Thing weaponItem = new Thing(new WieldableBehavior(), new MovableBehavior());
            weaponItem.Name = weaponName;
            weaponItem.SingularPrefix = "a";
            //weaponItem.ID = Guid.NewGuid().ToString();
            weaponItem.ID = "0";

            var wasAdded = sender.Thing.Parent.Add(weaponItem);

            var userControlledBehavior = sender.Thing.Behaviors.FindFirst<UserControlledBehavior>();
            if (wasAdded)
            {
                userControlledBehavior.Controller.Write(string.Format("You create a weapon called {0}.", weaponItem.Name));
            }
            else
            {
                userControlledBehavior.Controller.Write(string.Format("Could not add {0} to the room!", weaponItem.Name));
            }
        }

        /// <summary>Prepare for, and determine if the command's prerequisites have been met.</summary>
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
