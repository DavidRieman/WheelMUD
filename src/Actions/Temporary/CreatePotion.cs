//-----------------------------------------------------------------------------
// <copyright file="CreatePotion.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command that allows an admin to create a potion.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;
    using WheelMUD.Universe;

    /// <summary>A command that allows an admin to create a potion.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("create potion", CommandCategory.Admin)]
    [ActionDescription("@@@ Temp command.")]
    [ActionSecurity(SecurityRole.fullAdmin)]
    public class CreatePotion : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive, 
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.InitiatorMustBeBalanced,
            CommonGuards.InitiatorMustBeMobile,
            CommonGuards.RequiresAtLeastOneArgument
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            Thing potionItem = new Thing(new PotionBehavior()
            {
                PotionType = "health",
                Modifier = 30,
                MaxSips = 50,
                SipsLeft = 50,
                Duration = new TimeSpan(0, 0, 15),
            });

            sender.Thing.Parent.Children.Add(potionItem);

            var userControlledBehavior = sender.Thing.Behaviors.FindFirst<UserControlledBehavior>();
            userControlledBehavior.Controller.Write("You create a colourful potion");
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