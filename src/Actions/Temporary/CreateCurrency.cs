//-----------------------------------------------------------------------------
// <copyright file="CreateCurrency.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command that allows an admin to create currency.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    /* @@@ This command will be unnecessary with proper OLC item creation, etc...
    using System.Collections.Generic;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;
    using WheelMUD.Data.Entities;
    using WheelMUD.Universe.Things;
    using WheelMUD.Universe;
    using WheelMUD.Universe.Behaviors;

    /// <summary>A command that allows an admin to create currency.</summary>
    [ActionPrimaryAlias("create currency", CommandCategory.Admin)]
    [ActionAlias("create money", CommandCategory.Admin)]
    [ActionAlias("create gold", CommandCategory.Admin)]
    [ActionDescription("@@@ Temp command.")]
    [ActionSecurity(SecurityRole.fullAdmin)]
    internal class CreateCurrency : Action
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
            int amount = 5000;

            Thing currency = new Thing(new CurrencyBehavior())
            {
                Count = amount,
            };
            
            sender.Thing.Parent.Add(currency);
            sender.Thing.Controller.Write(string.Format("You create {0}.", currency.Description));
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
    }*/
}