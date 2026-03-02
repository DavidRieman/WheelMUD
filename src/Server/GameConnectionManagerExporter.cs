//-----------------------------------------------------------------------------
// <copyright file="GameConnectionManagerExporter.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;

namespace WheelMUD.Server
{
    /// <summary>
    /// A class for exporting/importing game connection manager instances through MEF, without necessarily
    /// instantiating each one. (In particular, this prevents creating multiple instances of connection managers
    /// where we generally only want to instantiate the highest-priority version.)
    /// </summary>
    public abstract class GameConnectionManagerExporter
    {
        /// <summary>Gets the connection manager instance.</summary>
        /// <returns>A new instance of the connection manager.</returns>
        public abstract IGameConnectionManager Instance { get; }

        /// <summary>Gets the Type of the connection manager, without instantiating it.</summary>
        /// <returns>The Type of the connection manager.</returns>
        public abstract Type ManagerType { get; }
    }
}
