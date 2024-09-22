//-----------------------------------------------------------------------------
// <copyright file="MXPHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using WheelMUD.Server.Interfaces;
using WheelMUD.Server.Telnet;
using WheelMUD.Utilities;

namespace WheelMUD.Server
{
    /// <summary>The MUD Extension Protocol (MXP) handler.</summary>
    /// <remarks>See: https://www.zuggsoft.com/zmud/mxp.htm for protocol details.</remarks>
    internal static class MXPHandler
    {
        /// <summary>Begin an open line. Allows command tagging from the MXP "open" category to follow in the line of output.</summary>
        internal static readonly string OpenLineCode = AnsiSequences.Esc + "[0z";

        /// <summary>
        /// Begin a secure line. Allows advanced command tagging to follow in the line of output.
        /// WARNING: Only use with output that is fully controlled by the server, devoid of user-generated strings.
        /// </summary>
        internal static readonly string SecureLineCode = AnsiSequences.Esc + "[1z";

        /// <summary>Begin a locked line. Disables all command tag processing for this line of output. Can be used to enforce rendering the output verbatim.</summary>
        internal static readonly string LockedLineCode = AnsiSequences.Esc + "[2z";

        /// <summary>Parses incoming data for the MXP version tag if we are expecting it and returns an array of bytes.</summary>
        /// <param name="sender">The connection requesting the data be parsed</param>
        /// <param name="data">The data to parse.</param>
        /// <returns>an array of bytes with the version tag stripped</returns>
        internal static byte[] ParseIncomingData(IConnection sender, byte[] data)
        {
            if (data.Length == 0) return data; // Already done.

            // Not a nice implementation of a state machine, but it does the job for this tiny piece of functionality.

            // Get our mxp telnet option.
            var mxpOption = sender.TelnetCodeHandler.FindOption<TelnetOptionMXP>();

            if (mxpOption != null && mxpOption.AwaitingVersionResponse)
            {
                // Version tag looks like this:
                // <ESC>[1z<VERSION MXP=mxpversion STYLE=styleversion CLIENT=clientname VERSION=clientversion REGISTERED=yes/no>\r\n
                // <ESC> is decimal 27
                var buffer = new List<byte>();
                foreach (byte b in data)
                {
                    switch (mxpOption.VersionResponseState)
                    {
                        case TelnetOptionMXP.ResponseState.Text:
                            // Esc
                            if (b == 27)
                            {
                                mxpOption.VersionResponseState = TelnetOptionMXP.ResponseState.Esc;
                            }
                            else
                            {
                                buffer.Add(b);
                            }

                            break;
                        case TelnetOptionMXP.ResponseState.Esc:
                            // [
                            if (b == 91)
                            {
                                mxpOption.VersionResponseState = TelnetOptionMXP.ResponseState.OpenBracket;
                            }
                            else
                            {
                                buffer.Add(b);
                                mxpOption.VersionResponseState = TelnetOptionMXP.ResponseState.Text;
                            }

                            break;
                        case TelnetOptionMXP.ResponseState.OpenBracket:
                            // 1
                            if (b == 49)
                            {
                                mxpOption.VersionResponseState = TelnetOptionMXP.ResponseState.One;
                            }
                            else
                            {
                                buffer.Add(b);
                                mxpOption.VersionResponseState = TelnetOptionMXP.ResponseState.Text;
                            }

                            break;
                        case TelnetOptionMXP.ResponseState.One:
                            // z
                            if (b == 122)
                            {
                                mxpOption.VersionResponseState = TelnetOptionMXP.ResponseState.VersionTag;
                            }
                            else
                            {
                                buffer.Add(b);
                                mxpOption.VersionResponseState = TelnetOptionMXP.ResponseState.Text;
                            }

                            break;
                        case TelnetOptionMXP.ResponseState.VersionTag:
                            {
                                // lf
                                if (b == 10)
                                {
                                    ProcessBuffer(sender, mxpOption);
                                    mxpOption.VersionResponseState = TelnetOptionMXP.ResponseState.Text;
                                }
                                else
                                {
                                    mxpOption.VersionResponseBuffer += Encoding.ASCII.GetString(new[] { b });
                                }

                                break;
                            }
                    }
                }

                return buffer.ToArray();
            }

            return data;
        }

        /// <summary>Processes the version response and fills their terminal class with info.</summary>
        /// <param name="sender">The connection this relates to.</param>
        /// <param name="mxpOption">The option we are processing.</param>
        private static void ProcessBuffer(IConnection sender, TelnetOptionMXP mxpOption)
        {
            string[] words = mxpOption.VersionResponseBuffer.Split(new[] { ' ', '>' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string word in words)
            {
                if (word.StartsWith("version", StringComparison.OrdinalIgnoreCase))
                {
                    string[] keyValueParts = word.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    sender.TerminalOptions.Version = keyValueParts[1].Trim('"');
                }
                else if (word.StartsWith("client", StringComparison.OrdinalIgnoreCase))
                {
                    string[] keyValueParts = word.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    sender.TerminalOptions.Client = keyValueParts[1].Trim('"').ToLower();
                }
            }

            mxpOption.VersionResponseBuffer = string.Empty;

            // TODO: This is a hack. zmud 6.16 doesn't support MCCP correctly but responds that it does.
            // We should disable the option here.
            if (sender.TerminalOptions.Client == "zmud" && sender.TerminalOptions.Version == "6.16")
            {
                sender.TelnetCodeHandler.FindOption<TelnetOptionMCCP>()?.Disable();
            }
        }
    }
}