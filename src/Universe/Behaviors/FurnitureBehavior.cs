//-----------------------------------------------------------------------------
// <copyright file="FurnitureBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: November 2009 by bengecko.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Universe
{
    using System.Collections.Generic;
    using WheelMUD.Core;

    /// <summary>FlammableBehavior adds the ability to an object to be treated as furniture.</summary>
    /// <remarks>
    /// @@@ TODO: Revisit this behavior... perhaps "furniture" is not a great scope for behavior here; perhaps we could have
    /// distinct behaviors for things like being able to sit and/or sleep on a thing, add an ObscuresExitBehavior, etc...
    /// </remarks>
    public class FurnitureBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the FurnitureBehavior class.</summary>
        public FurnitureBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the FurnitureBehavior class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public FurnitureBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /// <summary>Gets or sets a value indicating whether the object may be moved.</summary>
        public bool IsMoveable { get; set; }

        /// <summary>Gets or sets a value indicating whether the piece of furiture is able to hide an exit.</summary>
        public bool WillObscureExit { get; set; }

        /// <summary>Gets or sets a value indicating whether the piece of furiture is currently obscuring an exit.</summary>
        public bool IsObscuringExit { get; set; }

        /// <summary>Gets or sets a value indicating the exit that is obscured.</summary>
        public string ObscuredExit { get; set; }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            this.IsMoveable = true;
            this.IsObscuringExit = false;
            this.ObscuredExit = null;
            this.WillObscureExit = false;
        }
    }
}