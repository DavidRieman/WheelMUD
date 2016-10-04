//-----------------------------------------------------------------------------
// <copyright file="MobRecord.NoGen.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 7/7/2009 9:43:44 PM
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Entities
{
    /// <summary>Initializes a new instance of the MobRecord class.</summary>
    public partial class MobRecord
    {
        /// <summary>The types of mobiles.</summary>
        public enum MobTypes
        {
            /// <summary>A basic guard mob.</summary>
            BasicGuard = 1,

            /// <summary>A shop keep mob.</summary>
            ShopKeeper = 2
        }
    }
}