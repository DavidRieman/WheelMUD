using System.Text;

namespace WheelMUD.Utilities
{
    public static class AnsiStringUtilities
    {
        public static string ExpandString(string str, int length)
        {
            // BTW, you already know how big the result should be, so just
            // tell the StringBuilder its capacity to prevent unneeded resizes.
            // Side benefit: if the result is too long, you'll find out fairly quickly.
            var result = new StringBuilder(length, length);

            var wholeCopies = length / str.Length;
            for (var i = 0; i < wholeCopies; ++i)
            {
                result.Append(str);
            }

            // now append the last chunk, a possibly-zero-length prefix of `str`
            result.Append(str, 0, length % str.Length);

            return result.ToString();
        }

        /// <summary>Formats the string to fit the column width. It will be padded with spaces on the right side.</summary>
        /// <param name="columnWidth">Width of the column.</param>
        /// <param name="stringToFormat">The string to format.</param>
        /// <returns>Formatted row to match the column width.</returns>
        public static string FormatToColumn(int columnWidth, string stringToFormat)
        {
            string retval = string.Empty;

            if (stringToFormat.Length < columnWidth)
            {
                retval = stringToFormat.PadRight(columnWidth, ' ');
            }
            else
            {
                retval = stringToFormat;
            }

            return retval;
        }
    }
}