//-----------------------------------------------------------------------------
// <copyright file="ArrayHelpers.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>//   
//   Created: 2004 By David McClarnon (dmcclarnon@ntlworld.com)
//   Modified: June 16, 2010 by Fastalanasa.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.General
{
    using System;

    public sealed class ArrayHelpers
    {
        public static Array Add(Array aFirst, Array aSecond)
        {
            if (aFirst == null)
            {
                return aSecond.Clone() as Array;
            }

            if (aSecond == null)
            {
                return aFirst.Clone() as Array;
            }

            Type typeFirst = aFirst.GetType().GetElementType();
            Type typeSecond = aSecond.GetType().GetElementType();

            System.Diagnostics.Debug.Assert(typeFirst == typeSecond);

            Array aNewArray = Array.CreateInstance(typeFirst, aFirst.Length + aSecond.Length);
            aFirst.CopyTo(aNewArray, 0);
            aSecond.CopyTo(aNewArray, aFirst.Length);

            return aNewArray;
        }

        private ArrayHelpers()
        {

        }
    }
}
