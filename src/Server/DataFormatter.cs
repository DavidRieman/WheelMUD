//-----------------------------------------------------------------------------
// <copyright file="DataFormatter.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace WheelMUD.Server
{
    /// <summary>The data formatter. Runs data through our series of data filters for correct presentation on the client.</summary>
    internal static class DataFormatter
    {
        /// <summary>Runs the data through our presentation handlers.</summary>
        /// <param name="data">The data to format</param>
        /// <param name="connection">The connection object this data is going to be sent to</param>
        /// <param name="sendAllData">Indicates if all data should be sent.</param>
        /// <returns>A formatted string of data</returns>
        internal static string FormatData(string data, Connection connection, bool sendAllData)
        {
            if (connection.TerminalOptions.UseWordWrap && connection.TerminalOptions.Width > 0)
            {
                data = WordWrapHandler.Parse(data, connection.TerminalOptions.Width);
            }

            if (connection.TerminalOptions.UseBuffer && !sendAllData)
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
            
            if (!connection.TerminalOptions.UseANSI)
            {
                // TODO: Remove regex into separate parser.
                var options = RegexOptions.None;
                // since we automatically parse with the ansi builder, we search for ansi codes
                var regex = new Regex(@"\u001B\\[[;\\d]*[ -/]*[@-~]", options);
                string input = data;
                string replacement = string.Empty;
                data = regex.Replace(input, replacement);
            }

            if (!connection.TerminalOptions.UseMXP)
            {
                // TODO: Remove regex into separate parser. Currently this would remove all ANSI codes, not just MXP
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