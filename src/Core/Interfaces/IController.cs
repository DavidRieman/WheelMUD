//-----------------------------------------------------------------------------
// <copyright file="IController.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Core;
using WheelMUD.Server;

namespace WheelMUD.Interfaces
{
    /// <summary>An interface defining a Controller.</summary>
    public interface IController
    {
        /// <summary>Gets the base Thing (player or whatnot) attached to this session.</summary>
        Thing Thing { get; }

        /// <summary>Gets the last action input that the session received.</summary>
        ActionInput LastActionInput { get; }

        /// <summary>Write data to the users screen.</summary>
        /// <param name="data">The data to write.</param>
        /// <param name="sendPrompt">true to send the prompt after, false otherwise.</param>
        void Write(OutputBuilder data, bool sendPrompt = true);
    }
}