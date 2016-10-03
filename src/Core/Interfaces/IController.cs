//-----------------------------------------------------------------------------
// <copyright file="IController.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Interfaces for Entities.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Interfaces
{
    using WheelMUD.Core;
    using WheelMUD.Core.Events;

    /// <summary>An interface defining a Controller.</summary>
    public interface IController
    {
        /// <summary>An 'action received' event handler.</summary>
        event ActionReceivedEventHandler ActionReceived;

        /// <summary>Gets the living behavior of the Thing attached to this session.</summary>
        LivingBehavior LivingBehavior { get; }

        /// <summary>Gets the base Thing (player or whatnot) attached to this session.</summary>
        Thing Thing { get; }

        /// <summary>Gets the last action input that the session received.</summary>
        ActionInput LastActionInput { get; }

        /// <summary>Write data to the users screen.</summary>
        /// <param name="data">The data to write.</param>
        /// <param name="sendPrompt">true to send the prompt after, false otherwise.</param>
        void Write(string data, bool sendPrompt = true);

        /// <summary>Place an action on the command queue for immediate execution.</summary>
        /// <param name="actionInput">The action input to try to execute as an action.</param>
        void ExecuteAction(ActionInput actionInput);
    }
}