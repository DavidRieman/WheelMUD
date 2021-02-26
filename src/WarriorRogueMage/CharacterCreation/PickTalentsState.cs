//-----------------------------------------------------------------------------
// <copyright file="PickTalentsState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

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
            Session.Write("You will now pick your character's starting talent.");
            talents = TalentFinder.Instance.NormalTalents;
            formattedTalents = FormatTalentText();
            RefreshScreen(false);
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
                        var sb = new StringBuilder();
                        sb.AppendAnsiSeparator(color:"red", design: "=");
                        sb.AppendAnsiLine("Please select which talent you would like to view details for, like 'view sailor'.");
                        sb.AppendAnsiSeparator(color:"red", design: "=");
                        Session.Write(sb.ToString());
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
                        var sb = new StringBuilder();
                        sb.AppendAnsiSeparator(color:"red", design: "=");
                        sb.AppendAnsiLine("Invalid talent. Try again, or use 'view [talent]' or 'list'.");
                        sb.AppendAnsiSeparator(color:"red", design: "=");
                        Session.Write(sb.ToString());
                    }
                    break;
            }
        }

        public override string BuildPrompt()
        {
            return "Select the character's starting talent:<%nl%>";
        }

        private Talent GetTalent(string talentName)
        {
            return WrmChargenCommon.GetFirstPriorityMatch(talentName, talents);
        }

        private void ViewTalentDescription(string talent)
        {
            Talent foundTalent = GetTalent(talent);
            var sb = new StringBuilder();
            
            if (foundTalent != null)
            {
                sb.AppendAnsiSeparator(design:"=+");
                sb.AppendAnsiLine($"Description for {foundTalent.Name}");
                sb.AppendAnsiSeparator(design:"-");
                sb.AppendAnsiLine($"<%b%><%white%>{foundTalent.Description}");
                sb.AppendAnsiSeparator(design:"=+");
                
            }
            else
            {
                sb.AppendAnsiSeparator(color:"red", design: "=");
                sb.AppendAnsiLine("That talent does not exist.");
                sb.AppendAnsiSeparator(color:"red", design: "=");
            }
            
            Session.Write(sb.ToString());
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
                    string talent1 = AnsiStringUtilities.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                    string talent2 = AnsiStringUtilities.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                    string talent3 = AnsiStringUtilities.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                    string talent4 = AnsiStringUtilities.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                    text.AppendAnsiLine($"{talent1}  {talent2}  {talent3}  {talent4}");
                }

                if ((rows % 4) > 0)
                {
                    int columns = rows - (rows * 4);

                    switch (columns)
                    {
                        case 1:
                            string talentcolumn1 = AnsiStringUtilities.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                            text.AppendAnsiLine($"{talentcolumn1}");
                            break;
                        case 2:
                            string tk1 = AnsiStringUtilities.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                            string tk2 = AnsiStringUtilities.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                            text.AppendAnsiLine($"{tk1}  {tk2}");
                            break;
                        case 3:
                            string tkl1 = AnsiStringUtilities.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                            string tkl2 = AnsiStringUtilities.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                            string tkl3 = AnsiStringUtilities.FormatToColumn(longestTalentName, talentQueue.Dequeue().Name);
                            text.AppendAnsiLine("{tkl1}  {tkl2}  {tkl3}");
                            break;
                    }
                }
            }
            catch
            {
                // ignored
            }

            text.AppendAnsiLine();
            return text.ToString();
        }

        private void RefreshScreen(bool sendPrompt = true)
        {
            var sb = new StringBuilder();
            sb.AppendAnsiLine();
            sb.AppendAnsiLine("You may pick one starting talent for your character.");
            sb.AppendAnsiLine("<%green%>Please select 1 from the list below:<%n%>");
            sb.AppendAnsiLine(formattedTalents);
            sb.AppendAnsiSeparator();
            sb.AppendAnsiLine("To pick a talent, type the talent's name. Example: sailor");
            sb.AppendAnsiLine("To view a talent's description use the view command. Example: view sailor");
            sb.AppendAnsiLine("To see this screen again type list.");
            sb.AppendAnsiSeparator();
            Session.Write(sb.ToString(), sendPrompt);
        }
    }
}