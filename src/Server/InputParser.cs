//-----------------------------------------------------------------------------
// <copyright file="InputParser.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   This class handles data coming in over a connection, it checks to see if the data is
//   an action and if so notifies the interested parties for processing.
//   Created: August 2006 by Foxedup
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server
{
    using System;
    using System.Text;
    using WheelMUD.Core;
    using WheelMUD.Interfaces;

    /// <summary>The input parser.</summary>
    public class InputParser
    {
        /// <summary>The character sequence used to designate a new line.</summary>
        private const string newLineMarker = "\r";

        /// <summary>The 'input received' event handler delegate.</summary>
        /// <param name="sender">The sender of the input.</param>
        /// <param name="args">The connection arguments.</param>
        /// <param name="input">The input that was received.</param>
        public delegate void InputReceivedEventHandler(object sender, ConnectionArgs args, string input);

        /// <summary>The 'input received' event handler.</summary>
        public event InputReceivedEventHandler InputReceived;

        /// <summary>Checks the data passed to it to see if the connection now has a command ready.</summary>
        /// <param name="sender">The connection sending the data</param>
        /// <param name="data">The data received</param>
        public void OnDataReceived(IConnection sender, byte[] data)
        {
            // We can be sure that the data is text because the telnet server has dealt with any nasties.
            string input = Encoding.ASCII.GetString(data);

            // Append the data to our text buffer.
            sender.Buffer.Append(input);

            // Get the whole of our buffer.
            input = sender.Buffer.ToString();

            input = StripDodgyTerminator(sender, input);

            // If we have nothing left to process then bail out.
            if (input.Length == 0)
            {
                return;
            }

            SetLastTerminator(sender, input);

            // Replace any \r\n or \n\r or \n with just \r so that we can just deal with one line marker.
            input = input.Replace("\n", newLineMarker);
            input = input.Replace("\r\r", newLineMarker);

            // Our new line will only be a \r as we removed all \n above.
            if (input.Contains(newLineMarker))
            {
                // We have received a newline char in our stream, this means that
                // we need to raise a command. However we might receive multiple commands
                // in the same stream as our network connection might be slow.
                // 
                // We have a special case which is not captured by below.
                // That of just a carriage return being entered in the client.
                // We should pass that up as a valid command.
                if (input == newLineMarker)
                {
                    this.RaiseInputReceived(new ConnectionArgs(sender), string.Empty);
                }

                // Does our input "end" with \r if so then we have a series of full commands.
                // If it doesn't then we have a partial command at the end which needs to wait for more data.
                bool fullCommand = input.EndsWith(newLineMarker);

                // Split on our \r
                string[] commands = input.Split(new string[] { newLineMarker }, StringSplitOptions.RemoveEmptyEntries);
                string currentInput = string.Empty;

                for (int i = 0; i < commands.Length; i++)
                {
                    currentInput = commands[i];

                    // If its a full command.
                    if ((i < commands.Length - 1) || (i == commands.Length - 1 && fullCommand))
                    {
                        // Fill the last input buffer.
                        sender.LastRawInput = currentInput;
                        
                        // Raise the input received event.
                        this.RaiseInputReceived(new ConnectionArgs(sender), currentInput.Trim());
                    }
                }

                // Clear the buffer.
                sender.Buffer.Length = 0;
                if (!fullCommand)
                {
                    // If the last command wasn't a full command,
                    // we need to append that action back to the buffer.
                    sender.Buffer.Append(currentInput);
                }
            }
        }

        /// <summary>
        /// Because of the variation in line terminators between clients we have to choose one
        /// and strip out the others. This leaves a funny situation when we have determined that
        /// a command has come in because of \r so we process it but the next string we receive
        /// begins with \n because the client actually send command\r\n.
        /// This function checks the last terminator we received and if the opposite one is
        /// received at the start of the next set of data it removes it.
        /// </summary>
        /// <param name="sender">The connection we received the input on</param>
        /// <param name="input">The input we are checking</param>
        /// <returns>Returns the input, stripped of dodgy terminators.</returns>
        private static string StripDodgyTerminator(IConnection sender, string input)
        {
            if ((sender.LastInputTerminator == "\r") && input.StartsWith("\n"))
            {
                input = input.Replace("\n", string.Empty);
            }
            else if ((sender.LastInputTerminator == "\n") && input.StartsWith("\r"))
            {
                input = input.Replace("\r", string.Empty);
            }

            return input;
        }

        /// <summary>Sets the last input terminator to the termination char(s) we have just received (if any).</summary>
        /// <param name="sender">The connection we received input on</param>
        /// <param name="input">The input we are checking</param>
        private static void SetLastTerminator(IConnection sender, string input)
        {
            if (input.Contains("\r") & input.Contains("\n"))
            {
                sender.LastInputTerminator = "\r\n";
            }
            else if (input.Contains("\r"))
            {
                sender.LastInputTerminator = "\r";
            }
            else if (input.Contains("\n"))
            {
                sender.LastInputTerminator = "\n";
            }
        }

        /// <summary>Raises our input receive event safely.</summary>
        /// <param name="connectionArgs">The connection arguments</param>
        /// <param name="action">The text that was received</param>
        private void RaiseInputReceived(ConnectionArgs connectionArgs, string action)
        {
            if (this.InputReceived != null)
            {
                this.InputReceived(this, connectionArgs, action);
            }
        }
    }
}