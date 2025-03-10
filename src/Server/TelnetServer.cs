//-----------------------------------------------------------------------------
// <copyright file="TelnetServer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Server.Interfaces;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Server
{
    /// <summary>A telnet server.</summary>
    /// <remarks>This class decorates the base server, providing telnet facilities to the application.</remarks>
    public class TelnetServer : ISubSystem
    {
        /// <summary>Handles received data: Processes telnet escape codes and non-alpha characters.</summary>
        /// <param name="sender">The connection which sent the data.</param>
        /// <param name="data">The data that was sent.</param>
        /// <returns>True if data went to this sender's buffer which suggests we may be ready to consume that buffer.</returns>
        public static bool OnDataReceived(IConnection sender, byte[] data)
        {
            byte[] returnData = sender.TelnetCodeHandler.ProcessInput(data);
            returnData = MXPHandler.ParseIncomingData(sender, returnData);
            return BufferUserInput(sender, returnData);
        }

        /// <summary>Handles incoming user input data.</summary>
        /// <remarks>Moves user input to their input buffer, and handles edge cases like backspaces.</remarks>
        /// <param name="sender">The connection which sent the data.</param>
        /// <param name="data">The non-Telnet-command data that we received from the client.</param>
        /// <returns>
        /// True if data went to this sender's buffer which suggests we may have a user command to consume.
        /// (If server is in a mode that tries to interpret single keystrokes, it could disregard this hint.)
        /// </returns>
        private static bool BufferUserInput(IConnection sender, byte[] data)
        {
            if (data.Length == 0) return false; // Already done.

            bool readyForProcessing = false;

            // We will be iterating through all our pending data and figuring out which bits are valid
            // to keep for input buffer, and which bits are valid to echo back to the client (if any).
            List<byte> echoBuffer = sender.TerminalOptions.UseEcho ? [] : null;
            foreach (byte bit in data)
            {
                if (bit > 31 && bit < 127) // Normal printable characters.
                {
                    // For normal printable characters: Add to our buffer, and ECHO them if asked to.
                    sender.Buffer.Append((char)bit);

                    // However, the character we echo might be an obscuring character instead, if our connection is
                    // currently being told to do so (E.G. during password entry or other sensitive inputs).
                    echoBuffer?.Add(sender.TerminalOptions.UseEchoObscuring ? (byte)'*' : bit);
                }
                else if (bit == 10 || bit == 13) // Line Feed (LF=10) or Carriage Return (CR=13)
                {
                    // @@@ HMM maybe "if last character was (the other one) simply do not add it" ???
                    
                    // These go to our command processor, but... TODO: should we echo them?  (We want decent consistency
                    // of vertical spacing whether the client is printing whole lines or we are echoing char by char.)
                    sender.Buffer.Append((char)bit);

                    // The user has finished typing a line (which usually means a command is ready).
                    readyForProcessing = true;

                    // Regardless of whether the client sent use just CR or LF, in char-by-char mode we want to reply
                    // with both CR+LF to move their cursor to the beginning of the new line.
                    echoBuffer?.Add(13);
                    echoBuffer?.Add(10);
                }
                else if (bit == 8 || bit == 127)
                {
                    // Special case for a backspace (8) or delete (127): Remove the last buffered character (if any).
                    // If there was a buffer character, and we're in echo mode, we also need to echo that erasure.
                    if (sender.Buffer.Length > 0)
                    {
                        sender.Buffer.Remove(sender.Buffer.Length - 1, 1);
                        if (echoBuffer != null)
                        {
                            // In the unlikely event the client managed to deliver multiple characters including a
                            // backspace, we may non-optimally echo a character and immediate deletion of it: We
                            // could try to optimize that behavior out but it's probably not worth the bug risk on
                            // such a difficult to reproduce situation.
                            echoBuffer?.Add(8); // Backspace, space, backspace. (TODO: There's gotta be a better way?)
                            echoBuffer?.Add((byte)' '); 
                            echoBuffer?.Add(8);
                        }
                    }
                }
            }

            if (echoBuffer != null && echoBuffer.Count > 0)
            {
                sender.Send([.. echoBuffer]);
            }

            return readyForProcessing;
        }
    }
}