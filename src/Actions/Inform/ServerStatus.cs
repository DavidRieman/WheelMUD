//-----------------------------------------------------------------------------
// <copyright file="ServerStatus.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

/* TODO: https://github.com/DavidRieman/WheelMUD/issues/177: Revise and enable?
using System.Collections.Generic;
using System.Management;
using WheelMUD.Core;
using WheelMUD.Server;

namespace WheelMUD.Actions
{
    /// <summary>A command to display the server status information.</summary>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("serverstatus", CommandCategory.Inform)]
    [ActionAlias("server status", CommandCategory.Inform)]
    [ActionDescription("See the server status.")]
    [ActionSecurity(SecurityRole.minorAdmin)]
    public class ServerStatus : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>();

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var session = actionInput.Session;
            if (session == null) return; // This action only makes sense for player sessions.

            var output = new OutputBuilder();

            //// TODO Reference to config file
            var appName = "WheelMUD.vshost.exe";

            output.AppendSeparator('=', "red", true);
            output.AppendLine("System Status:");
            output.AppendSeparator('-', "red");

            ////ManagementObjectCollection queryCollection1 = query1.Get();

            var query1 = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
            var queryCollection1 = query1.Get();
            foreach (ManagementObject mo in queryCollection1)
            {
                output.Append($"Manufacturer : {mo["manufacturer"]}");
                output.AppendLine($"Model : {mo["model"]}");
                output.AppendLine($"Physical Ram : {(ulong)mo["totalphysicalmemory"] / 1024}");
            }

            output.AppendSeparator('-', "red");
            query1 = new ManagementObjectSearcher("SELECT * FROM Win32_process where NAME = '" + appName + "'");
            queryCollection1 = query1.Get();
            foreach (ManagementObject mo in queryCollection1)
            {
                foreach (var item in mo.Properties)
                {
                    output.AppendLine($"<%b%><%red%>{item.Name}<%b%><%yellow%>{item.Value}<%n%>");
                }
            }

            output.AppendSeparator('-', "red");
            query1 = new ManagementObjectSearcher("SELECT * FROM Win32_timezone");
            queryCollection1 = query1.Get();
            foreach (ManagementObject mo in queryCollection1)
            {
                output.AppendLine($"This Server lives in:{mo["caption"]}");
            }

            session.Write(output);
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            return VerifyCommonGuards(actionInput, ActionGuards);
        }
    }
}*/
