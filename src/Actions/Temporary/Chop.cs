//-----------------------------------------------------------------------------
// <copyright file="Chop.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

// TODO: Some concept of 'busy' or 'replacing your queued action' needs to 
// be implemented if this is an action model we want to keep.  For instance, 
// right now one can use a macro to type 'chop tree' a 10 times instantly, and 
// thus gathers 10 times as much when the 2 seconds timer has elapsed.  Locking 
// out all other actions doesn't necessarily make sense, since the user might 
// be attacked by a mob during the action;  this sort of thing should cancel 
// any crafting-like actions IMO.  Thus an appropriate model might be to have a 
// 'busy doing this action' model, where entering combat always cancels such an 
// action (as it should not be used for combat actions), and if the user issues 
// any new command, it is also cancelled.

using System;
using System.Collections.Generic;
using System.Linq;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>A command to chop at a tree.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("chop", CommandCategory.Temporary)]
    [ActionDescription("Temporary test command. Chop something like a tree or log.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class Chop : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive,
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.InitiatorMustBeBalanced,
            CommonGuards.RequiresAtLeastOneArgument
        };

        /// <summary>The tree object that is going to be chopped at.</summary>
        //private Thing tree;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            /* TODO: Avoid controller.write; should implement Request+Event pattern, plus the
             * notes as described at the top of this file...
            if (command.ActionInput.Context == null)
            {
                var userControlledBehavior = sender.Thing.BehaviorManager.FindFirst<UserControlledBehavior>();
                userControlledBehavior.Controller.Write("You check your axe and begin to swing at the tree.");

                ActionInput delayedAction = new ActionInput("chop " + tree.Id, sender) { Context = 1 };
                bridge.CommandManager.Enqueue(delayedAction, new TimeSpan(0, 0, 2));
            }
            else if ((int)command.ActionInput.Context == 1)
            {
                // TODO: Use consumable provider behavior to do the chopping.
                //Item wood = tree.Chop();
                //sender.Thing.Parent.Children.Add(wood);
                //sender.Thing.Controller.Write("You chop some wood from the tree.");
            }
            */
        }

        /// <summary>Prepare for, and determine if the command's prerequisites have been met.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            var commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            // Rule: Did they specify a tree to chop?
            // TODO: Better thing finders...
            var parent = actionInput.Actor.Parent;
            var thing = parent.Children.Where(t => t.Name.Equals(actionInput.Params[0], StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (thing == null)
            {
                return $"{actionInput.Params[0]} is not here.";
            }

            // TODO: Detect ConsumableProviderBehavior on the item, and detect if it is choppable.
            //if (!(item is Item))
            //{
            //    return string.Format("{0} is not a tree.", actionInput.Params[0]);
            //}

            //tree = (Item) item;

            //if (tree.NumberOfResources <= 0)
            //{
            //    return string.Format("The tree doesn't contain any suitable wood.");
            //}

            return null;
        }
    }
}