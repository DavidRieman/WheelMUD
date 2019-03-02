//-----------------------------------------------------------------------------
// <copyright file="ISystemBase.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   TODO: Add summary
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Interfaces
{
    /// <summary>This is the most basic system in the MUD.</summary>
    public interface ISystemBase
    {
        /// <summary>Starts this system.</summary>
        void Start();

        /// <summary>Stops this system.</summary>
        void Stop();
    }
}