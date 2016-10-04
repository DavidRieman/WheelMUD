//-----------------------------------------------------------------------------
// <copyright file="MudIdentity.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A MUD Identity for Entity Authorization.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Universe
{
    using System.Security.Principal;

    /// <summary>This gives <see cref="WheelMUD.Universe.Entity"/> child classes a .NET-style identity.</summary>
    /// <remarks>Added by Hector Sosa, Jr (aka Fastalanasa) on Dec 27, 2006.</remarks>
    public class MudIdentity : GenericIdentity
    {
        /// <summary>Initializes a new instance of the <see cref="MudIdentity"/> class.</summary>
        /// <param name="name">The name of this entity. An ID can be used in place of the name, as there could be more than one entity with the same name.</param>
        public MudIdentity(string name)
            : base(name)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="MudIdentity"/> class.</summary>
        /// <param name="name">The name of this entity. An ID can be used in place of the name, as there could be more than one entity with the same name.</param>
        /// <param name="authenticationType">The type of authentication used for this identity.</param>
        public MudIdentity(string name, string authenticationType)
            : base(name, authenticationType)
        {
        }
    }
}