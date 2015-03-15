//-----------------------------------------------------------------------------
// <copyright file="PickTalentsState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Permissive License.  All other rights reserved.
// </copyright>
// <summary>
//   Character creation state used to set the initial talents for the player.
//   Author: Fastalanasa
//   Date: May 8, 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using WheelMUD.Core;
    using WheelMUD.Core.GameEngine;

    /// <summary>
    /// The character creation step where the player will pick their talents.
    /// </summary>
    public class PickTalentsState : CharacterCreationSubState
    {
        private int longestTalentName;
        private string selectedTalent;
        private string formattedTalents;
        private List<GenericTableEntry> talents;

        /// <summary>
        /// Initializes a new instance of the <see cref="PickTalentsState"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        public PickTalentsState(Session session) : base(session)
        {
            this.Session.Write("You will now pick your character's starting talent.");
            this.Session.SetPrompt("Selecting the character's starting talent ==>");

            this.FormatTalentText();

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
                    this.SetTalent(command.ToLower());
                    break;
                case "clear":
                    this.ClearSelectedTalent();
                    break;
                case "view":
                    this.ViewTalentDescription(command.ToLower());
                    break;
                case "done":
                    this.ProcessDone();
                    break;
                case "list":
                    this.RefreshScreen();
                    break;
                default:
                    this.SendErrorMessage("Invalid command. Please use select, clear, view, list, or done.");
                    break;
            }
        }

        #region Private Members

        private void SetTalent(string talentToSet)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            string currentTalent = talentToSet.Replace("select ", string.Empty);
            this.selectedTalent = textInfo.ToTitleCase(currentTalent);

            this.RefreshScreen();
        }

        private void ClearSelectedTalent()
        {
            this.selectedTalent = string.Empty;

            this.RefreshScreen();
        }

        private void ViewTalentDescription(string talent)
        {
            string talentToView = talent.Replace("view ", string.Empty);

            GenericTableEntry foundTalent = this.talents.Find(s => s.Name.ToLower() == talentToView);

            if (foundTalent != null)
            {
                var sb = new StringBuilder();
                sb.Append("<%b%><%yellow%>=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=" + Environment.NewLine);
                sb.AppendFormat("Description for {0}" + Environment.NewLine, foundTalent.Name);
                sb.Append("=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=<%n%>" + Environment.NewLine);
                sb.Append("<%b%><%white%>" + foundTalent.Description + Environment.NewLine);
                sb.Append("<%b%><%yellow%>=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=<%n%>");

                this.Session.Write(sb.ToString());
            }
            else
            {
                this.SendErrorMessage("That talent does not exist.");
            }
        }

        private void ProcessDone()
        {
            // @@@ TODO: Make sure to set the talent to the appropriate object (PlayerBehavior?).
            // @@@ TODO: Remember to save the talent to RavenDb once it's setup.
            // Proceed to the next step.
            this.Session.SetPrompt(">");
            this.StateMachine.HandleNextStep(this, StepStatus.Success);
        }

        private void FormatTalentText()
        {
            var talentQueue = new Queue();
            this.talents = new List<GenericTableEntry>();
            var game = GameSystemController.Instance;
            var text = new StringBuilder();

            List<GameTable> talents = (from g in game.GameTables
                                             where g.Key.ToLower() == "talents"
                                             select g.Value).ToList();

            Dictionary<string, GenericTableEntry> talentPairs = talents[0].GameTableEntries;

            foreach (var gameTalent in talentPairs)
            {
                // Find out what skill name is the longest
                if (gameTalent.Key.Length > this.longestTalentName)
                {
                    this.longestTalentName = gameTalent.Key.Length;
                }

                talentQueue.Enqueue(gameTalent.Value);
                this.talents.Add(gameTalent.Value);
            }

            int rows = talents[0].GameTableEntries.Count;

            try
            {
                for (int i = 0; i < rows; i++)
                {
                    string talent1 = this.FormatToColumn(((GenericTableEntry)talentQueue.Dequeue()).Name);
                    string talent2 = this.FormatToColumn(((GenericTableEntry)talentQueue.Dequeue()).Name);
                    string talent3 = this.FormatToColumn(((GenericTableEntry)talentQueue.Dequeue()).Name);
                    string talent4 = this.FormatToColumn(((GenericTableEntry)talentQueue.Dequeue()).Name);
                    text.AppendFormat("{0}  {1}  {2}  {3}" + Environment.NewLine, talent1, talent2, talent3, talent4);
                }

                if ((rows % 4) > 0)
                {
                    int columns = rows - (rows * 4);

                    switch (columns)
                    {
                        case 1:
                            string talentcolumn1 = this.FormatToColumn(((GameSkill)talentQueue.Dequeue()).Name);
                            text.AppendFormat("{0}" + Environment.NewLine, talentcolumn1);
                            break;
                        case 2:
                            string tk = this.FormatToColumn(((GameSkill)talentQueue.Dequeue()).Name);
                            string tk1 = this.FormatToColumn(((GameSkill)talentQueue.Dequeue()).Name);
                            text.AppendFormat("{0}  {1}" + Environment.NewLine, tk, tk);
                            break;
                        case 3:
                            string tkl1 = this.FormatToColumn(((GameSkill)talentQueue.Dequeue()).Name);
                            string tkl2 = this.FormatToColumn(((GameSkill)talentQueue.Dequeue()).Name);
                            string tkl3 = this.FormatToColumn(((GameSkill)talentQueue.Dequeue()).Name);
                            text.AppendFormat("{0}  {1}  {2}" + Environment.NewLine, tkl1, tkl2, tkl3);
                            break;
                    }
                }
            }
            catch
            {
            }

            text.Append(Environment.NewLine);

            this.formattedTalents = text.ToString();
        }

        private string FormatToColumn(string talentName)
        {
            string retval = string.Empty;

            if (talentName.Length < this.longestTalentName)
            {
                retval = talentName.PadRight(this.longestTalentName, ' ');
            }
            else
            {
                retval = talentName;
            }

            return retval;
        }

        private string GetCommardPart(string command)
        {
            string retval = string.Empty;

            try
            {
                retval = Regex.Match(command, @"select|clear|view|list|done").Value;
            }
            catch (ArgumentException)
            {
                // Syntax error in the regular expression
            }

            return retval;
        }

        private void RefreshScreen()
        {
            var sb = new StringBuilder();
            sb.Append("+++++++" + Environment.NewLine);
            sb.Append("You may pick one starting talent for your character." + Environment.NewLine + Environment.NewLine);
            sb.AppendFormat("Selected Talent : {0}" + Environment.NewLine, this.selectedTalent);
            sb.Append(Environment.NewLine);
            sb.Append("<%green%>Please select 1 from the list below:<%n%>" + Environment.NewLine);
            sb.Append(this.formattedTalents);
            sb.Append("<%yellow%>==========================================================================" + Environment.NewLine);
            sb.Append("To pick a talent use the select command. Example: select unarmed" + Environment.NewLine);
            sb.Append("To view a talent's description use the view command. Example: view unarmed" + Environment.NewLine);
            sb.Append("To see this screen again type list." + Environment.NewLine);
            sb.Append("When you are done picking your talent, type done." + Environment.NewLine);
            sb.Append("==========================================================================<%n%>");

            this.Session.Write(sb.ToString());
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

        #endregion
    }
}