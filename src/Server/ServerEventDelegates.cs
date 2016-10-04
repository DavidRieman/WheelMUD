//-----------------------------------------------------------------------------
// <copyright file="ServerEventDelegates.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server
{
    /// <summary>The 'input received' event handler delegate.</summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="input">The input that was received.</param>
    public delegate void InputReceivedEventHandler(object sender, string input);
}