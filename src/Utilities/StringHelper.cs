using System.Collections.Generic;
using System.Linq;

namespace WheelMUD.Utilities
{
    public static class StringHelper
    {
        /// <summary>
        /// Builds a string with , or and depending on item count.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string BuildPrettyList(this IEnumerable<string>  items)
        {
            return string.Join(", ", items.Take(items.Count() - 1)) +
                   (items.Count() > 1 ? " and " : "") + items.LastOrDefault();
        }
    }
}