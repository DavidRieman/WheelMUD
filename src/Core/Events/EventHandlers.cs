//-----------------------------------------------------------------------------
// <copyright file="EventHandlers.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Interfaces;

namespace WheelMUD.Core
{
    /// <summary>A handler for GameEvents.</summary>
    /// <param name="root">The root Thing where this event broadcast started.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void GameEventHandler(Thing root, GameEvent e);

    /// <summary>A handler for CancellableGameEvents.</summary>
    /// <param name="root">The root Thing where this event broadcast started.</param>
    /// <param name="e">The cancelable event/request arguments.</param>
    public delegate void CancellableGameEventHandler(Thing root, CancellableGameEvent e);

    /// <summary>An 'action received' event handler.</summary>
    /// <param name="sender">The sender of the action.</param>
    /// <param name="actionInput">The action input received.</param>
    public delegate void ActionReceivedEventHandler(IController sender, ActionInput actionInput);

    /// <summary>The 'RPC session authenticated' event handler delegate.</summary>
    /// <param name="session">The RPC session that was authenticated.</param>
    public delegate void RpcSessionAuthenticatedEventHandler(Session session);
}