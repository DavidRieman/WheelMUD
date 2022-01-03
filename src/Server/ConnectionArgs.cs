//-----------------------------------------------------------------------------
// <copyright file="ConnectionArgs.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using WheelMUD.Server.Interfaces;

namespace WheelMUD.Server
{
    /// <summary>The connection args.</summary>
    public class ConnectionArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the ConnectionArgs class.</summary>
        /// <param name="connection">The connection.</param>
        public ConnectionArgs(IConnection connection)
        {
            Connection = connection;
        }

        /// <summary>Gets the connection.</summary>
        public IConnection Connection { get; private set; }
    }
}