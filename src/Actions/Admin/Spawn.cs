﻿//-----------------------------------------------------------------------------
// <copyright file="Spawn.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>Command to spawn a mobile NPC for testing.</summary>
    /// <remarks>TODO: Expose more options than just the name.</remarks>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("spawn", CommandCategory.Admin)]
    [ActionDescription("Spawns a mobile NPC for testing.")]
    [ActionExample("spawn Bob")]
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
            var session = actionInput.Session;
            if (session == null) return; // This action only makes sense for player sessions.

            var mobName = actionInput.Tail.Trim();

            var thing = new Thing { Id = "0", Name = mobName };

            var targetRoom = actionInput.Actor.Parent;
            targetRoom.Add(thing);
            thing.Stats["HP"] = new GameStat(session, "Hit Points", "HP", null, 10, 0, 10, true);

            thing.Behaviors.Add(new MobileBehavior());
            thing.Behaviors.Add(new LivingBehavior() { Consciousness = Consciousness.Awake });
            thing.Behaviors.Add(new SensesBehavior());
            thing.Behaviors.Add(new WanderingBehavior());

            MobileManager.Instance.RegisterMobile(thing);

            actionInput.Actor.Parent.Add(thing);
        }

        /// <summary>Prepare for, and determine if the command's prerequisites have been met.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            return VerifyCommonGuards(actionInput, ActionGuards);
        }
    }
}