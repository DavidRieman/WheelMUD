//-----------------------------------------------------------------------------
// <copyright file="IServiceLocator.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//    A generic interface for classes that will be used to locate external 
//    resources like 3rd party rules, combat systems, and the like.  
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Interfaces
{
    /// <summary>Classes that will be used to locate external resources like 3rd party rules, combat systems, and the like.</summary>
    public interface IServiceLocator
    {
        /// <summary>Gets the service.</summary>
        /// <typeparam name="T">The signature (class) that we are looking for.</typeparam>
        /// <returns>Returns an instance of the class in question, if it exists.</returns>
        T GetService<T>();
    }
}