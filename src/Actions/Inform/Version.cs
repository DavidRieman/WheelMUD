//-----------------------------------------------------------------------------
// <copyright file="Version.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Permissive License.  All other rights reserved.
// </copyright>
// <summary>
//   A script to allow a player to see what is the MUD engine's version.
//   Created: July 2010 by Fastalanasa.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Configuration;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Interfaces;

    [ActionPrimaryAlias("version", CommandCategory.Inform)]
    [ActionAlias("ver", CommandCategory.Inform)]
    [ActionDescription("See the MUD engine's version number.")]
    [ActionSecurity(SecurityRole.all)]
    public class Version : Action
    {
        /// <summary>The list of common guards that must always be passed for instances of this action.</summary>
        private static readonly List<CommonGuards> commonGuards = new List<CommonGuards>
        {};

        /// <summary>Executes the command.</summary>
        /// <param name="sender">Sender of the command.</param>
        /// <param name="bridge">The system to script bridge.</param>
        /// <param name="command">The full command requested.</param>
        public override void Execute(IController sender, ISystemToScriptBridge bridge, ICommand command)
        {
            string version = this.GetType().Assembly.GetName().Version.ToString();
            string name = new MudEngineAttributes().MudName;
            string output = string.Format("{0} is running version {1} of the WheelMUD Engine.", name, version);

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
    }
}
