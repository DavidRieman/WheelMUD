//-----------------------------------------------------------------------------
// <copyright file="SetAttributesState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage.CharacterCreation
{
    using System.Text;
    using System.Text.RegularExpressions;
    using WheelMUD.ConnectionStates;
    using WheelMUD.Core;

    /// <summary>Commands recognized by the Set Attributes character creation state.</summary>
    public enum SetAttributeCommand
    {
        /// <summary>Unknown / unrecognized attribute.</summary>
        Unknown,

        /// <summary>Adjust Warrior Attribute.</summary>
        Warrior,

        /// <summary>Adjust Rogue Attribute.</summary>
        Rogue,

        /// <summary>Adjust Mage Attribute.</summary>
        Mage,

        /// <summary>The user is done.</summary>
        Done,
    }

    /// <summary>The character creation step where the player will set their stats.</summary>
    public class SetAttributesState : CharacterCreationSubState
    {
        private static readonly string Prompt = "Select the character's starting attributes:<%nl%>";
        private static readonly int MaxPoints = 10;
        private int warriorPoints;
        private int roguePoints;
        private int magePoints;

        /// <summary>Initializes a new instance of the <see cref="SetAttributesState"/> class.</summary>
        /// <param name="session">The session.</param>
        public SetAttributesState(Session session)
            : base(session)
        {
            Session.Write("You will now set your basic attributes.<%nl%>", false);
            RefreshScreen(false);
        }

        /// <summary>Gets the total points spent so far by the character.</summary>
        private int SpentPoints => warriorPoints + roguePoints + magePoints;

        /// <summary>Processes the text that the player sends while in this state.</summary>
        /// <param name="s">The command that the player just sent.</param>
        public override void ProcessInput(string s)
        {
            var command = FindTargetCommand(s);
            switch (command)
            {
                case SetAttributeCommand.Warrior:
                    ProcessAttributeCommand(command, s, ref warriorPoints);
                    break;
                case SetAttributeCommand.Rogue:
                    ProcessAttributeCommand(command, s, ref roguePoints);
                    break;
                case SetAttributeCommand.Mage:
                    ProcessAttributeCommand(command, s, ref magePoints);
                    break;
                case SetAttributeCommand.Done:
                    if (SpentPoints != MaxPoints)
                    {
                        var sb = new StringBuilder();
                        sb.AppendAnsiSeparator(color:"red", design: "=");
                        sb.AppendAnsiLine("You have not spent all your points.");
                        sb.AppendAnsiSeparator(color:"red", design: "=");
                        Session.Write(sb.ToString());
                    }
                    else
                    {
                        // Proceed to the next step.
                        SetPlayerBehaviorAttributes();
                        StateMachine.HandleNextStep(this, StepStatus.Success);
                        return;
                    }
                    break;
                default:
                    var sb1 = new StringBuilder();
                    sb1.AppendAnsiSeparator(color:"red", design: "=");
                    sb1.AppendAnsiLine("Unknown command. Please use warrior, rogue, mage, or done.");
                    sb1.AppendAnsiSeparator(color:"red", design: "=");
                    Session.Write(sb1.ToString());
                    break;
            }

            RefreshScreen();
        }

        /// <summary>Builds the prompt.</summary>
        /// <returns>A prompt indicating how the player should proceed.</returns>
        public override string BuildPrompt()
        {
            return Prompt;
        }

        private void ProcessAttributeCommand(SetAttributeCommand command, string s, ref int targetPoints)
        {
            var op = FindOperation(s);
            var numberString = Regex.Match(s, @"\d+").Value;
            if (string.IsNullOrWhiteSpace(numberString))
            {
                var sb = new StringBuilder();
                sb.AppendAnsiSeparator(color:"red", design: "=");
                sb.AppendAnsiLine("No valid number was found.");
                sb.AppendAnsiSeparator(color:"red", design: "=");
                Session.Write(sb.ToString());
                return;
            }

            int n;
            if (!int.TryParse(numberString, out n))
            {
                var sb = new StringBuilder();
                sb.AppendAnsiSeparator(color:"red", design: "=");
                sb.AppendAnsiLine("Could not process the number.");
                sb.AppendAnsiSeparator(color:"red", design: "=");
                Session.Write(sb.ToString());
                return;
            }

            if (string.IsNullOrWhiteSpace(op))
            {
                // No operator? Try to set the value directly to the number provided.
                SetAttributeTarget(ref targetPoints, n);
            }
            else if (op == "+")
            {
                SetAttributeTarget(ref targetPoints, targetPoints + n);
            }
            else if (op == "-")
            {
                SetAttributeTarget(ref targetPoints, targetPoints - n);
            }
        }

        private void SetAttributeTarget(ref int targetPoints, int newValue)
        {
            int netChange = newValue - targetPoints;
            if (newValue > 6)
            {
                var sb = new StringBuilder();
                sb.AppendAnsiSeparator(color:"red", design: "=");
                sb.AppendAnsiLine("No attribute can be greater than 6.");
                sb.AppendAnsiSeparator(color:"red", design: "=");
                Session.Write(sb.ToString());
            }
            else if (newValue < 0)
            {
                var sb = new StringBuilder();
                sb.AppendAnsiSeparator(color:"red", design: "=");
                sb.AppendAnsiLine("No attribute can be less than 0.");
                sb.AppendAnsiSeparator(color:"red", design: "=");
                Session.Write(sb.ToString());
            }
            else if (SpentPoints + netChange > MaxPoints)
            {
                var sb = new StringBuilder();
                sb.AppendAnsiSeparator(color:"red", design: "=");
                sb.AppendAnsiLine("You do not have enough points to spend.");
                sb.AppendAnsiSeparator(color:"red", design: "=");
                Session.Write(sb.ToString());
            }
            else
            {
                targetPoints = newValue;
            }
        }

        private void RefreshScreen(bool sendPrompt = true)
        {
            var sb = new StringBuilder();
            sb.AppendAnsiLine();
            sb.AppendAnsiLine("You have 10 character points to be divided between 3 attributes.");
            sb.AppendAnsiLine("No attribute can have more than 6 points. Attributes can be zero.");
            sb.AppendAnsiLine();
            sb.AppendAnsiLine($"Warrior : {warriorPoints}");
            sb.AppendAnsiLine($"Rogue   : {roguePoints}");
            sb.AppendAnsiLine($"Mage    : {magePoints}");
            sb.AppendAnsiLine();
            sb.AppendAnsiLine($"You have {MaxPoints - SpentPoints} character points left.");
            sb.AppendAnsiLine();
            sb.AppendAnsiSeparator();
            sb.AppendAnsiLine("To add points to an attribute, use the + operator. Example: warrior +6");
            sb.AppendAnsiLine("To subtract points from an attribute, use the - operator. Example: warrior -6");
            sb.AppendAnsiLine("When you are done distributing the character points, type done.");
            sb.AppendAnsiSeparator();

            Session.Write(sb.ToString(), sendPrompt);
        }

        private SetAttributeCommand FindTargetCommand(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return SetAttributeCommand.Unknown;
            }

            s = s.ToLower().Trim();
            if (s.StartsWith("w"))
            {
                return SetAttributeCommand.Warrior;
            }

            if (s.StartsWith("r"))
            {
                return SetAttributeCommand.Rogue;
            }

            if (s.StartsWith("m"))
            {
                return SetAttributeCommand.Mage;
            }

            if (s.StartsWith("done") || s.StartsWith("end") || s.StartsWith("quit"))
            {
                return SetAttributeCommand.Done;
            }

            return SetAttributeCommand.Unknown;
        }

        private string FindOperation(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }

            if (s.Contains("+"))
            {
                return "+";
            }

            if (s.Contains("-"))
            {
                return "-";
            }

            return string.Empty;
        }

        private void SetPlayerBehaviorAttributes()
        {
            var playerBehavior = Session.Thing.Behaviors.FindFirst<PlayerBehavior>();
            var attributes = playerBehavior.Parent.Attributes;
            var character = Session.Thing;

            attributes["WAR"].SetValue(warriorPoints, character);
            attributes["ROG"].SetValue(roguePoints, character);
            attributes["MAG"].SetValue(magePoints, character);
        }
    }
}
