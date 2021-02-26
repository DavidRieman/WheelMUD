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
            Session.Write($"You will now pick your character's starting skills.<%nl%>");
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
                        var sb = new StringBuilder();
                        sb.AppendAnsiSeparator(color:"red", design: "=");
                        sb.AppendAnsiLine("Please select which skill you would like to view details for, like 'view axes'.");
                        sb.AppendAnsiSeparator(color:"red", design: "=");
                        Session.Write(sb.ToString());
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
                var sb = new StringBuilder();
                sb.AppendAnsiSeparator(color:"red", design: "=");
                sb.AppendAnsiLine("That skill does not exist.");
                sb.AppendAnsiSeparator(color:"red", design: "=");
                Session.Write(sb.ToString());
                return false;
            }

            if (selectedSkills.Contains(selectedSkill))
            {
                var sb = new StringBuilder();
                sb.AppendAnsiSeparator(color:"red", design: "=");
                sb.AppendAnsiLine("You have already selected that skill.");
                sb.AppendAnsiSeparator(color:"red", design: "=");
                Session.Write(sb.ToString());
                return false;
            }

            if (selectedSkills.Count() >= 3)
            {
                var sb = new StringBuilder();
                sb.AppendAnsiSeparator(color:"red", design: "=");
                sb.AppendAnsiLine("You have already selected all 3 skills.");
                sb.AppendAnsiSeparator(color:"red", design: "=");
                Session.Write(sb.ToString());
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
            var sb = new StringBuilder();
            
            if (foundSkill == null)
            {
                sb.AppendAnsiSeparator(color:"red", design: "=");
                sb.AppendAnsiLine("That skill does not exist.");
                sb.AppendAnsiSeparator(color:"red", design: "=");
                Session.Write(sb.ToString());
                return;
            }

            sb.AppendAnsiSeparator(design:"=+");
            sb.AppendAnsiLine($"Description for {foundSkill.Name}");
            sb.AppendAnsiSeparator(design:"-");
            sb.AppendAnsiLine($"<%b%><%white%>{foundSkill.Description}");
            sb.AppendAnsiSeparator(design:"=+");

            Session.Write(sb.ToString());
        }

        private void ProcessDone()
        {
            if (selectedSkills.Count() != 3)
            {
                var sb = new StringBuilder();
                sb.AppendAnsiSeparator(color:"red", design: "=");
                sb.AppendAnsiLine("Please select all your starting skills before proceeding to the next step.");
                sb.AppendAnsiSeparator(color:"red", design: "=");
                Session.Write(sb.ToString());
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
                string skill1 = AnsiStringUtilities.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                string skill2 = AnsiStringUtilities.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                string skill3 = AnsiStringUtilities.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                string skill4 = AnsiStringUtilities.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                text.AppendAnsiLine($"{skill1}  {skill2}  {skill3}  {skill4}");
            }

            if ((gameSkills.Count % 4) > 0)
            {
                int columns = gameSkills.Count - (rows * 4);
                switch (columns)
                {
                    case 1:
                        string skillcolumn1 = AnsiStringUtilities.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                        text.AppendAnsiLine($"{skillcolumn1}");
                        break;
                    case 2:
                        string sk1 = AnsiStringUtilities.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                        string sk2 = AnsiStringUtilities.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                        text.AppendAnsiLine($"{sk1}  {sk2}");
                        break;
                    case 3:
                        string skl1 = AnsiStringUtilities.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                        string skl2 = AnsiStringUtilities.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                        string skl3 = AnsiStringUtilities.FormatToColumn(longestSkillName, ((GameSkill)skillQueue.Dequeue()).Name);
                        text.AppendAnsiLine($"{skl1}  {skl2}  {skl3}");
                        break;
                }
            }

            text.AppendAnsiLine();
            return text.ToString();
        }

        private void RefreshScreen(bool sendPrompt = true)
        {
            var sb = new StringBuilder();
            sb.AppendAnsiLine();
            sb.AppendAnsiLine("You may pick three starting skills for your character.");
            int n = 1;
            foreach (var skill in selectedSkills)
            {
                sb.AppendAnsiLine($"Skill #{n} : {skill.Name}");
                n++;
            }
            sb.AppendAnsiLine();
            sb.AppendAnsiLine("<%green%>Please select 3 from the list below:<%n%>");
            sb.AppendAnsiLine(formattedSkills);
            sb.AppendAnsiLine($"<%green%>You have {3 - selectedSkills.Count()} skills left.<%n%>");
            sb.AppendAnsiSeparator();
            sb.AppendAnsiLine("To pick a skill, type the skill's name. Example: unarmed");
            sb.AppendAnsiLine("To view a skill's description use the view command. Example: view unarmed");
            sb.AppendAnsiLine("To see this screen again type list.");
            sb.AppendAnsiLine("When you are done picking your three skills, type done.");
            sb.AppendAnsiSeparator();

            Session.Write(sb.ToString(), sendPrompt);
        }
    }
}
