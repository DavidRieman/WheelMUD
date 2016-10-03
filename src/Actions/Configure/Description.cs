//-----------------------------------------------------------------------------
// <copyright file="Description.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An action to change your character's description.
//   @@@ TODO: Implement
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>An action to change your character's description.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("description", CommandCategory.Configure)]
    [ActionAlias("descript", CommandCategory.Configure)]
    [ActionDescription("Change your character's description.")]
    [ActionSecurity(SecurityRole.player)]
    public class Description : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
        };

        /// <summary>Gets or sets the new description that will be used.</summary>
        private string NewDescription { get; set; }

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            if (sender != null && sender.Thing != null)
            {
                Thing entity = sender.Thing;

                if (entity != null)
                {
                    if (!string.IsNullOrEmpty(this.NewDescription))
                    {
                        entity.Description = this.NewDescription;
                        entity.Save();
                        sender.Write("Description successfully changed.");
                    }
                    else
                    {
                        sender.Write(string.Format("Your current description is \"{0}\".", entity.Description));
                    }
                }
                else
                {
                    sender.Write("Unexpected error occurred changing description, please contact admin.");
                }
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

            this.NewDescription = actionInput.Tail;

            return null;
        }
    }
}