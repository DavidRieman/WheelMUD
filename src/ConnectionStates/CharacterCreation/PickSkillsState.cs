//-----------------------------------------------------------------------------
// <copyright file="PickSillsState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Permissive License.  All other rights reserved.
// </copyright>
// <summary>
//   Character creation state used to set the initial skills for the player.
//   Author: Fastalanasa
//   Date: May 8, 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using WheelMUD.Core;
    using WheelMUD.Core.GameEngine;

    /// <summary>
    /// The character creation step where the player will pick their skills.
    /// </summary>
    public class PickSkillsState : CharacterCreationSubState
    {
        private string skillOne;
        private string skillTwo;
        private string skillThree;
        private int longestSkillName;
        private string formattedSkills;
        private int skillCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="PickSkillsState"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        public PickSkillsState(Session session) : base(session)
        {
            this.Session.Write("You will now pick your character's starting skills.");
            this.Session.SetPrompt("Selecting the character's skills ==>");

            this.FormatSkillText();

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
            string currentCommand = this.GetCommardPart(command.ToLower());

            switch (currentCommand)
            {
                case "select":
                    this.SetSkill(command.ToLower());
                    break;
                case "clear":
                    this.ClearSkills();
                    break;
                case "view":
                    this.ViewSkillDescription(command.ToLower());
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

        private void SetSkill(string skillToSet)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            string currentSkill = skillToSet.Replace("select ", string.Empty);
            currentSkill = textInfo.ToTitleCase(currentSkill);

            if (string.IsNullOrEmpty(this.skillOne))
            {
                this.skillOne = currentSkill;
                this.skillCount = 1;
            }
            else if (string.IsNullOrEmpty(this.skillTwo))
            {
                this.skillTwo = currentSkill;
                this.skillCount = 2;
            }
            else if (string.IsNullOrEmpty(this.skillThree))
            {
                this.skillThree = currentSkill;
                this.skillCount = 3;
            }

            this.RefreshScreen();
        }

        private void ClearSkills()
        {
            this.skillOne = string.Empty;
            this.skillTwo = string.Empty;
            this.skillThree = string.Empty;
            this.skillCount = 0;

            this.RefreshScreen();
        }

        private void ViewSkillDescription(string skill)
        {
            string skillToView = skill.Replace("view ", string.Empty);

            var game = GameSystemController.Instance;

            GameSkill foundSkill = game.GameSkills.Find(s => s.Name.ToLower() == skillToView);

            if (foundSkill != null)
            {
                var sb = new StringBuilder();
                sb.Append("<%b%><%yellow%>=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=" + Environment.NewLine);
                sb.AppendFormat("Description for {0}" + Environment.NewLine, foundSkill.Name);
                sb.Append("=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=<%n%>" + Environment.NewLine);
                sb.Append("<%b%><%white%>" + foundSkill.Description + Environment.NewLine);
                sb.Append("<%b%><%yellow%>=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=<%n%>");

                this.Session.Write(sb.ToString());
            }
            else
            {
                this.SendErrorMessage("That skill does not exist.");
            }
        }

        private void ProcessDone()
        {
            // @@@ TODO: Make sure to set the skills to the appropriate object (PlayerBehavior?).
            // @@@ TODO: Remember to save the skills to RavenDb once it's setup.
            // Proceed to the next step.
            this.Session.SetPrompt(">");
            this.StateMachine.HandleNextStep(this, StepStatus.Success);
        }

        private void FormatSkillText()
        {
            var skillQueue = new Queue();
            var game = GameSystemController.Instance;
            var text = new StringBuilder();
            int rows = game.GameSkills.Count / 4;

            foreach (var gameSkill in game.GameSkills)
            {
                // Find out what skill name is the longest
                if (gameSkill.Name.Length > this.longestSkillName)
                {
                    this.longestSkillName = gameSkill.Name.Length;
                }

                skillQueue.Enqueue(gameSkill);
            }

            try
            {
                for (int i = 0; i < rows; i++)
                {
                    string skill1 = this.FormatToColumn(((GameSkill)skillQueue.Dequeue()).Name);
                    string skill2 = this.FormatToColumn(((GameSkill)skillQueue.Dequeue()).Name);
                    string skill3 = this.FormatToColumn(((GameSkill)skillQueue.Dequeue()).Name);
                    string skill4 = this.FormatToColumn(((GameSkill)skillQueue.Dequeue()).Name);
                    text.AppendFormat("{0}  {1}  {2}  {3}" + Environment.NewLine, skill1, skill2, skill3, skill4);
                }

                if ((game.GameSkills.Count % 4) > 0)
                {
                    int columns = game.GameSkills.Count - (rows * 4);

                    switch (columns)
                    {
                        case 1:
                            string skillcolumn1 = this.FormatToColumn(((GameSkill)skillQueue.Dequeue()).Name);
                            text.AppendFormat("{0}" + Environment.NewLine, skillcolumn1);
                            break;
                        case 2:
                            string sk = this.FormatToColumn(((GameSkill)skillQueue.Dequeue()).Name);
                            string sk1 = this.FormatToColumn(((GameSkill)skillQueue.Dequeue()).Name);
                            text.AppendFormat("{0}  {1}" + Environment.NewLine, sk, sk);
                            break;
                        case 3:
                            string skl1 = this.FormatToColumn(((GameSkill)skillQueue.Dequeue()).Name);
                            string skl2 = this.FormatToColumn(((GameSkill)skillQueue.Dequeue()).Name);
                            string skl3 = this.FormatToColumn(((GameSkill)skillQueue.Dequeue()).Name);
                            text.AppendFormat("{0}  {1}  {2}" + Environment.NewLine, skl1, skl2, skl3);
                            break;
                    }
                }
            }
            catch { }

            text.Append(Environment.NewLine);

            this.formattedSkills = text.ToString();
        }

        private string FormatToColumn(string skillName)
        {
            string retval = string.Empty;

            if (skillName.Length < this.longestSkillName)
            {
                retval = skillName.PadRight(this.longestSkillName, ' ');
            }
            else
            {
                retval = skillName;
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
            sb.Append("You may pick three starting skills for your character." + Environment.NewLine + Environment.NewLine);
            sb.AppendFormat("Skill #1 : {0}" + Environment.NewLine, this.skillOne);
            sb.AppendFormat("Skill #2 : {0}" + Environment.NewLine, this.skillTwo);
            sb.AppendFormat("Skill #3 : {0}" + Environment.NewLine, this.skillThree);
            sb.Append(Environment.NewLine);
            sb.Append("<%green%>Please select 3 from the list below:<%n%>" + Environment.NewLine);
            sb.Append(this.formattedSkills);
            sb.AppendFormat("<%green%>You have {0} skills left.<%n%>" + Environment.NewLine, 3 - this.skillCount);
            sb.Append("<%yellow%>=========================================================================" + Environment.NewLine);
            sb.Append("To pick a skill use the select command. Example: select unarmed" + Environment.NewLine);
            sb.Append("To view a skill's description use the view command. Example: view unarmed" + Environment.NewLine);
            sb.Append("To see this screen again type list." + Environment.NewLine);
            sb.Append("When you are done picking your three skills, type done." + Environment.NewLine);
            sb.Append("=========================================================================<%n%>");

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
