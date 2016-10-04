//-----------------------------------------------------------------------------
// <copyright file="IThing.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Interfaces
{
    /// <summary>An interface defining a Thing.</summary>
    public interface IThing : IPersistable
    {
        /// <summary>Gets or sets the ID of this thing.</summary>
        string ID { get; set; }

        /// <summary>Gets the name of this thing.</summary>
        string Name { get; }

        /// <summary>Gets the full name of this thing.</summary>
        string FullName { get; }

        /// <summary>Gets the description of this thing.</summary>
        string Description { get; }

        /// <summary>Gets or sets the title of this thing.</summary>
        string Title { get; set; }
    }
}