//-----------------------------------------------------------------------------
// <copyright file="GameAction.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace WheelMUD.Core
{
    /// <summary>A base class that represents an instance of an Action.</summary>
    public abstract class GameAction
    {
        /// <summary>Common guards for various commands.</summary>
        protected enum CommonGuards
        {
            /// <summary>The initiator of the action must be alive.</summary>
            InitiatorMustBeAlive,

            /// <summary>The initiator of the action must be conscious.</summary>
            InitiatorMustBeConscious,

            /// <summary>The initiator of the action must be standing.</summary>
            InitiatorMustBeStanding,

            /// <summary>The initiator of the action must be balanced.</summary>
            /// <remarks>Not Implemented. Need new implementation not based on stats.</remarks>
            InitiatorMustBeBalanced,

            /// <summary>The initiator of the action must be able to move.</summary>
            /// <remarks>Not Implemented. Need new implementation not based on stats.</remarks>
            InitiatorMustBeMobile,

            /// <summary>The initiator of the action must be a player (and have a known player Session).</summary>
            InitiatorMustBeAPlayer,

            /// <summary>There must be at least one additional argument.</summary>
            RequiresAtLeastOneArgument,

            /// <summary>There must be at least two additional arguments.</summary>
            RequiresAtLeastTwoArguments
        }

        /// <summary>Gets the best match for a Player or Mobile entity from the specified name.</summary>
        /// <param name="entityName">The name of the entity to find.</param>
        /// <returns>Entity if an entity is found, otherwise null.</returns>
        public static Thing GetPlayerOrMobile(string entityName)
        {
            // Commands using this method want to find an entity with the following precedence:
            // * Player who matches the name exactly.
            // * Mobile who matches the name exactly.
            // * Player who matches the partial name.
            // * Mobile who matches the partial name.
            return PlayerManager.Instance.FindLoadedPlayerByName(entityName, false) ??
                   MobileManager.Instance.FindMobileByName(entityName, false) ??
                   PlayerManager.Instance.FindLoadedPlayerByName(entityName, true) ??
                   MobileManager.Instance.FindMobileByName(entityName, true);
        }

        /// <summary>Executes the command.</summary>
        /// <remarks>Verify that the Guards pass first.</remarks>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public abstract void Execute(ActionInput actionInput);

        /// <summary>Guards the command against incorrect usage.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>An error message describing the failure for the user, or null if all guards pass.</returns>
        public abstract string Guards(ActionInput actionInput);

        /// <summary>Verify that a set of common guards are all passed.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <param name="guards">The list of CommonGuards that all need to pass.</param>
        /// <returns>An error message describing the failure for the user, or null if all guards pass.</returns>
        /// TODO: Perhaps make this return a bool with the out being the string? Then you can check a guard with if statement instead of null check
        protected string VerifyCommonGuards(ActionInput actionInput, List<CommonGuards> guards)
        {
            var actor = actionInput.Actor;
            if (actor == null)
            {
                return "An action can only be performed by an actor.";
            }

            // Rule: Is the initiator in a room?
            // (This used to be a guard on some commands, but there does not seem to be any situation where the actor
            // should be able to use ANY command while "not in a room", and the outermost World Thing shouldn't ever
            // issue a command either, even if an admin tries to force it to.)
            if (actor.Parent == null)
            {
                return "You can't do that while you are not in the world.";
            }

            // Rule: Is the initiator a player?
            if (guards.Contains(CommonGuards.InitiatorMustBeAPlayer) && (actor.FindBehavior<PlayerBehavior>() == null || actionInput.Session == null))
            {
                return "This command can only be executed by a player with an active session.";
            }

            // Rule: Is at least two arguments supplied?
            if (guards.Contains(CommonGuards.RequiresAtLeastTwoArguments) && actionInput.Params.Length < 2)
            {
                return $"This command needs more than that. (Use 'help {actionInput.Noun}' for details.)";
            }

            // Rule: Is at least one argument supplied?
            if (guards.Contains(CommonGuards.RequiresAtLeastOneArgument) && actionInput.Params.Length <= 0)
            {
                return $"This command needs more than that. (Use 'help {actionInput.Noun}' for details.)";
            }

            // Rule: Is the initiator alive?
            var livingBehavior = actor.FindBehavior<LivingBehavior>();
            if (guards.Contains(CommonGuards.InitiatorMustBeAlive) && (livingBehavior == null || livingBehavior.Consciousness == Consciousness.Dead))
            {
                return "You are dead and can not do that.";
            }

            // Rule: Is the initiator conscious?
            // (Note that being dead and unconscious are intentionally different checks; one may want to 
            // have a command that is only available while dead, but not just unconscious, or vice versa.
            // For example, a command that lets you request a CR from an NPC or whatnot should not be 
            // available while you are simply unconscious.  Most commands though will tend to care about 
            // both states in the same way, but the option to differentiate should still be available.)
            if (guards.Contains(CommonGuards.InitiatorMustBeConscious) && (livingBehavior == null || livingBehavior.Consciousness == Consciousness.Unconscious))
            {
                return "You are unconscious and can not do that.";
            }

            return null;
        }
    }
}