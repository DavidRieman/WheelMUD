//-----------------------------------------------------------------------------
// <copyright file="Tunnel.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>A command that creates an exit between two Things.</summary>
    /// <remarks>
    /// TODO: Ability to specify an exit template (for example, including some door behaviors).
    /// TODO: Ability to specify a room template to auto-create a new room in the specified direction too.
    /// TODO: Ability to specify this is a one-way exit.
    /// TODO: Allow minor builder to work ONLY from and to entities attached to their own unpublished Areas. Scale other permissions...
    /// </remarks>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("tunnel", CommandCategory.Builder)]
    [ActionAlias("make-exit", CommandCategory.Player)]
    [ActionDescription("Creates an exit from the current room to the specified room ID. Usage: tunnel [direction] [targetID]")]
    [ActionSecurity(SecurityRole.minorBuilder)]
    public class Tunnel : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.RequiresAtLeastTwoArguments
        };

        /// <summary>The target room.</summary>
        private Thing target;

        private string tunnelDir;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var exitBehavior = new ExitBehavior();
            var from = actionInput.Actor.Parent;

            var mirrorDir = ExitBehavior.MirrorDirectionMap[tunnelDir];
            exitBehavior.AddDestination(tunnelDir, target.Id);
            exitBehavior.AddDestination(mirrorDir, from.Id);

            // Attach the new two-way exit to both locations.
            var exit = new Thing(new MultipleParentsBehavior(), exitBehavior);
            from.Add(exit);
            target.Add(exit);
            actionInput.Session.WriteLine($"Created exit {tunnelDir} to {target.Name} ({target.Id}) and back.");

            from.Save();
            target.Save();
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

            string targetId = actionInput.Params[1];
            target = ThingManager.Instance.FindThing(targetId);
            if (target == null)
            {
                return $"Could not find target with ID `{targetId}`";
            }

            tunnelDir = ExitBehavior.NormalizeDirection(actionInput.Params[0].ToLower());
            if (!ExitBehavior.MirrorDirectionMap.ContainsKey(tunnelDir))
            {
                var validOptions = string.Join(", ", ExitBehavior.MirrorDirectionMap.Keys);
                return $"Tunnel only creates two-way exits, from invertible options: {validOptions}.";
            }

            return null;
        }
    }
}
