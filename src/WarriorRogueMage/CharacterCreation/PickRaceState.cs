//-----------------------------------------------------------------------------
// <copyright file="PickRaceState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Character creation state used to set the player's race.
//   Author: Fastalanasa
//   Date: May 8, 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage.CharacterCreation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using WheelMUD.ConnectionStates;
    using WheelMUD.Core;

    /// <summary>The character creation step where the player will pick their race.</summary>
    public class PickRaceState : CharacterCreationSubState
    {
        private int longestRaceName;
        private string formattedRaces;
        private GameRace selectedRace;
        private List<GameRace> gameRaces;

        /// <summary>Initializes a new instance of the <see cref="PickRaceState"/> class.</summary>
        /// <param name="session">The session.</param>
        public PickRaceState(Session session)
            : base(session)
        {
            this.Session.Write("You will now pick your character's race.");
            this.gameRaces = GameSystemController.Instance.GameRaces;
            this.FormatRaceText();
            this.RefreshScreen(false);
        }

        /// <summary>ProcessInput is used to receive the user input during this state.</summary>
        /// <param name="command">The command text to be processed.</param>
        public override void ProcessInput(string command)
        {
            string currentCommand = command.ToLower().Trim();

            switch (currentCommand)
            {
                case "clear":
                    this.ClearRace();
                    break;
                case "view":
                    this.ViewRaceDescription(currentCommand);
                    break;
                case "done":
                    this.ProcessDone();
                    break;
                case "list":
                    this.RefreshScreen();
                    break;
                default:
                    string tentativeRaceName = this.GetCommardPart(currentCommand);

                    if (tentativeRaceName == string.Empty)
                    {
                        this.SetRace(currentCommand);
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
            return "Select your character's race.\n> ";
        }

        private void SetRace(string raceToSelect)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            string currentRace = textInfo.ToTitleCase(raceToSelect);

            GameRace currSelection = (from r in this.gameRaces
                                      where r.Name == currentRace
                                      select r).FirstOrDefault();

            if (currSelection != null)
            {
                this.selectedRace = currSelection;
            }
            else
            {
                WrmChargenCommon.SendErrorMessage(this.Session, "That race does not exist.");
            }

            this.RefreshScreen();
        }

        private void ClearRace()
        {
            this.RefreshScreen();
        }

        private void ViewRaceDescription(string race)
        {
            string raceToView = race.Replace("view ", string.Empty);

            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            string properCaseRace = textInfo.ToTitleCase(raceToView);

            var foundRace = (from r in this.gameRaces
                             where r.Name == properCaseRace
                             select r).FirstOrDefault();

            if (foundRace != null)
            {
                var sb = new StringBuilder();
                sb.Append("<%b%><%yellow%>=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=" + Environment.NewLine);
                sb.AppendFormat("Description for {0}" + Environment.NewLine, foundRace.Name);
                sb.Append("=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=<%n%>" + Environment.NewLine);
                sb.Append("<%b%><%white%>" + foundRace.Description + Environment.NewLine);
                sb.Append("<%b%><%yellow%>=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=<%n%>");

                this.Session.Write(sb.ToString());
            }
            else
            {
                WrmChargenCommon.SendErrorMessage(this.Session, "That race does not exist.");
            }
        }

        private void ProcessDone()
        {
            if (this.selectedRace != null)
            {
                var playerBehavior = this.StateMachine.NewCharacter.Behaviors.FindFirst<PlayerBehavior>();
                playerBehavior.Race = this.selectedRace;

                // Proceed to the next step.
                this.StateMachine.HandleNextStep(this, StepStatus.Success);
            }
            else
            {
                WrmChargenCommon.SendErrorMessage(this.Session, "You must select a valid race before continuing.");
            }
        }

        private void FormatRaceText()
        {
            var raceQueue = new Queue<GameRace>();
            var text = new StringBuilder();
            int rows = this.gameRaces.Count / 4;

            foreach (var gameRace in this.gameRaces)
            {
                // Find out what skill name is the longest
                if (gameRace.Name.Length > this.longestRaceName)
                {
                    this.longestRaceName = gameRace.Name.Length;
                }

                raceQueue.Enqueue(gameRace);
            }

            try
            {
                for (int i = 0; i < rows; i++)
                {
                    string race1 = WrmChargenCommon.FormatToColumn(this.longestRaceName, raceQueue.Dequeue().Name);
                    string race2 = WrmChargenCommon.FormatToColumn(this.longestRaceName, raceQueue.Dequeue().Name);
                    string race3 = WrmChargenCommon.FormatToColumn(this.longestRaceName, raceQueue.Dequeue().Name);
                    string race4 = WrmChargenCommon.FormatToColumn(this.longestRaceName, raceQueue.Dequeue().Name);
                    text.AppendFormat("{0}  {1}  {2}  {3}" + Environment.NewLine, race1, race2, race3, race4);
                }

                if ((this.gameRaces.Count % 4) > 0)
                {
                    int columns = this.gameRaces.Count - (rows * 4);

                    switch (columns)
                    {
                        case 1:
                            string racecolumn1 = WrmChargenCommon.FormatToColumn(this.longestRaceName, raceQueue.Dequeue().Name);
                            text.AppendFormat("{0}" + Environment.NewLine, racecolumn1);
                            break;
                        case 2:
                            string rc = WrmChargenCommon.FormatToColumn(this.longestRaceName, raceQueue.Dequeue().Name);
                            string rcc1 = WrmChargenCommon.FormatToColumn(this.longestRaceName, raceQueue.Dequeue().Name);
                            text.AppendFormat("{0}  {1}" + Environment.NewLine, rc, rcc1);
                            break;
                        case 3:
                            string rc1 = WrmChargenCommon.FormatToColumn(this.longestRaceName, raceQueue.Dequeue().Name);
                            string rc2 = WrmChargenCommon.FormatToColumn(this.longestRaceName, raceQueue.Dequeue().Name);
                            string rc3 = WrmChargenCommon.FormatToColumn(this.longestRaceName, raceQueue.Dequeue().Name);
                            text.AppendFormat("{0}  {1}  {2}" + Environment.NewLine, rc1, rc2, rc3);
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }

            text.Append(Environment.NewLine);

            this.formattedRaces = text.ToString();
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

        private void RefreshScreen(bool sendPrompt = true)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine();
            string raceName = this.selectedRace != null ? this.selectedRace.Name : "(unselected)";
            sb.AppendFormat("Your race is : {0}" + Environment.NewLine, raceName);
            sb.AppendLine();
            sb.AppendLine("<%green%>Please select 1 from the list below:<%n%>");
            sb.AppendLine(this.formattedRaces);
            sb.AppendLine("<%yellow%>=========================================================================");
            sb.AppendLine("To pick a race, just type the race's name. Example: human");
            sb.AppendLine("To view a races's description use the view command. Example: view orc");
            sb.AppendLine("To see this screen again type list.");
            sb.AppendLine("When you are done picking your race, type done.");
            sb.AppendLine("=========================================================================<%n%>");

            this.Session.Write(sb.ToString(), sendPrompt);
        }
    }
}