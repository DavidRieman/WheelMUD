//-----------------------------------------------------------------------------
// <copyright file="Genders.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    [ExportGameGender]
    public class GenderMale : GameGender
    {
        public GenderMale()
            : base("Male", "M")
        {
        }
    }

    [ExportGameGender]
    public class GenderFemale : GameGender
    {
        public GenderFemale()
            : base("Female", "F")
        {
        }
    }
}