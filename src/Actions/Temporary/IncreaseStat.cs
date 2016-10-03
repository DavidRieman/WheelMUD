//-----------------------------------------------------------------------------
// <copyright file="IncreaseStat.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   @@@ Temporary script to test the stat effects.
//   Created: November 2006 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Effects;
    using WheelMUD.Interfaces;

    /// <summary>@@@ Temporary script to test the stat effects.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("increase stat", CommandCategory.Admin)]
    [ActionAlias("increasestat", CommandCategory.Admin)]
    [ActionDescription("@@@ Temp command.")]
    [ActionSecurity(SecurityRole.fullAdmin)]
    public class IncreaseStat : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            //StatEffect effect;

            var statEffect = sender.Thing.Behaviors.FindFirst<StatEffect>();
            if (statEffect != null)
            {
                // @@@ ??
            } 
            else 
            {
                statEffect = new StatEffect();
                //{
                //    Modifier = 15,
                //    Name = "strength",
                //};
            }
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