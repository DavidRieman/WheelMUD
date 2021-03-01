//-----------------------------------------------------------------------------
// <copyright file="PickTalentsState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using WheelMUD.ConnectionStates;
using WheelMUD.Core;
using WheelMUD.Utilities;

namespace WarriorRogueMage.CharacterCreation
{
    /// <summary>The character creation step where the player will pick their talents.</summary>
    public class PickTalentsState : CharacterCreationSubState
    {
        private int longestTalentName;
        private readonly string formattedTalents;
        private readonly List<Talent> talents;

        /// <summary>Initializes a new instance of the <see cref="PickTalentsState"/> class.</summary>
        /// <param name="session">The session.</param>
        public PickTalentsState(Session session)
            : base(session)
        {
            talents = TalentFinder.Instance.NormalTalents;
            formattedTalents = FormatTalentText();
        }

        public override void Begin()
        {
            Session.WriteAnsiLine("You will now pick your character's starting talent.", false);
            RefreshScreen();
        }

        /// <summary>ProcessInput is used to receive the user input during this state.</summary>
        /// <param name="command">The command text to be processed.</param>
        public override void ProcessInput(string command)
        {
            var commandParts = command.Split(' ');
            var currentCommand = commandParts[0];
            switch (currentCommand)
            {
                case "view":
                    if (commandParts.Length > 1)
                    {
                        string talentName = string.Join(" ", commandParts, 1, commandParts.Length - 1);
                        ViewTalentDescription(talentName);
                    }
                    else
                    {
                        WrmChargenCommon.SendErrorMessage(Session, "Please select which talent you would like to view details for, like 'view sailor'.");
                    }
                    break;
                case "list":
                    RefreshScreen();
                    break;
                default:
                    var talent = GetTalent(currentCommand);
                    if (talent != null)
                    {
                        // TODO: Save talent to a WRM-specific Behavior?
                        StateMachine.HandleNextStep(this, StepStatus.Success);
                    }
                    else
                    {
                        WrmChargenCommon.SendErrorMessage(Session, "Invalid talent. Try again, or use 'view [talent]' or 'list'.");
                    }
                    break;
            }
        }

        public override string BuildPrompt()
        {
            return "Select the character's starting talent: > ";
        }

        private Talent GetTalent(string talentName)
        {
            return WrmChargenCommon.GetFirstPriorityMatch(talentName, talents);
        }

        private void ViewTalentDescription(string talent)
        {
            Talent foundTalent = GetTalent(talent);
            if (foundTalent != null)
            {
                var sb = new StringBuilder();
                sb.Append("<%b%><%yellow%>=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=" + Environment.NewLine);
                sb.AppendFormat("Description for {0}" + Environment.NewLine, foundTalent.Name);
                sb.Append("=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=<%n%>" + Environment.NewLine);
                sb.Append("<%b%><%white%>" + foundTalent.Description + Environment.NewLine);
                sb.Append("<%b%><%yellow%>=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=<%n%>");

                Session.Write(sb.ToString());
            }
            else
            {
                WrmChargenCommon.SendErrorMessage(Session, "That talent does not exist.");
            }
        }

        private string FormatTalentText()
        {
            var talentQueue = new Queue<Talent>();
            var text = new StringBuilder();

            foreach (var gameTalent in talents)
            {
                // Find out what talent name is the longest
                if (gameTalent.Name.Length > longestTalentName)
                {
                    longestTalentName = gameTalent.Name.Length;
                }

                talentQueue.Enqueue(gameTalent);
            }

            int rows = talents.Count / 4;

            try
            {
                for (int i = 0; i < rows; i++)
                {
                    string talent1 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                    string talent2 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                    string talent3 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                    string talent4 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                    text.AppendFormat("{0}  {1}  {2}  {3}" + Environment.NewLine, talent1, talent2, talent3, talent4);
                }

                if ((rows % 4) > 0)
                {
                    int columns = rows - (rows * 4);

                    switch (columns)
                    {
                        case 1:
                            string talentcolumn1 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                            text.AppendFormat("{0}" + Environment.NewLine, talentcolumn1);
                            break;
                        case 2:
                            string tk1 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                            string tk2 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                            text.AppendFormat("{0}  {1}" + Environment.NewLine, tk1, tk2);
                            break;
                        case 3:
                            string tkl1 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                            string tkl2 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                            string tkl3 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                            text.AppendFormat("{0}  {1}  {2}" + Environment.NewLine, tkl1, tkl2, tkl3);
                            break;
                    }
                }
            }
            catch
            {
            }

            return text.ToString();
        }

        private void RefreshScreen(bool sendPrompt = true)
        {
            var sb = new StringBuilder();
            sb.AppendAnsiLine();
            sb.AppendAnsiLine("You may pick one starting talent for your character.");
            sb.AppendAnsiLine("<%green%>Please select 1 from the list below:<%n%>");
            sb.AppendAnsiLine(formattedTalents);
            sb.AppendAnsiLine("<%yellow%>==========================================================================");
            sb.AppendAnsiLine("To pick a talent, type the talent's name. Example: sailor");
            sb.AppendAnsiLine("To view a talent's description use the view command. Example: view sailor");
            sb.AppendAnsiLine("To see this screen again type list.");
            sb.AppendAnsiLine("==========================================================================<%n%>");
            Session.Write(sb.ToString(), sendPrompt);
        }
    }
}