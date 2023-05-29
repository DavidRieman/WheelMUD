//-----------------------------------------------------------------------------
// <copyright file="User.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;

namespace WheelMUD.Data
{
    public class UserAccountHistory
    {
        public string LastIPAddress { get; set; }

        public DateTime LastLogIn { get; set; }

        public DateTime LastLogOut { get; set; }

        public DateTime Created { get; set; }
    }
}
