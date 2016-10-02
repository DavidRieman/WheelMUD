//-----------------------------------------------------------------------------
// <copyright file="Clone.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command that allows an admin to clone any cloneable item.
//   Revised: June 2009 by Karak: generic cloning instead of 'ClonePotion.cs' etc.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>A command that allows an admin to clone an item.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("clone", CommandCategory.Admin)]
    [ActionDescription("Clones an object.")]
    [ActionSecurity(SecurityRole.fullAdmin)]
    public class Clone : GameAction
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
            string itemID = actionInput.Params[0];
            IController sender = actionInput.Controller;
            Thing parent = sender.Thing.Parent;
            Thing thing = parent.FindChild(t => t.ID == itemID);
            if (thing == null)
            {
                parent = sender.Thing;
                thing = parent.FindChild(t => t.ID == itemID);
                if (thing == null)
                {
                    sender.Write(string.Format("Cannot find {0}.", itemID));
                    return;
                }
            }

            Thing clonedThing = thing.Clone();
            parent.Add(clonedThing);
            var userControlledBehavior = sender.Thing.Behaviors.FindFirst<UserControlledBehavior>();
            userControlledBehavior.Controller.Write("You clone " + thing.ID + ". New item is " + clonedThing.ID + ".");
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