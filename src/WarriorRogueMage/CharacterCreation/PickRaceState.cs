//-----------------------------------------------------------------------------
// <copyright file="PickRaceState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Utilities;

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
        private readonly List<GameRace> gameRaces;

        /// <summary>Initializes a new instance of the <see cref="PickRaceState"/> class.</summary>
        /// <param name="session">The session.</param>
        public PickRaceState(Session session)
            : base(session)
        {
            Session.Write("You will now pick your character's race.<%nl%>");
            gameRaces = GameSystemController.Instance.GameRaces;
            FormatRaceText();
            RefreshScreen(false);
        }

        /// <summary>ProcessInput is used to receive the user input during this state.</summary>
        /// <param name="command">The command text to be processed.</param>
        public override void ProcessInput(string command)
        {
            string currentCommand = command.ToLower().Trim();

            switch (currentCommand)
            {
                case "view":
                    ViewRaceDescription(currentCommand);
                    break;
                case "list":
                    RefreshScreen();
                    break;
                default:
                    var race = GetRace(currentCommand);
                    if (race != null)
                    {
                        var playerBehavior = Session.Thing.Behaviors.FindFirst<PlayerBehavior>();
                        playerBehavior.Race = race;
                        StateMachine.HandleNextStep(this, StepStatus.Success);
                    }
                    else
                    {
                        var sb = new StringBuilder();
                        sb.AppendAnsiSeparator(color:"red", design: "=");
                        sb.AppendAnsiLine("Invalid race. Try again, or use 'view [race]' or 'list'.");
                        sb.AppendAnsiSeparator(color:"red", design: "=");
                        Session.Write(sb.ToString());
                    }
                    break;
            }
        }

        public override string BuildPrompt()
        {
            return "Select your character's race.<%nl%>";
        }

        private GameRace GetRace(string raceName)
        {
            return WrmChargenCommon.GetFirstPriorityMatch(raceName, gameRaces);
        }

        private void ViewRaceDescription(string race)
        {
            string raceToView = race.Replace("view ", string.Empty);

            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            string properCaseRace = textInfo.ToTitleCase(raceToView);

            var foundRace = (from r in gameRaces
                             where r.Name == properCaseRace
                             select r).FirstOrDefault();

            var sb = new StringBuilder();
            
            if (foundRace != null)
            {
                sb.AppendAnsiSeparator(design:"=+");
                sb.AppendAnsiLine($"Description for {foundRace.Name}");
                sb.AppendAnsiSeparator(design:"-");
                sb.AppendAnsiLine($"<%b%><%white%>{foundRace.Description}");
                sb.AppendAnsiSeparator(design:"=+");
            }
            else
            {
                sb.AppendAnsiSeparator(color:"red", design: "=");
                sb.AppendAnsiLine("That race does not exist.");
                sb.AppendAnsiSeparator(color:"red", design: "=");
            }
            
            Session.Write(sb.ToString());
        }

        private void FormatRaceText()
        {
            var raceQueue = new Queue<GameRace>();
            var text = new StringBuilder();
            int rows = gameRaces.Count / 4;

            foreach (var gameRace in gameRaces)
            {
                // Find out what skill name is the longest
                if (gameRace.Name.Length > longestRaceName)
                {
                    longestRaceName = gameRace.Name.Length;
                }

                raceQueue.Enqueue(gameRace);
            }

            try
            {
                for (int i = 0; i < rows; i++)
                {
                    string race1 = AnsiStringUtilities.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                    string race2 = AnsiStringUtilities.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                    string race3 = AnsiStringUtilities.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                    string race4 = AnsiStringUtilities.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                    text.AppendAnsiLine($"{race1}  {race2}  {race3}  {race4}");
                }

                if ((gameRaces.Count % 4) > 0)
                {
                    int columns = gameRaces.Count - (rows * 4);

                    switch (columns)
                    {
                        case 1:
                            string racecolumn1 = AnsiStringUtilities.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                            text.AppendAnsiLine($"{racecolumn1}");
                            break;
                        case 2:
                            string rc = AnsiStringUtilities.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                            string rcc1 = AnsiStringUtilities.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                            text.AppendAnsiLine($"{rc}  {rcc1}");
                            break;
                        case 3:
                            string rc1 = AnsiStringUtilities.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                            string rc2 = AnsiStringUtilities.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                            string rc3 = AnsiStringUtilities.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                            text.AppendAnsiLine($"{rc1}  {rc2}  {rc3}");
                            break;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            text.AppendAnsiLine();

            formattedRaces = text.ToString();
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
            sb.AppendAnsiLine();
            sb.AppendAnsiLine("<%green%>Please select 1 from the list below:<%n%>");
            sb.AppendAnsiLine(formattedRaces);
            sb.AppendAnsiSeparator();
            sb.AppendAnsiLine("To pick a race, just type the race's name. Example: human");
            sb.AppendAnsiLine("To view a race's description use the view command. Example: view orc");
            sb.AppendAnsiLine("To see this screen again type 'list'.");
            sb.AppendAnsiSeparator();

            Session.Write(sb.ToString(), sendPrompt);
        }
    }
}