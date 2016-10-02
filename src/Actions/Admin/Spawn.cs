//-----------------------------------------------------------------------------
// <copyright file="Spawn.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Command to spawn a mobile NPC for testing.
//   Created: March 2012 by James McManus
//   TODO: Expose more options than just the name.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>Command to spawn a mobile NPC for testing.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("spawn", CommandCategory.Admin)]
    [ActionDescription("Spawns a mobile NPC for testing.\r\nUsage: spawn (name)")]
    [ActionSecurity(SecurityRole.fullAdmin)]
    public class Spawn : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.RequiresAtLeastOneArgument
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;

            string mobName = actionInput.Tail.Trim();

            var thing = new Thing();
            thing.ID = "0";
            thing.Name = mobName;
            thing.Parent = sender.Thing.Parent;
            thing.Stats["HP"] = new GameStat(sender, "Hit Points", "HP", null, 10, 0, 10, true);

            thing.Behaviors.Add(new MobileBehavior());
            thing.Behaviors.Add(new LivingBehavior() { Consciousness = Consciousness.Awake });
            thing.Behaviors.Add(new SensesBehavior());
            thing.Behaviors.Add(new WanderingBehavior());

            MobileManager.Instance.RegisterMobile(thing);

            sender.Thing.Parent.Add(thing);
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

            return null;
        }
    }
}