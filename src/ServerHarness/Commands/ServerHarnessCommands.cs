//-----------------------------------------------------------------------------
// <copyright file="ServerHarnessCommands.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using WheelMUD.Core;
using WheelMUD.Utilities.Interfaces;

namespace ServerHarness
{
    public class ServerHarnessCommands : IRecomposable
    {
        /// <summary>Gets the singleton instance of the ServerHarnessCommands class.</summary>
        public static ServerHarnessCommands Instance { get; } = new ServerHarnessCommands();

        /// <summary>Prevents a default instance of the ServerHarnessCommands class from being created.</summary>
        private ServerHarnessCommands()
        {
            Recompose();
        }

        /// <summary>Gets, via MEF composition, a list of available server harness commands.</summary>
        [ImportMany]
        private List<IServerHarnessCommand> ComposedCommands { get; set; }

        /// <summary>Provides a place to register commands that are formed dynamically at runtime (such as a means to exit the server harness).</summary>
        public List<DynamicServerHarnessCommand> DynamicCommands { get; private set; } = new List<DynamicServerHarnessCommand>();

        /// <summary>Retrieves an array of all available server harness commands (including those found with MEF and dynamically).</summary>
        public IServerHarnessCommand[] AllCommands
        {
            get
            {
                lock (this)
                {
                    return (from command in ComposedCommands.Union(DynamicCommands)
                            orderby command.Names.First()
                            select command).ToArray();
                }
            }
        }

        public void Recompose()
        {
            lock (this)
            {
                DefaultComposer.Container.ComposeParts(this);
            }
        }
    }
}
