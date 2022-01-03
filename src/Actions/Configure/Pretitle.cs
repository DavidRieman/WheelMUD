//-----------------------------------------------------------------------------
// <copyright file="Pretitle.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>A command to set a player's title.</summary>
    /// <remarks>
    /// TODO: Maybe use an App.config flag to decide if this command should be something users do for themselves,
    ///   or an admin only command by default. Of course, a game system may wish to replace this with a system for
    ///   earning specific pre-set pretitles depending on in-game progress (e.g. getting knighted), for the 
    ///   command to available to the player but only selecting one from an earned list. SEE Title.cs as well.
    /// </remarks>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("pretitle", CommandCategory.Player)]
    [ActionAlias("set pretitle", CommandCategory.Player)]
    [ActionDescription("Set or view your pretitle.")]
    [ActionSecurity(SecurityRole.player)]
    public class Pretitle : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAPlayer,
        };

        private string oldPretitle;

        private string newPretitle;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var session = actionInput.Session;
            if (session == null) return; // This action only makes sense for player sessions.

            actionInput.Actor.SingularPrefix = newPretitle;

            session.WriteLine(string.IsNullOrEmpty(newPretitle) ?
                    $"Your old pretitle was \"{oldPretitle}\" and is now removed." :
                    $"Your old pretitle was \"{oldPretitle}\" and is now \"{newPretitle}\".");

            actionInput.Actor.FindBehavior<PlayerBehavior>()?.SavePlayer();
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            var commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            // Rule: The new pretitle must be empty or meet the length requirements.
            oldPretitle = actionInput.Actor.SingularPrefix;

            if (!string.IsNullOrEmpty(actionInput.Tail))
            {
                newPretitle = actionInput.Tail;

                if (newPretitle.Length < 2 || newPretitle.Length > 15)
                {
                    return "The pretitle may not be less than 2 nor more than 15 characters long.";
                }
            }

            // TODO: May want to implement 'no color' or 'no swearing' or 'no non-alpha character' rules here, etc.

            return null;
        }
    }
}