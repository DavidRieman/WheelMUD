//-----------------------------------------------------------------------------
// <copyright file="PickRaceState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using WheelMUD.ConnectionStates;
using WheelMUD.Core;
using WheelMUD.Utilities;

namespace WarriorRogueMage.CharacterCreation
{
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
            gameRaces = GameSystemController.Instance.GameRaces;
            FormatRaceText();
        }

        public override void Begin()
        {
            Session.WriteAnsiLine("You will now pick your character's race.", false);
            RefreshScreen();
        }

        /// <summary>ProcessInput is used to receive the user input during this state.</summary>
        /// <param name="command">The command text to be processed.</param>
        public override void ProcessInput(string command)
        {
            var currentCommand = command.ToLower().Trim();

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
                        WrmChargenCommon.SendErrorMessage(Session, "Invalid race. Try again, or use 'view [race]' or 'list'.");
                    }
                    break;
            }
        }

        public override string BuildPrompt()
        {
            return "Select your character's race: > ";
        }

        private GameRace GetRace(string raceName)
        {
            return WrmChargenCommon.GetFirstPriorityMatch(raceName, gameRaces);
        }

        private void ViewRaceDescription(string race)
        {
            var raceToView = race.Replace("view ", string.Empty);

            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var textInfo = cultureInfo.TextInfo;

            var properCaseRace = textInfo.ToTitleCase(raceToView);

            var foundRace = (from r in gameRaces
                             where r.Name == properCaseRace
                             select r).FirstOrDefault();

            if (foundRace != null)
            {
                var ab = new AnsiBuilder();
                ab.AppendSeparator('=', "yellow", true);
                ab.AppendLine($"Description for {foundRace.Name}");
                ab.AppendSeparator('-', "yellow");
                ab.AppendLine($"<%b%><%white%>{foundRace.Description}<%n%>");
                ab.AppendSeparator('=', "yellow", true);
                Session.Write(ab.ToString());
            }
            else
            {
                WrmChargenCommon.SendErrorMessage(Session, "That race does not exist.");
            }
        }

        private void FormatRaceText()
        {
            var raceQueue = new Queue<GameRace>();
            var text = new AnsiBuilder();
            var rows = gameRaces.Count / 4;

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
                for (var i = 0; i < rows; i++)
                {
                    var race1 = WrmChargenCommon.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                    var race2 = WrmChargenCommon.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                    var race3 = WrmChargenCommon.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                    var race4 = WrmChargenCommon.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                    text.AppendLine($"{race1}  {race2}  {race3}  {race4}");
                }

                if (gameRaces.Count % 4 > 0)
                {
                    var columns = gameRaces.Count - (rows * 4);

                    switch (columns)
                    {
                        case 1:
                            var racecolumn1 = WrmChargenCommon.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                            text.AppendLine($"{racecolumn1}");
                            break;
                        case 2:
                            var rc = WrmChargenCommon.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                            var rcc1 = WrmChargenCommon.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                            text.AppendLine($"{rc}  {rcc1}");
                            break;
                        case 3:
                            var rc1 = WrmChargenCommon.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                            var rc2 = WrmChargenCommon.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                            var rc3 = WrmChargenCommon.FormatToColumn(longestRaceName, raceQueue.Dequeue().Name);
                            text.AppendLine($"{rc1}  {rc2}  {rc3}");
                            break;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            formattedRaces = text.ToString();
        }

        private string GetCommardPart(string command)
        {
            var retval = string.Empty;

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

        private void RefreshScreen()
        {
            var ab = new AnsiBuilder();
            ab.AppendLine();
            ab.AppendLine("<%green%>Please select 1 from the list below:<%n%>");
            ab.AppendLine(formattedRaces);
            ab.AppendSeparator('=', "yellow");
            ab.AppendLine("To pick a race, just type the race's name. Example: human");
            ab.AppendLine("To view a races' description use the view command. Example: view orc");
            ab.AppendLine("To see this screen again type 'list'.");
            ab.AppendSeparator('=', "yellow");
            Session.Write(ab.ToString());
        }
    }
}