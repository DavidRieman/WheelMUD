//-----------------------------------------------------------------------------
// <copyright file="ISystemHost.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Core system interfaces.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Interfaces
{
    /// <summary>An interface describing a SystemHost.</summary>
    public interface ISystemHost
    {
        /// <summary>Send an update to the system host.</summary>
        /// <param name="sender">The sending system.</param>
        /// <param name="msg">The message to be sent.</param>
        void UpdateSystemHost(ISystem sender, string msg);
    }
}