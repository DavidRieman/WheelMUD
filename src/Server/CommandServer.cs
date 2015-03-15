/*
 * Created by: Foxedup
 * Created: August 2006
 * Purpose: This class handles data coming in over a connection, it checks to see if the data is
 *          an action and if so notifies the interested parties for processing.
*/

using System;
using System.Text;
using WheelMUD.Core;
using WheelMUD.Core.Interfaces;

namespace WheelMUD.Server
{
    public class CommandServer : ISubSystem
    {
        #region Declerations

        public delegate void inputReceivedEventHandler(Object sender, ConnectionArgs args, String input);

        public event inputReceivedEventHandler InputReceived;
        private static string _newLineMarker = "\r";

        #endregion

        #region ISubSystem Implementation

        private ISubSystemHost _subSystemHost;

        public String Name
        {
            get { return "Command Server"; }
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void SubscribeToSystem(ISubSystemHost sender)
        {
            _subSystemHost = sender;
        }

        public void InformSubscribedSystem(SystemUpdateArgs args)
        {
            _subSystemHost.UpdateSubSystemHost(this, args);
        }

        #endregion

        /// <summary>
        /// Checks the data passed to it to see if the connection now has
        /// a command ready.
        /// </summary>
        /// <param name="sender">The connection sending the data</param>
        /// <param name="data">The data received</param>
        public void OnDataReceived(Connection sender, Byte[] data)
        {
            //we can be sure that the data is text because the telnet server has dealt with any nasties
            string input = Encoding.ASCII.GetString(data);

            //Append the data to our text buffer.
            sender.Buffer.Append(input);

            //get the whole of our buffer
            input = sender.Buffer.ToString();

            input = StripDodgyTerminator(sender, input);

            //if we have nothing left to process then bail out
            if (input.Length == 0)
            {
                return;
            }

            SetLastTerminator(sender, input);

            //replace any \r\n or \n\r or \n with just \r so that we can just deal with one line marker
            input = input.Replace("\n", _newLineMarker);
            input = input.Replace("\r\r", _newLineMarker);

            //our new line will only be a \r as we removed all \n above.

            if (input.Contains(_newLineMarker))
            {
                //we have received a newline char in our stream, this means that
                //we need to raise a command. However we might receive multiple commands
                //in the same stream as our network connection might be slow.

                //we have a special case which is not captured by below.
                //that of just a carriage return being entered in the client.
                //we should pass that up as a valid command.
                if (input == _newLineMarker)
                {
                    RaiseInputReceived(new ConnectionArgs(sender), "");
                }

                //does our input "end" with \r if so then we have a series of full commands.
                //if it doesnt then we have a partial command at the end which needs to wait for more data.

                bool fullCommand = input.EndsWith(_newLineMarker);

                //split on our \r
                string[] commands = input.Split(new string[] { _newLineMarker }, StringSplitOptions.RemoveEmptyEntries);
                string action = string.Empty;
                for (int i = 0; i < commands.Length; i++)
                {
                    action = commands[i].Trim();

                    //if its a full command.
                    if ((i < commands.Length - 1) || (i == commands.Length - 1 && fullCommand))
                    {
                        //fill the last input buffer
                        sender.LastInput = action;
                        //raise the input recevied event
                        RaiseInputReceived(new ConnectionArgs(sender), action);
                    }
                }

                //clear the buffer
                sender.Buffer.Length = 0;
                if (!fullCommand)
                {
                    //if the last command wasnt a full command,
                    //we need to append that action back to the buffer
                    sender.Buffer.Append(action);
                }

            }
        }

        /// <summary>
        /// raises our input receive event safely
        /// </summary>
        /// <param name="connectionArgs">The connection arg</param>
        /// <param name="action">The text that was received</param>
        private void RaiseInputReceived(ConnectionArgs connectionArgs, String action)
        {
            if (InputReceived != null)
            {
                InputReceived(this, connectionArgs, action);
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
        /// <returns></returns>
        private static string StripDodgyTerminator(Connection sender, String input)
        {
            if ((sender.LastInputTerminator == "\r") && input.StartsWith("\n"))
            {
                input = input.Replace("\n", "");
            }
            else if ((sender.LastInputTerminator == "\n") && input.StartsWith("\r"))
            {
                input = input.Replace("\r", "");
            }
            return input;
        }

        /// <summary>
        /// Sets the last input terminator to the termination char(s) we have
        /// just received (if any)
        /// </summary>
        /// <param name="sender">The connection we received input on</param>
        /// <param name="input">The input we are checking</param>
        private static void SetLastTerminator(Connection sender, String input)
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
    }
}