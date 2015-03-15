//-----------------------------------------------------------------------------
// <copyright file="SetStartingStatsState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Permissive License.  All other rights reserved.
// </copyright>
// <summary>
//   Character creation state used to set the initial values for the player's
//   base stats.
//   Author: Fastalanasa
//   Date: May 6, 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using WheelMUD.Core;

    /// <summary>
    /// Matches the current stat names.
    /// </summary>
    public enum StatNames
    {
        Warrior,
        Rogue,
        Mage
    }

    /// <summary>
    /// The character creation step where the player will set their stats.
    /// </summary>
    public class SetStartingStatsState : CharacterCreationSubState
    {
        private int Warrior;
        private int Rogue;
        private int Mage;
        private int CharacterPoints = 10;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetStartingStatsState"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        public SetStartingStatsState(Session session) : base(session)
        {
            this.Session.Write("You will now set your basic stats.");
            this.Session.SetPrompt("Setting player stats ==>");

            this.RefreshScreen();
        }

        /// <summary>
        /// Processes the text that the player sends while in this state.
        /// </summary>
        /// <param name="command">
        /// The command that the player just sent.
        /// </param>
        public override void ProcessInput(string command)
        {
            string commandPart = this.GetCommardPart(command);

            switch (commandPart.ToLower())
            {
                case "warrior":
                    this.ProcessCommand(StatNames.Warrior, command);
                    break;
                case "rogue":
                    this.ProcessCommand(StatNames.Rogue, command);
                    break;
                case "mage":
                    this.ProcessCommand(StatNames.Mage, command);
                    break;
                case "done":
                    this.SetPlayerBehaviorStats();

                    // @@@ TODO: Add saving stats to RavenDb here.
                    // Proceed to the next step.
                    this.Session.SetPrompt(">");
                    this.StateMachine.HandleNextStep(this, StepStatus.Success);
                    break;
                default:
                    this.SendErrorMessage("Unknown command. Please use warrior, rogue, mage, or done.");
                    break;
            }
        }

        private void RefreshScreen()
        {
            var sb = new StringBuilder();
            sb.Append("+++++++" + Environment.NewLine);
            sb.Append("You have 10 character points to be divided between 3 stats." + Environment.NewLine);
            sb.Append("No stat can have more than 6 points. Stats can be zero." + Environment.NewLine + Environment.NewLine);
            sb.AppendFormat("Warrior : {0}" + Environment.NewLine, this.Warrior);
            sb.AppendFormat("Rogue   : {0}" + Environment.NewLine, this.Rogue);
            sb.AppendFormat("Mage    : {0}" + Environment.NewLine, this.Mage);
            sb.Append(Environment.NewLine);
            sb.AppendFormat("You have {0} character points left.", this.CharacterPoints);
            sb.Append(Environment.NewLine);
            sb.Append("<%yellow%>====================================================================" + Environment.NewLine);
            sb.Append("To add points to a stat use the + operator. Example: warrior +6" + Environment.NewLine);
            sb.Append("To subtract points to a stat use the - operator. Example: warrior -6" + Environment.NewLine);
            sb.Append("When you are done distributing the character points type done." + Environment.NewLine);
            sb.Append("====================================================================<%n%>");

            this.Session.Write(sb.ToString());
        }

        private void ProcessCommand(StatNames currentStat, string command)
        {
            switch (currentStat)
            {
                case StatNames.Warrior:
                    this.ProcessWarrior(command);
                    break;
                case StatNames.Rogue:
                    this.ProcessRogue(command);
                    break;
                case StatNames.Mage:
                    this.ProcessMage(command);
                    break;
                default:
                    break;
            }

            this.RefreshScreen();
        }

        private void ProcessMage(string command)
        {
            string mageOperation = GetOperation(command);

            if (!string.IsNullOrEmpty(mageOperation))
            {
                string mageOperator = mageOperation.Substring(0, 1);
                int amount;
                int.TryParse(mageOperation.Replace(mageOperator, string.Empty), out amount);
                this.DoOperation(mageOperator, ref this.Mage, amount);
            }
            else
            {
                this.SendInvalidOperatorMessage();
            }
        }

        private void ProcessRogue(string command)
        {
            string rogueOperation = GetOperation(command);

            if (!string.IsNullOrEmpty(rogueOperation))
            {
                string rogueOperator = rogueOperation.Substring(0, 1);
                int amount;
                int.TryParse(rogueOperation.Replace(rogueOperator, string.Empty), out amount);
                this.DoOperation(rogueOperator, ref this.Rogue, amount);
            }
            else
            {
                this.SendInvalidOperatorMessage();
            }
        }

        private void ProcessWarrior(string command)
        {
            string warriorOperation = GetOperation(command);

            if (!string.IsNullOrEmpty(warriorOperation))
            {
                string warriorOperator = warriorOperation.Substring(0, 1);
                int amount;
                int.TryParse(warriorOperation.Replace(warriorOperator, string.Empty), out amount);
                this.DoOperation(warriorOperator, ref this.Warrior, amount); 
            }
            else
            {
                this.SendInvalidOperatorMessage();
            }
        }

        private void DoOperation(string operand, ref int stat, int amount)
        {
            int currValue = stat;

            switch (operand)
            {
                case "+":
                    if ((currValue += amount) > 6)
                    {
                        this.SendErrorMessage("That stat can not be greater than 6.");
                    }
                    else
                    {
                        int currPoints = CharacterPoints;

                        if ((currPoints -= amount) < 0)
                        {
                            SendErrorMessage("You tried to add more points than were available.");
                        }
                        else
                        {
                            stat += amount;
                            CharacterPoints -= amount;
                        }
                    }
                    break;
                case "-":
                    if ((currValue -= amount) < 0)
                    {
                        this.SendErrorMessage("That stat can not be less than 0.");
                    }
                    else
                    {
                        int currPoints = CharacterPoints;

                        if ((currPoints += amount) > 10)
                        {
                            SendErrorMessage("You tried to subtract more points than were available.");
                        }
                        else
                        {
                            stat -= amount;
                            CharacterPoints += amount;
                        }
                    }
                    break;
                default:
                    this.SendErrorMessage("Invalid operation. Please use + or - to add or subtract.");
                    break;
            }
        }

        private void SendInvalidOperatorMessage()
        {
            this.SendErrorMessage("No valid operator was found.");
        }

        private string GetCommardPart(string command)
        {
            string retval = string.Empty; 

            try
            {
                retval = Regex.Match(command, @"(\w*)").Value;
            }
            catch (ArgumentException)
            {
                // Syntax error in the regular expression
            }

            return retval;
        }

        private static string GetOperation(string stringToParse)
        {
            string retval = string.Empty;

            try
            {
                retval = Regex.Match(stringToParse, @"[-+]?\b\d+\b").Value;
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

        private void SetPlayerBehaviorStats()
        {
            var playerBehavior = this.StateMachine.NewCharacter.BehaviorManager.FindFirst<PlayerBehavior>();

            playerBehavior.Parent.Stats["WARRIOR"].SetValue(this.Warrior, this.Session.Thing);
            playerBehavior.Parent.Stats["ROGUE"].SetValue(this.Rogue, this.Session.Thing);
            playerBehavior.Parent.Stats["MAGE"].SetValue(this.Mage, this.Session.Thing);
        }
    }
}
