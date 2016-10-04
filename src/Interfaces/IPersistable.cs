//-----------------------------------------------------------------------------
// <copyright file="IPersistable.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Interfaces
{
    /// <summary>An interface that defines a persistable object.</summary>
    public interface IPersistable
    {
        /// <summary>Saves the thing.</summary>
        void Save();
    }
}