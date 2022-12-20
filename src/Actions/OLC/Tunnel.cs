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

        /// <summary>
        /// Provides a reusable means for code to tunnel exists.
        /// Useful for this command itself, but also for base area creation code, randomized instanced area mazes, etc.
        /// </summary>
        /// <param name="actionInput"></param>
        public static void TwoWay(Thing fromThing, string tunnelDirection, Thing toThing)
        {
            // Exits persistence depends on room IDs to be effective. (Exits that don't have an ID after deserializing
            // will just get deleted.) So it is best to minimize attachment of exits against unsaved rooms. As such, we
            // will automatically try to save any room with no ID that we are going to attach to, BEFORE we start to
            // attach the exit to it (to avoid save loop complexity). Targets still will not always have an Id. For
            // example, ephemeral rooms (Persistence=false) might be used for instancing or random mazes and such: In
            // such cases, indeed, we don't want to persist the exits that just get created at new-instancing-time or
            // at maze-randomization-time as those pieces of code should be reattaching the exits when they are ready.
            if (fromThing.PersistedId == null) { fromThing.Save(); }
            if (toThing.PersistedId == null) { toThing.Save(); }

            var exitBehavior = new ExitBehavior();
            var mirrorDirection = ExitBehavior.MirrorDirectionMap[tunnelDirection];
            exitBehavior.AddDestination(tunnelDirection, toThing);
            exitBehavior.AddDestination(mirrorDirection, fromThing);

            // Prepare a new exit Thing with the default set of exit behaviors.
            var exit = new Thing(new MultipleParentsBehavior(), exitBehavior);
            if (fromThing.Persists && toThing.Persists)
            {
                // If the linked exists persist, so too will the exit between them. (If either one does not persist,
                // then trying to persist the exit too could lead to unlinked exits accumulating across future server
                // restarts.) Assign the ID pattern that will get an auto-assigned index from the DB, and save it
                // immediately so the existing rooms can refer to it by ID.
                exit.Id = "exits|";
                exit.Save();
            }

            fromThing.Add(exit);
            toThing.Add(exit);
            fromThing.Save();
            toThing.Save();
        }

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            TwoWay(actionInput.Actor.Parent, tunnelDir, target);
            actionInput.Session.WriteLine($"Created exit {tunnelDir} to {target.Name} ({target.Id}) and back.");
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
