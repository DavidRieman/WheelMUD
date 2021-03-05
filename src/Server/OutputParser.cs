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
        public static string NonAnsi(char[] buffer, int wordWrapLength)
        {
            var baseResult = "";
            var token = "";
            var charFromNewline = 0;

            var isToken = false;

            for (var i = 0; i < buffer.Length; i++)
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
                        
                        baseResult = baseResult.Remove(baseResult.Length - 1);
                    }
                }

                if (!isToken)
                {
                    baseResult += buffer[i];
                    charFromNewline++;

                    if (charFromNewline <= wordWrapLength) continue;
                    
                    var lastSpaceIndex = baseResult.LastIndexOf(' ');
                        
                    if (lastSpaceIndex > 0)
                    {
                        baseResult = baseResult.Remove(lastSpaceIndex, 1);
                        charFromNewline = baseResult.Length - lastSpaceIndex;
                        baseResult = baseResult.Insert(lastSpaceIndex, AnsiSequences.NewLine);
                    }
                    else
                    {
                        baseResult += AnsiSequences.NewLine;
                        charFromNewline = 0;
                    }
                }
                else token += buffer[i];
            }

            return baseResult;
        }
        
        //TODO: Optimize more
        public static string Ansi(char[] buffer, int wordWrapLength)
        {
            var ansiResult = "";
            var token = "";
            var charFromNewline = 0;

            var isToken = false;

            for (var i = 0; i < buffer.Length; i++)
            {
                if (isToken && buffer[i] == '>')
                {
                    if (buffer[i - 1] == '%')
                    {
                        isToken = false;
                        token = token.Trim('%');

                        if (token == "nl") charFromNewline = 0;
                        
                        AnsiSequences.TokenMap.TryGetValue(token, out var ansiCode);
                        ansiResult += ansiCode;
                        
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
                        
                        ansiResult = ansiResult.Remove(ansiResult.Length - 1);
                    }
                }

                if (!isToken)
                {
                    ansiResult += buffer[i];
                    charFromNewline++;

                    if (charFromNewline <= wordWrapLength) continue;
                    
                    var lastSpaceIndex = ansiResult.LastIndexOf(' ');
                        
                    if (lastSpaceIndex > 0)
                    {
                        ansiResult = ansiResult.Remove(lastSpaceIndex, 1);
                        charFromNewline = ansiResult.Length - lastSpaceIndex;
                        ansiResult = ansiResult.Insert(lastSpaceIndex, AnsiSequences.NewLine);
                    }
                    else
                    {
                        ansiResult += AnsiSequences.NewLine;
                        charFromNewline = 0;
                    }
                }
                else token += buffer[i];
            }

            return ansiResult;
        }
    }
}