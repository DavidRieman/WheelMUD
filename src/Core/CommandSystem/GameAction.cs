//-----------------------------------------------------------------------------
// <copyright file="GameAction.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A base class that represents an instance of an Action.
//   Created: April 2009 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Interfaces;

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

            /// <summary>The initiator of the action must be a player.</summary>
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
            return PlayerManager.Instance.FindPlayerByName(entityName, false) ??
                   MobileManager.Instance.FindMobileByName(entityName, false) ??
                   PlayerManager.Instance.FindPlayerByName(entityName, true) ??
                   MobileManager.Instance.FindMobileByName(entityName, true);
        }

        /// <summary>Gets a room for the specified ID.</summary>
        /// <param name="roomId">The ID of the room to find.</param>
        /// <returns>Room if a room is found, otherwise null</returns>
        public static Thing GetRoom(long roomId)
        {
            return PlacesManager.Instance.WorldBehavior.FindRoom(roomId);
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
        protected string VerifyCommonGuards(ActionInput actionInput, List<CommonGuards> guards)
        {
            // Rule: Is the sender and sender entity specified?  (This shouldn't be allowed to happen ever?)
            IController sender = actionInput.Controller;
            if (sender == null || sender.Thing == null)
            {
                return "A non-entity can not send this command.";
            }

            // Rule: Is the initiator in a room?
            // (@@@ Note that this guard was found on some commands, but I know of no situation where the user
            // should be able to use ANY command while "not in a room" which generally shouldn't occur?)
            if (sender.Thing.Parent == null)
            {
                return "You can't do that while you are not in a room.";
            }

            // Rule: Is the initiator a player?
            if (guards.Contains(CommonGuards.InitiatorMustBeAPlayer) && sender.Thing.Behaviors.FindFirst<PlayerBehavior>() == null)
            {
                return "This command can only be executed by a player.";
            }

            // Rule: Is at least two arguments supplied?
            if (guards.Contains(CommonGuards.RequiresAtLeastTwoArguments) && actionInput.Params.Length < 2)
            {
                return string.Format("This command needs more than that. (Use 'help {0}' for details.)", actionInput.Noun);
            }

            // Rule: Is at least one argument supplied?
            if (guards.Contains(CommonGuards.RequiresAtLeastOneArgument) && actionInput.Params.Length <= 0)
            {
                return string.Format("This command needs more than that. (Use 'help {0}' for details.)", actionInput.Noun);
            }

            // Rule: Is the initiator alive?
            if (guards.Contains(CommonGuards.InitiatorMustBeAlive) && sender.LivingBehavior.Consciousness == Consciousness.Dead)
            {
                return "You are dead and can not do that.";
            }

            // Rule: Is the initiator conscious?
            // (Note that being dead and unconscious are intentionally different checks; one may want to 
            // have a command that is only available while dead, but not just unconscious, or vice versa.
            // For example, a command that lets you request a CR from an NPC or whatnot should not be 
            // available while you are simply unconscious.  Most commands though will tend to care about 
            // both states in the same way, but the option to differentiate should still be available.)
            if (guards.Contains(CommonGuards.InitiatorMustBeConscious) && sender.LivingBehavior.Consciousness == Consciousness.Unconscious)
            {
                return "You are unconscious and can not do that.";
            }

            return null;
        }
    }
}