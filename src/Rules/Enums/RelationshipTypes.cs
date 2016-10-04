//-----------------------------------------------------------------------------
// <copyright file="RelationshipTypes.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 2/19/2011
//   Purpose   : A central repository for game rule enums.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Rules
{
    using System;

    /// <summary>List of valid relationship types between a parent object and another object through a managed property.</summary>
    [Flags]
    public enum RelationshipTypes
    {
        /// <summary>The default value, indicating all values are cleared.</summary>
        None = 0x0,

        /// <summary>Property is a reference to a child object contained by the parent.</summary>
        Child = 0x1,

        /// <summary>Property is a reference to a lazy loaded object.</summary>
        /// <remarks>Attempting to get or read the property value prior to a set or load will result in an exception.</remarks>
        LazyLoad = 0x2,

        /// <summary>Property is stored in a private field.</summary>
        /// <remarks>
        /// Attempting to read or write the property in FieldManager (managed fields) will throw an exception. 
        /// NonGeneric ReadProperty/LoadProperty will call property get/set methods. 
        /// </remarks>
        PrivateField = 0x4,
    }
}