//-----------------------------------------------------------------------------
// <copyright file="ISubSystemHost.cs" company="WheelMUD Development Team">
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
    public interface ISubSystemHost
    {
        /// <summary>Allows the sub system host to receive an update when subscribed to this system.</summary>
        /// <param name="sender">The sender of the update.</param>
        /// <param name="message">The message to send to the host.</param>
        void UpdateSubSystemHost(ISubSystem sender, string message);
    }
}