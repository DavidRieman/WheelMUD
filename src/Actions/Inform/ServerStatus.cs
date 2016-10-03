//-----------------------------------------------------------------------------
// <copyright file="ServerStatus.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A script to show server status information.
//   SQL status including DB IIS. Need to add try catch error handling to WMI connection.
//   Created: January 2007 by Saquivor.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using System.Management;
    using System.Text;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;

    /// <summary>A command to display the server status information.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("serverstatus", CommandCategory.Inform)]
    [ActionAlias("server status", CommandCategory.Inform)]
    [ActionDescription("See the server status.")]
    [ActionSecurity(SecurityRole.player)]
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
            StringBuilder sb = new StringBuilder();
            ulong totalMemory;
            //// TODO Reference to config file
            string appName = string.Empty;
            string div = string.Empty;

            appName = "TestHarness.vshost.exe";
            div = "<%b%><%red%>~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~<%n%>";

            sb.AppendLine(div);
            sb.AppendLine("System Status:");
            sb.AppendLine(div);
            ManagementObjectSearcher query1; // = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
            ManagementObjectCollection queryCollection1;

            ////ManagementObjectCollection queryCollection1 = query1.Get();

            query1 = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
            queryCollection1 = query1.Get();
            foreach (ManagementObject mo in queryCollection1)
            {
                sb.Append("Manufacturer : " + mo["manufacturer"].ToString() + " - ");
                sb.Append("Model : " + mo["model"].ToString() + " - ");
                //// UInt64.TryParse
                totalMemory = (ulong)mo["totalphysicalmemory"] / 1024;
                sb.AppendLine("Physical Ram : " + totalMemory.ToString());
            }
            
            sb.AppendLine(div);
            query1 = new ManagementObjectSearcher("SELECT * FROM Win32_process where NAME = '" + appName + "'");
            queryCollection1 = query1.Get();
            foreach (ManagementObject mo in queryCollection1)
            {
                foreach (PropertyData item in mo.Properties)
                {
                    sb.AppendLine("<%b%><%red%>" + item.Name + " - " + "<%b%><%yellow%>" + item.Value + "<%n%>");
                }
            }

            sb.AppendLine(div);
            query1 = new ManagementObjectSearcher("SELECT * FROM Win32_timezone");
            queryCollection1 = query1.Get();
            foreach (ManagementObject mo in queryCollection1)
            {
                sb.AppendLine("This Server lives in:" + mo["caption"].ToString());
            }
            
            actionInput.Controller.Write(sb.ToString().TrimEnd(null));
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