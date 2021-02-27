//-----------------------------------------------------------------------------
// <copyright file="PickSkillsState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Utilities;

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
            Session.Write($"You will now pick your character's starting skills.{AnsiSequences.NewLine}");
            gameSkills = GameSystemController.Instance.GameSkills;
            formattedSkills = FormatSkillText();
            RefreshScreen(false);
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
                    ClearSkills();
                    break;
                case "view":
                    if (commandParts.Length > 1)
                    {
                        ViewSkillDescription(commandParts[1]);
                    }
                    else
                    {
                        WrmChargenCommon.SendErrorMessage(Session, "Please select which skill you would like to view details for, like 'view axes'.");
                    }
                    break;
                case "list":
                    RefreshScreen();
                    break;
                case "done":
                    ProcessDone();
                    break;
                default:
                    SetSkill(currentCommand);
                    break;
            }
        }

        private GameSkill GetSkill(string skillName)
        {
            return WrmChargenCommon.GetFirstPriorityMatch(skillName, gameSkills);
        }

        public override string BuildPrompt()
        {
            return $"Select the character's skills{AnsiSequences.NewLine}";
        }

        private bool SetSkill(string skillName)
        {
            var selectedSkill = GetSkill(skillName);
            if (selectedSkill == null)
            {
                WrmChargenCommon.SendErrorMessage(Session, "That skill does not exist.");
                return false;
            }

            if (selectedSkills.Contains(selectedSkill))
            {
                WrmChargenCommon.SendErrorMessage(Session, "You have already selected that skill.");
                return false;
            }

            if (selectedSkills.Count() >= 3)
            {
                WrmChargenCommon.SendErrorMessage(Session, "You have already selected all 3 skills.");
                return false;
            }

            selectedSkills.Add(selectedSkill);
            RefreshScreen();
            Session.WritePrompt();
            return true;
        }

        private void ClearSkills()
        {
            selectedSkills.Clear();
            RefreshScreen();
            Session.WritePrompt();
        }

        private void ViewSkillDescription(string skillName)
        {
            GameSkill foundSkill = GetSkill(skillName);
            if (foundSkill == null)
            {
                WrmChargenCommon.SendErrorMessage(Session, "That skill does not exist.");
                return;
            }

            var sb = new StringBuilder();
            sb.AppendAnsiLine("<%b%><%yellow%>=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=");
            sb.AppendAnsiLine($"Description for {foundSkill.Name}");
            sb.AppendAnsiLine("=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=<%n%>");
            sb.AppendAnsiLine($"<%b%><%white%>{foundSkill.Description}");
            sb.AppendAnsiLine("<%b%><%yellow%>=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=<%n%>");

            Session.Write(sb.ToString());
        }

        private void ProcessDone()
        {
            if (selectedSkills.Count() != 3)
            {
                WrmChargenCommon.SendErrorMessage(Session, "Please select all your starting skills before proceeding to the next step.");
                return;
            }

            // Assign the skills to the PlayerBehavior's parent, which should be a Thing object.
            var playerBehavior = Session.Thing.Behaviors.FindFirst<PlayerBehavior>();
            foreach (var gameSkill in selectedSkills)
            {
                playerBehavior.Parent.Skills.Add(gameSkill.Name, gameSkill);
            }

            // Proceed to the next step.
            StateMachine.HandleNextStep(this, StepStatus.Success);
        }

        private string FormatSkillText()
        {
            var skillQueue = new Queue();
            var text = new StringBuilder();
            int rows = gameSkills.Count / 4;
            var longestSkillName = 0;

            foreach (var gameSkill in gameSkills)
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
                text.AppendAnsiLine($"{skill1}  {skill2}  {skill3}  {skill4}");
            }

            if ((gameSkills.Count % 4) > 0)
            {
                int columns = gameSkills.Count - (rows * 4);
                switch (columns)
                {
                    case 1:
                        string skillcolumn1 = WrmChargenCommon.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                        text.AppendAnsiLine($"{skillcolumn1}");
                        break;
                    case 2:
                        string sk1 = WrmChargenCommon.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                        string sk2 = WrmChargenCommon.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                        text.AppendAnsiLine($"{sk1}  {sk2}");
                        break;
                    case 3:
                        string skl1 = WrmChargenCommon.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                        string skl2 = WrmChargenCommon.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                        string skl3 = WrmChargenCommon.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                        text.AppendAnsiLine($"{skl1}  {skl2}  {skl3}");
                        break;
                }
            }

            text.AppendAnsiLine();
            return text.ToString();
        }

        private void RefreshScreen(bool sendPrompt = true)
        {
            var nl = Environment.NewLine;
            var sb = new StringBuilder();
            sb.AppendAnsiLine();
            sb.AppendAnsiLine();
            sb.AppendAnsiLine("You may pick three starting skills for your character.");
            int n = 1;
            foreach (var skill in selectedSkills)
            {
                sb.AppendAnsiLine($"Skill #{n} : {skill.Name}{nl}");
                n++;
            }
            sb.AppendAnsiLine();
            sb.AppendAnsiLine("<%green%>Please select 3 from the list below:<%n%>");
            sb.AppendAnsiLine(formattedSkills);
            sb.AppendAnsiLine($"<%green%>You have {3 - selectedSkills.Count()} skills left.<%n%>");
            sb.AppendAnsiLine("<%yellow%>=========================================================================");
            sb.AppendAnsiLine("To pick a skill, type the skill's name. Example: unarmed");
            sb.AppendAnsiLine("To view a skill's description use the view command. Example: view unarmed");
            sb.AppendAnsiLine("To see this screen again type list.");
            sb.AppendAnsiLine("When you are done picking your three skills, type done.");
            sb.AppendAnsiLine("=========================================================================<%n%>");

            Session.Write(sb.ToString(), sendPrompt);
        }
    }
}
