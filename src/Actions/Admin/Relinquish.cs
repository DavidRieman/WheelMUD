//-----------------------------------------------------------------------------
// <copyright file="Relinquish.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An action to relinquish control of a mobile or player.
//   @@@ TODO: Implement
//   @@@ TODO - special - might want to be available to players, depending on 
//       how security for actions is done during possession...  but probably 
//       shouldn't appear in the help list.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;

    /// <summary>An action to relinquish control of a mobile or player.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("relinquish", CommandCategory.Admin)]
    [ActionAlias("relinquish control", CommandCategory.Admin)]
    [ActionAlias("dispossess", CommandCategory.Admin)]
    [ActionDescription("Release control of a mobile or player.")]
    [ActionSecurity(SecurityRole.mobile)]
    public class Relinquish : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // @@@ TODO: Implement.
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