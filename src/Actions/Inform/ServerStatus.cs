//-----------------------------------------------------------------------------
// <copyright file="ServerStatus.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Utilities;
using System.Collections.Generic;
using System.Management;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>A command to display the server status information.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("serverstatus", CommandCategory.Inform)]
    [ActionAlias("server status", CommandCategory.Inform)]
    [ActionDescription("See the server status.")]
    [ActionSecurity(SecurityRole.minorAdmin)]
    public class ServerStatus : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var ab = new AnsiBuilder();

            //// TODO Reference to config file
            var appName = "WheelMUD.vshost.exe";

            ab.AppendSeparator('=', "red", true);
            ab.AppendLine("System Status:");
            ab.AppendSeparator('-', "red");

            ////ManagementObjectCollection queryCollection1 = query1.Get();

            var query1 = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
            var queryCollection1 = query1.Get();
            foreach (ManagementObject mo in queryCollection1)
            {
                ab.Append($"Manufacturer : {mo["manufacturer"]}");
                ab.AppendLine($"Model : {mo["model"]}");
                ab.AppendLine($"Physical Ram : {(ulong)mo["totalphysicalmemory"] / 1024}");
            }

            ab.AppendSeparator('-', "red");
            query1 = new ManagementObjectSearcher("SELECT * FROM Win32_process where NAME = '" + appName + "'");
            queryCollection1 = query1.Get();
            foreach (ManagementObject mo in queryCollection1)
            {
                foreach (var item in mo.Properties)
                {
                    ab.AppendLine($"<%b%><%red%>{item.Name}<%b%><%yellow%>{item.Value}<%n%>");
                }
            }

            ab.AppendSeparator('-', "red");
            query1 = new ManagementObjectSearcher("SELECT * FROM Win32_timezone");
            queryCollection1 = query1.Get();
            foreach (ManagementObject mo in queryCollection1)
            {
                ab.AppendLine($"This Server lives in:{mo["caption"]}");
            }

            actionInput.Controller.Write(ab.ToString());
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            var commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            return commonFailure;
        }
    }
}