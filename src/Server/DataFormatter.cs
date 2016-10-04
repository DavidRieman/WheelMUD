//-----------------------------------------------------------------------------
// <copyright file="DataFormatter.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Runs data through our series of data filters for correct presentation on the client.
//   Created: January 2007 by Foxedup
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server
{
    using System.Text.RegularExpressions;
    using WheelMUD.Core.Enums;
    using WheelMUD.Core.Output;

    /// <summary>The data formatter.</summary>
    internal static class DataFormatter
    {
        /// <summary>Runs the data through our presentation handlers.</summary>
        /// <param name="data">The data to format</param>
        /// <param name="connection">The connection object this data is going to be sent to</param>
        /// <param name="sendAllData">Indicates if all data should be sent.</param>
        /// <returns>A formatted string of data</returns>
        internal static string FormatData(string data, Connection connection, bool sendAllData)
        {
            if (connection.Terminal.UseWordWrap && connection.Terminal.Width > 0)
            {
                data = WordWrapHandler.Parse(data, connection.Terminal.Width);
            }

            if (connection.Terminal.UseBuffer && !sendAllData)
            {
                connection.OutputBuffer.Clear();
                string[] temp = BufferHandler.Parse(data);
                string[] output;
                bool appendOverflow = false;

                if (temp.Length > connection.PagingRowLimit)
                {
                    appendOverflow = true;
                    connection.OutputBuffer.Append(temp);
                    output = connection.OutputBuffer.GetRows(BufferDirection.Forward, connection.PagingRowLimit);
                }
                else
                {
                    output = temp;
                }

                data = BufferHandler.Format(
                    output,
                    false,
                    appendOverflow,
                    connection.OutputBuffer.CurrentLocation,
                    connection.OutputBuffer.Length);
            }

            if (connection.Terminal.UseANSI)
            {
                data = AnsiHandler.Parse(data);
            }
            else
            {
                // TODO: Remove regex into separate parser.
                var options = RegexOptions.None;
                var regex = new Regex(@"<%?\w+[^>]*%>", options);
                string input = data;
                string replacement = string.Empty;
                data = regex.Replace(input, replacement);
            }

            if (!connection.Terminal.UseMXP)
            {
                // TODO: Remove regex into separate parser.
                var options = RegexOptions.None;
                var regex = new Regex(@"</?\w+[^>]*>", options);
                string input = data;
                string replacement = string.Empty;
                data = regex.Replace(input, replacement);
            }

            return data;
        }
    }
}