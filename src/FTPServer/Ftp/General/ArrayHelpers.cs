//-----------------------------------------------------------------------------
// <copyright file="ArrayHelpers.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace WheelMUD.Ftp.General
{
    public sealed class ArrayHelpers
    {
        private ArrayHelpers()
        {
        }

        public static Array Add(Array firstArray, Array secondArray)
        {
            if (firstArray == null)
            {
                return secondArray.Clone() as Array;
            }

            if (secondArray == null)
            {
                return firstArray.Clone() as Array;
            }

            Type typeFirst = firstArray.GetType().GetElementType();
            Type typeSecond = secondArray.GetType().GetElementType();

            Debug.Assert(typeFirst == typeSecond, "The array types must be the same to add them.");

            Array newArray = Array.CreateInstance(typeFirst, firstArray.Length + secondArray.Length);
            firstArray.CopyTo(newArray, 0);
            secondArray.CopyTo(newArray, firstArray.Length);

            return newArray;
        }
    }
}