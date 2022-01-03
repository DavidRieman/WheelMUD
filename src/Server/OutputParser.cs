//-----------------------------------------------------------------------------
// <copyright file="OutputParser.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Diagnostics;
using WheelMUD.Utilities;

namespace WheelMUD.Server
{
    public static class OutputParser
    {
        // TODO: Optimize more
        public static string Parse(char[] buffer, int bufferPos, TerminalOptions terminalOptions)
        {
            var result = "";
            var token = "";
            var charFromNewline = 0;
            var isToken = false;

            for (var i = 0; i < bufferPos; i++)
            {
                // OutputBuffer / OutputParser should be the only place writing ANSI NewLine character sequences, to help guarantee correct Telnet protocol handling.
                // (Telnet protocol says new lines sent should be CR LF regardless of what OS the server is running on and what OS the client is running on.)
                Debug.Assert(buffer[i] != '\r' && buffer[i] != '\n', "Output should not include explicit newline characters. Use <%nl%> or OutputBuffer WriteLine functions instead.");
                if (isToken && buffer[i] == '>')
                {
                    if (buffer[i - 1] == '%')
                    {
                        isToken = false;
                        token = token.Trim('%');

                        if (token == "nl")
                        {
                            charFromNewline = 0;
                            token = "";
                            result += AnsiSequences.NewLine;
                            continue;
                        }

                        if (terminalOptions.UseANSI)
                        {
                            result += AnsiHandler.ConvertCode(token);
                        }

                        token = "";
                        continue;
                    }
                }

                if (!isToken && buffer[i] == '<')
                {
                    if (buffer[i + 1] == '%')
                    {
                        if (i == 0 || buffer[i - 1] != '\\')
                        {
                            isToken = true;
                            continue;
                        }

                        result = result.Remove(result.Length - 1);
                    }
                }

                if (!isToken)
                {
                    result += buffer[i];
                    charFromNewline++;

                    if (charFromNewline <= terminalOptions.Width || terminalOptions.UseWordWrap == false) continue;

                    var lastSpaceIndex = result.LastIndexOf(' ');

                    if (lastSpaceIndex > 0)
                    {
                        result = result.Remove(lastSpaceIndex, 1);
                        charFromNewline = result.Length - lastSpaceIndex;
                        result = result.Insert(lastSpaceIndex, AnsiSequences.NewLine);
                    }
                    else
                    {
                        result += AnsiSequences.NewLine;
                        charFromNewline = 0;
                    }
                }
                else token += buffer[i];
            }

            return result;
        }
    }
}