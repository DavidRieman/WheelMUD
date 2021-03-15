//-----------------------------------------------------------------------------
// <copyright file="OutputParser.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Utilities;

namespace WheelMUD.Server
{
    public static class OutputParser
    {
        //TODO: Optimize more
        public static string Parse(char[] buffer,int bufferPos, TerminalOptions terminalOptions)
        {
            var result = "";
            var token = "";
            var charFromNewline = 0;
            var isError = false;
            var isWarning = false;

            var isToken = false;

            for (var i = 0; i < bufferPos; i++)
            {
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

                        if (token == "error")
                        {
                            token = "";
                            isError = true;
                            continue;
                        }

                        if (token == "warning")
                        {
                            token = "";
                            isWarning = true;
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

            if (isError)
            {
                if (!terminalOptions.UseANSI)
                    return $"ERROR MESSAGE:{AnsiSequences.NewLine}{result}{AnsiSequences.NewLine}";
                
                var firstline = $"{AnsiSequences.ForegroundRed}{new string('X', terminalOptions.Width)}";
                var lastline = $"{new string('=', terminalOptions.Width)}{AnsiSequences.TextNormal}";
                return $"{AnsiSequences.NewLine}{firstline}{AnsiSequences.NewLine}{result}{AnsiSequences.NewLine}{lastline}{AnsiSequences.NewLine}";

            }
            
            if (isWarning)
            {
                if (!terminalOptions.UseANSI)
                    return $"WARNING MESSAGE:{AnsiSequences.NewLine}{result}{AnsiSequences.NewLine}";
                
                var firstline = $"{AnsiSequences.ForegroundYellow}{new string('x', terminalOptions.Width)}";
                var lastline = $"{new string('=', terminalOptions.Width)}{AnsiSequences.TextNormal}";
                return $"{AnsiSequences.NewLine}{firstline}{AnsiSequences.NewLine}{result}{AnsiSequences.NewLine}{lastline}{AnsiSequences.NewLine}";

            }
            
            return result;
        }
    }
}