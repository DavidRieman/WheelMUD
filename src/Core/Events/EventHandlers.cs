//-----------------------------------------------------------------------------
// <copyright file="EventHandlers.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    using WheelMUD.Interfaces;

    /// <summary>A handler for GameEvents.</summary>
    /// <param name="root">The root Thing where this event broadcast started.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void GameEventHandler(Thing root, GameEvent e);

    /// <summary>A handler for CancellableGameEvents.</summary>
    /// <param name="root">The root Thing where this event broadcast started.</param>
    /// <param name="e">The cancellable event/request arguments.</param>
    public delegate void CancellableGameEventHandler(Thing root, CancellableGameEvent e);

    /// <summary>An 'action received' event handler.</summary>
    /// <param name="sender">The sender of the action.</param>
    /// <param name="actionInput">The the action input received.</param>
    public delegate void ActionReceivedEventHandler(IController sender, ActionInput actionInput);

    /// <summary>The 'rpc session authenticated' event handler delegate.</summary>
    /// <param name="session">The RPC session that was authenticated.</param>
    public delegate void RpcSessionAuthenticatedEventHandler(Session session);
}