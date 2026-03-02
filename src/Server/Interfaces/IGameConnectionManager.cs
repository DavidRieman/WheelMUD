//-----------------------------------------------------------------------------
// <copyright file="IGameConnectionManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Server
{
    /// <summary>Interface for game connection managers that handle player connections via different protocols.</summary>
    public interface IGameConnectionManager : ISubSystem
    {
        /// <summary>A 'client connected' event raised by the connection manager.</summary>
        event Action<Connection> ClientConnect;

        /// <summary>A 'client disconnected' event raised by the connection manager.</summary>
        event Action<Connection> ClientDisconnected;

        /// <summary>A 'data received' event raised by the connection manager.</summary>
        event Action<Connection> DataReceived;

        /// <summary>Closes a connection.</summary>
        /// <param name="connectionId">The ID of the connection to close.</param>
        void CloseConnection(string connectionId);
    }
}
