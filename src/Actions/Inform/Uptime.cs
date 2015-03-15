//-----------------------------------------------------------------------------
// <copyright file="Uptime.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   Lists how long has the game been running.
//   Created: July 2010 by Fastalanasa.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using WheelMUD.Configuration;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Interfaces;

    [ActionPrimaryAlias("uptime", CommandCategory.Inform)]
    [ActionDescription("Lists how long has the game been running.")]
    [ActionSecurity(SecurityRole.all)]
    public class Uptime : Action
    {
        /// <summary>The list of common guards that must always be passed for instances of this action.</summary>
        private static readonly List<CommonGuards> commonGuards = new List<CommonGuards> { };

        /// <summary>Executes the command.</summary>
        /// <param name="sender">Sender of the command.</param>
        /// <param name="bridge">The system to script bridge.</param>
        /// <param name="command">The full command requested.</param>
        public override void Execute(IController sender, ISystemToScriptBridge bridge, ICommand command)
        {
            string name = new MudEngineAttributes().MudName;
            string duration = CalculateDuration(bridge.ServerManager.StartTime);
            string output = string.Format("{0} has been running for {1}.", name, duration);

            sender.Write(output);
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="sender">Sender of the command.</param>
        /// <param name="bridge">The system to script bridge.</param>
        /// <param name="command">The full command requested.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(IController sender, ISystemToScriptBridge bridge, ICommand command)
        {
            string commonFailure = VerifyCommonGuards(sender, bridge, command, commonGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            return null;
        }

        private static string CalculateDuration(DateTime startTime)
        {
            var sb = new StringBuilder();
            TimeSpan duration = DateTime.Now - startTime;

            if (duration.Days > 0)
            {
                if (duration.Days == 1)
                {
                    sb.Append(string.Format("{0} day ", duration.Days));
                }
                else
                {
                    sb.Append(string.Format("{0} days ", duration.Days));
                }
            }

            if (duration.Hours > 0)
            {
                if (duration.Hours == 1)
                {
                    sb.Append(string.Format("{0} hour ", duration.Hours));
                }
                else
                {
                    sb.Append(string.Format("{0} hours ", duration.Hours));
                }
            }

            if (duration.Minutes > 0)
            {
                if (duration.Minutes == 1)
                {
                    sb.Append(string.Format("{0} minute ", duration.Minutes));
                }
                else
                {
                    sb.Append(string.Format("{0} minutes ", duration.Minutes));
                }
            }

            if (duration.Seconds > 0)
            {
                if (duration.Seconds == 1)
                {
                    sb.Append(string.Format("{0} second ", duration.Seconds));
                }
                else
                {
                    sb.Append(string.Format("{0} seconds ", duration.Seconds));
                }
            }

            return sb.ToString().TrimEnd();
        }
    }
}