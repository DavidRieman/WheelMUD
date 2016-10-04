//-----------------------------------------------------------------------------
// <copyright file="IRecomposable.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Interfaces
{
    /// <summary>Describes a component whose subcomponents are recomposable.</summary>
    public interface IRecomposable
    {
        /// <summary>Recompose all subcomponents which are recomposable.</summary>
        void Recompose();
    }
}