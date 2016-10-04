//-----------------------------------------------------------------------------
// <copyright file="ISystem.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Core system interfaces.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Interfaces
{
    /// <summary>An interface describing a SubSystemHost.</summary>
    public interface ISystem : ISystemBase, ISubSystemHost
    {
        /// <summary>Subscribes this system to the specified system host, so that host can receive updates.</summary>
        /// <param name="sender">The system host to receive our updates.</param>
        void SubscribeToSystemHost(ISystemHost sender);
    }
}