//-----------------------------------------------------------------------------
// <copyright file="ISuperSystemSubscriber.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;

namespace WheelMUD.Server.Interfaces
{
    /// <summary>An interface describing a SuperSystemSubscriber.</summary>
    public interface ISuperSystemSubscriber : IDisposable
    {
        /// <summary>Notify subscribers of the specified message.</summary>
        /// <param name="message">The message to pass along.</param>
        void Notify(string message);
    }
}