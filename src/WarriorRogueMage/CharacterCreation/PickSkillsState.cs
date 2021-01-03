//-----------------------------------------------------------------------------
// <copyright file="PickSkillsState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage.CharacterCreation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using WheelMUD.ConnectionStates;
    using WheelMUD.Core;

    /// <summary>The character creation step where the player will pick their skills.</summary>
    public class PickSkillsState : CharacterCreationSubState
    {
        private readonly string formattedSkills;
        private readonly List<GameSkill> gameSkills;
        private readonly List<GameSkill> selectedSkills = new List<GameSkill>();

        /// <summary>Initializes a new instance of the <see cref="PickSkillsState"/> class.</summary>
        /// <param name="session">The session.</param>
        public PickSkillsState(Session session)
            : base(session)
        {
            this.Session.Write("You will now pick your character's starting skills.");
            this.gameSkills = GameSystemController.Instance.GameSkills;
            this.formattedSkills = this.FormatSkillText();
            this.RefreshScreen(false);
        }

        /// <summary>Processes the text that the player sends while in this state.</summary>
        /// <param name="command">The command that the player just sent.</param>
        public override void ProcessInput(string command)
        {
            var commandParts = command.Split(' ');
            string currentCommand = commandParts[0].ToLower();
            switch (currentCommand)
            {
                case "clear":
                    this.ClearSkills();
                    break;
                case "view":
                    if (commandParts.Length > 1)
                    {
                        this.ViewSkillDescription(commandParts[1]);
                    }
                    else
                    {
                        WrmChargenCommon.SendErrorMessage(this.Session, "Please select which skill you would like to view details for, like 'view axes'.");
                    }
                    break;
                case "list":
                    this.RefreshScreen();
                    break;
                case "done":
                    this.ProcessDone();
                    break;
                default:
                    this.SetSkill(currentCommand);
                    break;
            }
        }

        private GameSkill GetSkill(string skillName)
        {
            return WrmChargenCommon.GetFirstPriorityMatch(skillName, gameSkills);
        }

        public override string BuildPrompt()
        {
            return "Select the character's skills\n> ";
        }

        private bool SetSkill(string skillName)
        {
            var selectedSkill = GetSkill(skillName);
            if (selectedSkill == null)
            {
                WrmChargenCommon.SendErrorMessage(this.Session, "That skill does not exist.");
                return false;
            }

            if (selectedSkills.Contains(selectedSkill))
            {
                WrmChargenCommon.SendErrorMessage(this.Session, "You have already selected that skill.");
                return false;
            }

            if (this.selectedSkills.Count() >= 3)
            {
                WrmChargenCommon.SendErrorMessage(this.Session, "You have already selected all 3 skills.");
                return false;
            }

            this.selectedSkills.Add(selectedSkill);
            this.RefreshScreen();
            this.Session.WritePrompt();
            return true;
        }

        private void ClearSkills()
        {
            this.selectedSkills.Clear();
            this.RefreshScreen();
            this.Session.WritePrompt();
        }

        private void ViewSkillDescription(string skillName)
        {
            GameSkill foundSkill = GetSkill(skillName);
            if (foundSkill == null)
            {
                WrmChargenCommon.SendErrorMessage(this.Session, "That skill does not exist.");
                return;
            }

            var sb = new StringBuilder();
            sb.Append("<%b%><%yellow%>=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=" + Environment.NewLine);
            sb.AppendFormat("Description for {0}" + Environment.NewLine, foundSkill.Name);
            sb.Append("=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=<%n%>" + Environment.NewLine);
            sb.Append("<%b%><%white%>" + foundSkill.Description + Environment.NewLine);
            sb.Append("<%b%><%yellow%>=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=<%n%>");

            this.Session.Write(sb.ToString());
        }

        private void ProcessDone()
        {
            if (this.selectedSkills.Count() != 3)
            {
                WrmChargenCommon.SendErrorMessage(this.Session, "Please select all your starting skills before proceeding to the next step.");
                return;
            }

            // Assign the skills to the PlayerBehavior's parent, which should be a Thing object.
            var playerBehavior = this.Session.Thing.Behaviors.FindFirst<PlayerBehavior>();
            foreach (var gameSkill in this.selectedSkills)
            {
                playerBehavior.Parent.Skills.Add(gameSkill.Name, gameSkill);
            }

            // Proceed to the next step.
            this.StateMachine.HandleNextStep(this, StepStatus.Success);
        }

        private string FormatSkillText()
        {
            var skillQueue = new Queue();
            var text = new StringBuilder();
            int rows = this.gameSkills.Count / 4;
            var longestSkillName = 0;

            foreach (var gameSkill in this.gameSkills)
            {
                // Find out what skill name is the longest
                if (gameSkill.Name.Length > longestSkillName)
                {
                    longestSkillName = gameSkill.Name.Length;
                }

                skillQueue.Enqueue(gameSkill);
            }

            // TODO: The columns calculations and such look wrong...
            for (int i = 0; i < rows; i++)
            {
                string skill1 = WrmChargenCommon.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                string skill2 = WrmChargenCommon.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                string skill3 = WrmChargenCommon.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                string skill4 = WrmChargenCommon.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                text.AppendFormat("{0}  {1}  {2}  {3}" + Environment.NewLine, skill1, skill2, skill3, skill4);
            }

            if ((this.gameSkills.Count % 4) > 0)
            {
                int columns = this.gameSkills.Count - (rows * 4);
                switch (columns)
                {
                    case 1:
                        string skillcolumn1 = WrmChargenCommon.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                        text.AppendFormat("{0}" + Environment.NewLine, skillcolumn1);
                        break;
                    case 2:
                        string sk1 = WrmChargenCommon.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                        string sk2 = WrmChargenCommon.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                        text.AppendFormat("{0}  {1}" + Environment.NewLine, sk1, sk2);
                        break;
                    case 3:
                        string skl1 = WrmChargenCommon.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                        string skl2 = WrmChargenCommon.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                        string skl3 = WrmChargenCommon.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                        text.AppendFormat("{0}  {1}  {2}" + Environment.NewLine, skl1, skl2, skl3);
                        break;
                }
            }

            text.Append(Environment.NewLine);
            return text.ToString();
        }

        private void RefreshScreen(bool sendPrompt = true)
        {
            var nl = Environment.NewLine;
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("You may pick three starting skills for your character.");
            int n = 1;
            foreach (var skill in this.selectedSkills)
            {
                sb.AppendFormat("Skill #{0} : {1}{2}", n, skill.Name, nl);
                n++;
            }
            sb.AppendLine();
            sb.AppendLine("<%green%>Please select 3 from the list below:<%n%>");
            sb.AppendLine(this.formattedSkills);
            sb.AppendFormat("<%green%>You have {0} skills left.<%n%>" + Environment.NewLine, 3 - this.selectedSkills.Count());
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