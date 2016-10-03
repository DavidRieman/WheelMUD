//-----------------------------------------------------------------------------
// <copyright file="MXPHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Handles MUD Extension Protocol (MXP) data.
//   Created: January 2007 by Foxedup
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using WheelMUD.Interfaces;
    using WheelMUD.Server.Telnet;

    /// <summary>The MUD Extension Protocol (MXP) handler.</summary>
    internal static class MXPHandler
    {
        /// <summary>Parses incoming data for the MXP version tag if we are expecting it and returns an array of bytes.</summary>
        /// <param name="sender">The connection requesting the data be parsed</param>
        /// <param name="data">The data to parse.</param>
        /// <returns>an array of bytes with the version tag stripped</returns>
        internal static byte[] ParseIncomingData(IConnection sender, byte[] data)
        {
            // Not a nice implementation of a state machine, but it does the job for this tiny piece of functionality.

            // Get our mxp telnet option.
            var mxpOption = sender.TelnetCodeHandler.TelnetOptions.Find(o => o.Name.Equals("mxp")) as TelnetOptionMXP;

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
            string[] words = mxpOption.VersionResponseBuffer.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string word in words)
            {
                if (word.Trim().StartsWith("version", StringComparison.InvariantCultureIgnoreCase))
                {
                    string[] parts = word.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    sender.Terminal.Version = parts[1].Trim().TrimEnd(new[] { '>' }).Trim(new[] { '"' });
                }
                else if (word.Trim().StartsWith("client", StringComparison.InvariantCultureIgnoreCase))
                {
                    string[] parts = word.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    sender.Terminal.Client = parts[1].Trim().TrimEnd(new[] { '>' }).Trim(new[] { '"' }).ToLower();
                }
            }

            mxpOption.VersionResponseBuffer = string.Empty;

            // @@@ This is a hack. zmud 6.16 doesn't support MCCP correctly but responds that it does.
            // We should disable the option here.
            if (sender.Terminal.Client == "zmud" && sender.Terminal.Version == "6.16")
            {
                ITelnetOption mccpOption =
                    sender.TelnetCodeHandler.TelnetOptions.Find(
                        delegate(ITelnetOption o) { return o.Name.Equals("compress2"); });
                if (mccpOption != null)
                {
                    mccpOption.Disable();
                }
            }
        }
    }
}