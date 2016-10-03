//-----------------------------------------------------------------------------
// <copyright file="CreateConsumable.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command that allows an admin to create a consumable.
//   Created: January 2007 by Foxedup.
//   Modified on 4/25/2010 by Feverdream (Add use of all enumerated consumable types.)
// </summary>
//-----------------------------------------------------------------------------

/* Disabled: Create Consumable shouldn't be a specific command. Instead, we want to have a templating system that
             allows item creation based on arbitrary templates, of which "consumable" could be one which might
             automatically come prepared with a ConsumableBehavior attached. The same templating system could be 
             used to create doors, keys, containers, exits, mobs, weapons, armor, portals, spawners, and so on.
namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Enums;
    using WheelMUD.Interfaces;

    /// <summary>A command that allows an admin to create a consumable.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("create consumable", CommandCategory.Admin)]
    [ActionDescription("@@@ Temp command.")]
    [ActionSecurity(SecurityRole.fullAdmin)]
    public class CreateConsumable : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive,
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.InitiatorMustBeBalanced,
            CommonGuards.InitiatorMustBeMobile,
            CommonGuards.RequiresAtLeastTwoArguments
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            string mat = actionInput.Tail.Trim().ToLower();

            string[] ma;
            if (mat != null && mat.Contains(" ") && (ma = mat.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)) != null && ma.Length == 2)
            {
                mat = ma[1].Trim(new char[] { '"', '\'', ' ' });
            }

            // Our consumable.
            ConsumableType ct = ConsumableType.Unknown;

            // @@@ Create shouldn't specifically 
            var userControlledBehavior = sender.Thing.Behaviors.FindFirst<UserControlledBehavior>();
            try
            {
                // Our consumable.
                ct = (ConsumableType)Enum.Parse(typeof(ConsumableType), mat, true);
            }
            catch (Exception ex)
            {
                ex.ToString();
                if (sender.Thing.Name != sender.Thing.Name)
                {
                    userControlledBehavior.Controller.Write(string.Format("{0} mummbles and waves his hand dramatically, but nothing happens.", sender.Thing.Name));
                }
                else
                {
                    userControlledBehavior.Controller.Write("You want to create what?");
                }

                return;
            }

            // Create the new consumable. 
            Thing consumable = new Thing();
            consumable.Name = mat;
            consumable.ID = "0";
            consumable.Behaviors.Add(new MovableBehavior());

            // @@@ TODO: Instead:
            ////Thing consumable = new Thing(new ConsumableBehavior()
            ////{
            ////    ConsumableType = ct;
            ////});

            // Tell the room.
            if (sender.Thing.Name != sender.Thing.Name)
            {
                userControlledBehavior.Controller.Write(string.Format("{0} creates a {1} consumable.", sender.Thing.Name, mat));
            }
            else
            {
                userControlledBehavior.Controller.Write(string.Format("You create a {0} consumable.", mat));
            }

            // Add the new item to the room. @@@ Shouldn't this create to the Entity inventory?
            sender.Thing.Parent.Add(consumable);
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
}*/