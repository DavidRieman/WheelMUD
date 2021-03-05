//-----------------------------------------------------------------------------
// <copyright file="Save.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>An action to save your game.</summary>
    /// <remarks>TODO: Add auto-save system, allowing this to be an optional command.</remarks>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("save", CommandCategory.Configure)]
    [ActionDescription("Save your game.")]
    [ActionSecurity(SecurityRole.player)]
    public class Save : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>();

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            //TODO: Inform player of save
            actionInput.Controller.Thing.FindBehavior<PlayerBehavior>()?.SavePlayer();
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            return VerifyCommonGuards(actionInput, ActionGuards);
        }
    }
}