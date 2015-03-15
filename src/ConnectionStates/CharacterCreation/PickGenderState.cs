//-----------------------------------------------------------------------------
// <copyright file="PickGenderState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Permissive License.  All other rights reserved.
// </copyright>
// <summary>
//   Character creation state used to set the player's gender.
//   Author: Fastalanasa
//   Date: May 8, 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using WheelMUD.Core;

    /// <summary>
    /// The character creation step where the player will pick their gender.
    /// </summary>
    public class PickGenderState : CharacterCreationSubState
    {
        private string playerGender;

        /// <summary>
        /// Initializes a new instance of the <see cref="PickGenderState"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        public PickGenderState(Session session) : base(session)
        {
            this.Session.Write("You will now pick your character's gender.");
            this.Session.SetPrompt("Selecting the character's gender ==>");

            this.RefreshScreen();
        }

        /// <summary>
        /// ProcessInput is used to receive the user input during this state.
        /// </summary>
        /// <param name="command">The command text to be processed.</param>
        public override void ProcessInput(string command)
        { 
            string currentCommand = this.GetCommardPart(command.ToLower());

            switch (currentCommand)
            {
                case "select":
                    this.SetGender(command.ToLower());
                    break;
                case "done":
                    this.ProcessDone();
                    break;
                default:
                    this.SendErrorMessage("Invalid command. Please use select or done.");
                    break;
            }
        }

        private void SetGender(string command)
        {
            string currentGender = command.Replace("select ", string.Empty);

            switch (currentGender.ToLower())
            {
                case "male":
                case "female":
                case "eunuch":
                    this.playerGender = currentGender;
                    break;
                default:
                    this.SendErrorMessage(string.Format("'{0}' is an invalid gender selection.", currentGender));
                    break;
            }

            this.RefreshScreen();
        }

        private void ProcessDone()
        {
            // @@@ TODO: Make sure to set the gender to the appropriate object (PlayerBehavior?).
            // Proceed to the next step.
            this.Session.SetPrompt(">");
            this.StateMachine.HandleNextStep(this, StepStatus.Success);
        }

        private void RefreshScreen()
        {
            var sb = new StringBuilder();
            sb.Append("+++++++" + Environment.NewLine);
            sb.Append("You have the following gender choices:" + Environment.NewLine + Environment.NewLine);
            sb.Append("Male" + Environment.NewLine);
            sb.Append("Female" + Environment.NewLine);
            sb.Append("Eunuch" + Environment.NewLine);
            sb.Append(Environment.NewLine);
            if (string.IsNullOrEmpty(this.playerGender))
            {
                sb.Append("<%b%><%red%>No gender has been selected.<%n%>" + Environment.NewLine);
            }
            else
            {
                sb.AppendFormat("<%green%>The chosen gender is {0}.<%n%>" + Environment.NewLine, this.playerGender);
            }

            sb.Append("<%yellow%>===============================================================" + Environment.NewLine);
            sb.Append("To pick a gender use the select command. Example: select female" + Environment.NewLine);
            sb.Append("When you are done picking a gender type done." + Environment.NewLine);
            sb.Append("===============================================================<%n%>");

            this.Session.Write(sb.ToString());
        }

        private string GetCommardPart(string command)
        {
            string retval = string.Empty;

            try
            {
                retval = Regex.Match(command, @"select|done").Value;
            }
            catch (ArgumentException)
            {
                // Syntax error in the regular expression
            }

            return retval;
        }

        private void SendErrorMessage(string message)
        {
            var divider = new StringBuilder();
            var wrappedMessage = new StringBuilder();

            foreach (char t in message)
            {
                divider.Append("=");
            }

            wrappedMessage.Append("<%red%>" + divider + Environment.NewLine);
            wrappedMessage.Append(message + Environment.NewLine);
            wrappedMessage.Append(divider + "<%n%>");

            this.Session.Write(wrappedMessage.ToString());
        }
    }
}
