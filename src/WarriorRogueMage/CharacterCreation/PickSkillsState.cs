//-----------------------------------------------------------------------------
// <copyright file="PickSkillsState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Character creation state used to set the initial skills for the player.
//   Author: Fastalanasa
//   Date: May 8, 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage.CharacterCreation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using WheelMUD.ConnectionStates;
    using WheelMUD.Core;

    /// <summary>The character creation step where the player will pick their skills.</summary>
    public class PickSkillsState : CharacterCreationSubState
    {
        private string skillOne;
        private string skillTwo;
        private string skillThree;
        private int longestSkillName;
        private string formattedSkills;
        private int skillCount;
        private List<GameSkill> gameSkills;

        /// <summary>Initializes a new instance of the <see cref="PickSkillsState"/> class.</summary>
        /// <param name="session">The session.</param>
        public PickSkillsState(Session session)
            : base(session)
        {
            this.Session.Write("You will now pick your character's starting skills.");
            this.gameSkills = GameSystemController.Instance.GameSkills;
            this.FormatSkillText();
            this.RefreshScreen(false);
        }

        /// <summary>Processes the text that the player sends while in this state.</summary>
        /// <param name="command">The command that the player just sent.</param>
        public override void ProcessInput(string command)
        {
            string currentCommand = command.ToLower().Trim();
            switch (currentCommand)
            {
                case "clear":
                    this.ClearSkills();
                    break;
                case "view":
                    this.ViewSkillDescription(currentCommand);
                    break;
                case "done":
                    this.ProcessDone();
                    break;
                case "list":
                    this.RefreshScreen();
                    break;
                default:
                    string tentativeSkillName = this.GetCommardPart(currentCommand);

                    if (tentativeSkillName == string.Empty)
                    {
                        this.SetSkill(currentCommand);
                    }
                    else
                    {
                        WrmChargenCommon.SendErrorMessage(this.Session, "Invalid command. Please use clear, view, list, or done.");
                    }

                    break;
            }
        }

        public override string BuildPrompt()
        {
            return "Select the character's skills\n> ";
        }

        private string GetCommardPart(string command)
        {
            string retval = string.Empty;

            try
            {
                retval = Regex.Match(command, @"clear|view|list|done").Value;
            }
            catch (ArgumentException)
            {
                // Syntax error in the regular expression
            }

            return retval;
        }

        private void SetSkill(string skillToSet)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            string currentSkill = textInfo.ToTitleCase(skillToSet);

            GameSkill currentSelection = (from r in this.gameSkills
                                          where r.Name == currentSkill
                                          select r).FirstOrDefault();

            if (currentSelection != null)
            {
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
            }
            else
            {
                WrmChargenCommon.SendErrorMessage(this.Session, "That skill does not exist.");
            }

            this.RefreshScreen();
            this.Session.WritePrompt();
        }

        private void ClearSkills()
        {
            this.skillOne = string.Empty;
            this.skillTwo = string.Empty;
            this.skillThree = string.Empty;
            this.skillCount = 0;

            this.RefreshScreen();
            this.Session.WritePrompt();
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
                WrmChargenCommon.SendErrorMessage(this.Session, "That skill does not exist.");
            }
        }

        private void ProcessDone()
        {
            int doneSkillCount = 0;
            var newSkills = new List<GameSkill>();

            // Process the first skill
            if (!string.IsNullOrEmpty(this.skillOne))
            {
                var firstSkill = this.gameSkills.Find(s => s.Name.ToLower() == this.skillOne.ToLower());

                if (firstSkill != null)
                {
                    newSkills.Add(firstSkill);
                    doneSkillCount++;
                }
            }

            // Process the second skill
            if (!string.IsNullOrEmpty(this.skillTwo))
            {
                var secondtSkill = this.gameSkills.Find(s => s.Name.ToLower() == this.skillTwo.ToLower());

                if (secondtSkill != null)
                {
                    newSkills.Add(secondtSkill);
                    doneSkillCount++;
                }
            }

            // Process the third skill
            if (!string.IsNullOrEmpty(this.skillThree))
            {
                var thirdtSkill = this.gameSkills.Find(s => s.Name.ToLower() == this.skillThree.ToLower());

                if (thirdtSkill != null)
                {
                    newSkills.Add(thirdtSkill);
                    doneSkillCount++;
                }
            }

            if (doneSkillCount == 3)
            {
                var playerBehavior = this.StateMachine.NewCharacter.Behaviors.FindFirst<PlayerBehavior>();

                // Assign the skills to the PlayerBehavior's parent, which should be a Thing object.
                foreach (var gameSkill in newSkills)
                {
                    playerBehavior.Parent.Skills.Add(gameSkill.Name, gameSkill);
                }

                // Proceed to the next step.
                this.StateMachine.HandleNextStep(this, StepStatus.Success);
                return;
            }
            else
            {
                WrmChargenCommon.SendErrorMessage(this.Session, "Please fill all of the skill slots before proceeding to the next step.");
            }

            this.RefreshScreen();
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
                    string skill1 = WrmChargenCommon.FormatToColumn(this.longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                    string skill2 = WrmChargenCommon.FormatToColumn(this.longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                    string skill3 = WrmChargenCommon.FormatToColumn(this.longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                    string skill4 = WrmChargenCommon.FormatToColumn(this.longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                    text.AppendFormat("{0}  {1}  {2}  {3}" + Environment.NewLine, skill1, skill2, skill3, skill4);
                }

                if ((game.GameSkills.Count % 4) > 0)
                {
                    int columns = game.GameSkills.Count - (rows * 4);

                    switch (columns)
                    {
                        case 1:
                            string skillcolumn1 = WrmChargenCommon.FormatToColumn(this.longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                            text.AppendFormat("{0}" + Environment.NewLine, skillcolumn1);
                            break;
                        case 2:
                            string sk1 = WrmChargenCommon.FormatToColumn(this.longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                            string sk2 = WrmChargenCommon.FormatToColumn(this.longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                            text.AppendFormat("{0}  {1}" + Environment.NewLine, sk1, sk2);
                            break;
                        case 3:
                            string skl1 = WrmChargenCommon.FormatToColumn(this.longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                            string skl2 = WrmChargenCommon.FormatToColumn(this.longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                            string skl3 = WrmChargenCommon.FormatToColumn(this.longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                            text.AppendFormat("{0}  {1}  {2}" + Environment.NewLine, skl1, skl2, skl3);
                            break;
                    }
                }
            }
            catch
            {
            }

            text.Append(Environment.NewLine);

            this.formattedSkills = text.ToString();
        }

        private void RefreshScreen(bool sendPrompt = true)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("You may pick three starting skills for your character.");
            sb.AppendFormat("Skill #1 : {0}" + Environment.NewLine, this.skillOne);
            sb.AppendFormat("Skill #2 : {0}" + Environment.NewLine, this.skillTwo);
            sb.AppendFormat("Skill #3 : {0}" + Environment.NewLine, this.skillThree);
            sb.AppendLine();
            sb.AppendLine("<%green%>Please select 3 from the list below:<%n%>");
            sb.AppendLine(this.formattedSkills);
            sb.AppendFormat("<%green%>You have {0} skills left.<%n%>" + Environment.NewLine, 3 - this.skillCount);
            sb.AppendLine("<%yellow%>=========================================================================");
            sb.AppendLine("To pick a skill, type the skill's name. Example: unarmed");
            sb.AppendLine("To view a skill's description use the view command. Example: view unarmed");
            sb.AppendLine("To see this screen again type list.");
            sb.AppendLine("When you are done picking your three skills, type done.");
            sb.AppendLine("=========================================================================<%n%>");

            this.Session.Write(sb.ToString(), sendPrompt);
        }
    }
}