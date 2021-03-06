//-----------------------------------------------------------------------------
// <copyright file="StringHelper.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace WheelMUD.Utilities
{
    public static class StringHelper
    {
        /// <summary>
        /// Returns a string with ',' or 'and' depending on item count.
        /// </summary>
        /// <remarks>Output example: "apples, oranges and bananas".</remarks>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string BuildPrettyList(this IEnumerable<string>  items)
        {
            return string.Join(", ", items.Take(items.Count() - 1)) +
                   (items.Count() > 1 ? " and " : "") + items.LastOrDefault();
        }
    }
}