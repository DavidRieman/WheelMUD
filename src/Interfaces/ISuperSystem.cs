//-----------------------------------------------------------------------------
// <copyright file="ISuperSystem.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Core system interfaces.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Interfaces
{
    /// <summary>An interface describing a SuperSystem.</summary>
    public interface ISuperSystem : ISystemBase, ISystemHost
    {
        /// <summary>Subscribe to the specified super system subscriber.</summary>
        /// <param name="sender">The subscribing system; generally use 'this'.</param>
        void SubscribeToSystem(ISuperSystemSubscriber sender);

        /// <summary>Unsubscribe from the specified super system subscriber.</summary>
        /// <param name="sender">The unsubscribing system; generally use 'this'.</param>
        void UnSubscribeFromSystem(ISuperSystemSubscriber sender);
    }
}