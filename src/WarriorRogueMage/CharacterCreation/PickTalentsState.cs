//-----------------------------------------------------------------------------
// <copyright file="PickTalentsState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.ConnectionStates;
using WheelMUD.Core;
using WheelMUD.Server;

namespace WarriorRogueMage.CharacterCreation
{
    /// <summary>The character creation step where the player will pick their talents.</summary>
    public class PickTalentsState : CharacterCreationSubState
    {
        private static readonly OutputBuilder prompt = new OutputBuilder().Append("Select the character's starting talent: > ");
        private int longestTalentName;
        private readonly List<Talent> talents;

        /// <summary>Initializes a new instance of the <see cref="PickTalentsState"/> class.</summary>
        /// <param name="session">The session.</param>
        public PickTalentsState(Session session)
            : base(session)
        {
            talents = TalentFinder.Instance.NormalTalents;
        }

        public override void Begin()
        {
            var output = new OutputBuilder();
            output.AppendLine("You will now pick your character's starting talent.");
            Session.Write(output, false);
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
                        var talentName = string.Join(" ", commandParts, 1, commandParts.Length - 1);
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

        public override OutputBuilder BuildPrompt()
        {
            return prompt;
        }

        private Talent GetTalent(string talentName)
        {
            return WrmChargenCommon.GetFirstPriorityMatch(talentName, talents);
        }

        private void ViewTalentDescription(string talent)
        {
            var foundTalent = GetTalent(talent);
            if (foundTalent != null)
            {
                var output = new OutputBuilder();
                output.AppendSeparator('=', "yellow", true);
                output.AppendLine($"Description for {foundTalent.Name}");
                output.AppendSeparator('-', "yellow");
                output.AppendLine($"<%b%><%white%>{foundTalent.Description}");
                output.AppendSeparator('=', "yellow", true);

                Session.Write(output);
            }
            else
            {
                WrmChargenCommon.SendErrorMessage(Session, "That talent does not exist.");
            }
        }

        private void FormatTalentText(OutputBuilder outputBuilder)
        {
            var talentQueue = new Queue<Talent>();

            foreach (var gameTalent in talents)
            {
                // Find out what talent name is the longest
                if (gameTalent.Name.Length > longestTalentName)
                {
                    longestTalentName = gameTalent.Name.Length;
                }

                talentQueue.Enqueue(gameTalent);
            }

            var rows = talents.Count / 4;

            try
            {
                for (var i = 0; i < rows; i++)
                {
                    var talent1 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                    var talent2 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                    var talent3 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                    var talent4 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                    outputBuilder.AppendLine($"{talent1}  {talent2}  {talent3}  {talent4}");
                }

                if ((rows % 4) > 0)
                {
                    var columns = rows - (rows * 4);

                    switch (columns)
                    {
                        case 1:
                            var talentcolumn1 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                            outputBuilder.AppendLine($"{talentcolumn1}");
                            break;
                        case 2:
                            var tk1 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                            var tk2 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                            outputBuilder.AppendLine($"{tk1}  {tk2}");
                            break;
                        case 3:
                            var tkl1 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                            var tkl2 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                            var tkl3 = WrmChargenCommon.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                            outputBuilder.AppendLine($"{tkl1}  {tkl2}  {tkl3}");
                            break;
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        private void RefreshScreen()
        {
            var output = new OutputBuilder();
            output.AppendLine();
            output.AppendLine("You may pick one starting talent for your character.");
            output.AppendLine("<%green%>Please select 1 from the list below:<%n%>");
            FormatTalentText(output);
            output.AppendSeparator('=', "yellow");
            output.AppendLine("To pick a talent, type the talent's name. Example: sailor");
            output.AppendLine("To view a talent's description use the view command. Example: view sailor");
            output.AppendLine("To see this screen again type list.");
            output.AppendSeparator('=', "yellow");
            Session.Write(output);
        }
    }
}