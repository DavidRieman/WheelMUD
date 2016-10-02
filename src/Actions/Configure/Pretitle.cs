//-----------------------------------------------------------------------------
// <copyright file="Pretitle.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command to set a player's title.
//   Created: February 2007 by Saquivor.
//   Modified: December 2009 by bengecko
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>A command to set a player's title.</summary>
    [ExportGameAction]
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

        private Thing player;

        private string oldPretitle;

        private string newPretitle;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            this.player.SingularPrefix = this.newPretitle;

            if (string.IsNullOrEmpty(this.newPretitle))
            {
                sender.Write(string.Format("Your old pretitle was \"{0}\" and is now removed.", this.oldPretitle));
            }
            else
            {
                sender.Write(string.Format("Your old pretitle was \"{0}\" and is now \"{1}\".", this.oldPretitle, this.newPretitle));
            }

            this.player.Save();
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            string commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            // The comon guards already guarantees the sender is a player, hence no null checks here.
            this.player = sender.Thing;

            // Rule: The new pretitle must be empty or meet the length requirements.
            this.oldPretitle = this.player.SingularPrefix;

            if (!string.IsNullOrEmpty(actionInput.Tail))
            {
                this.newPretitle = actionInput.Tail;

                if (this.newPretitle.Length < 2 || this.newPretitle.Length > 15)
                {
                    return "The pretitle may not be less than 2 nor more than 15 characters long.";
                }
            }

            //// One could implement 'no color' or 'no swearing' or 'no non-alpha character' rules here, etc.

            return null;
        }
    }
}