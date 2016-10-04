//-----------------------------------------------------------------------------
// <copyright file="TelnetDataStates.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A set of classes that represent the states that data moves through as it
//   is processed for Telnet Option codes.
//   Created: January 2007 by Foxedup
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server.Telnet
{
    /// <summary>The connection telnet state.</summary>
    internal abstract class ConnectionTelnetState
    {
        /// <summary>@@@ What is this?</summary>
        internal const int IAC = 255;

        /// <summary>@@@ What is this?</summary>
        internal const int SB = 250;

        /// <summary>@@@ What is this?</summary>
        internal const int SE = 240;

        /// <summary>Initializes a new instance of the ConnectionTelnetState class.</summary>
        /// <param name="parent">The parent telnet code handler which this object is created by.</param>
        protected ConnectionTelnetState(TelnetCodeHandler parent)
        {
            this.Parent = parent;
        }

        /// <summary>Gets or sets the parent telnet code handler.</summary>
        internal TelnetCodeHandler Parent { get; set; }

        /// <summary>The 'process input' method must be implemented by inheriting classes.</summary>
        /// <param name="data">The data to be processed.</param>
        public abstract void ProcessInput(byte data);
    }
}