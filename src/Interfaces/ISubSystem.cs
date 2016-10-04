//-----------------------------------------------------------------------------
// <copyright file="ISubSystem.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Core system interfaces.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Interfaces
{
    /// <summary>An interface describing a SubSystem.</summary>
    public interface ISubSystem : ISystemBase
    {
        /// <summary>Subscribe to receive system updates from this system.</summary>
        /// <param name="sender">The subscribing system; generally use 'this'.</param>
        void SubscribeToSystem(ISubSystemHost sender);

        /// <summary>Inform subscribed system(s) of the specified update.</summary>
        /// <param name="msg">The message to be sent to subscribed system(s).</param>
        void InformSubscribedSystem(string msg);
    }
}