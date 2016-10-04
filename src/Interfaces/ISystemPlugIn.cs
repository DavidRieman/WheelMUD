//-----------------------------------------------------------------------------
// <copyright file="ISystemPlugIn.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Core system interfaces.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Interfaces
{
    /// <summary>Interface that will be used to load MUD systems that are non-core. These will be loaded using MEF.</summary>
    public interface ISystemPlugIn : ISystem
    {
    }
}