//-----------------------------------------------------------------------------
// <copyright file="SetAttributesState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Text.RegularExpressions;
using WheelMUD.ConnectionStates;
using WheelMUD.Core;
using WheelMUD.Server;

namespace WarriorRogueMage.CharacterCreation
{
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
        private static readonly OutputBuilder prompt = new OutputBuilder().Append("Select the character's starting attributes: > ");
        private static readonly int MaxPoints = 10;
        private int warriorPoints;
        private int roguePoints;
        private int magePoints;

        /// <summary>Initializes a new instance of the <see cref="SetAttributesState"/> class.</summary>
        /// <param name="session">The session.</param>
        public SetAttributesState(Session session)
            : base(session)
        {
        }

        public override void Begin()
        {
            Session.Write(new OutputBuilder().AppendLine("You will now set your basic attributes."), false);
            RefreshScreen();
        }

        /// <summary>Gets the total points spent so far by the character.</summary>
        private int SpentPoints
        {
            get { return warriorPoints + roguePoints + magePoints; }
        }

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
                        WrmChargenCommon.SendErrorMessage(Session, "You have not spent all your points.");
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
                    WrmChargenCommon.SendErrorMessage(Session, "Unknown command. Please use warrior, rogue, mage, or done.");
                    break;
            }

            RefreshScreen();
        }

        /// <summary>Builds the prompt.</summary>
        /// <returns>A prompt indicating how the player should proceed.</returns>
        public override OutputBuilder BuildPrompt()
        {
            return prompt;
        }

        private void ProcessAttributeCommand(SetAttributeCommand command, string s, ref int targetPoints)
        {
            var op = FindOperation(s);
            var numberString = Regex.Match(s, @"\d+").Value;
            if (string.IsNullOrWhiteSpace(numberString))
            {
                WrmChargenCommon.SendErrorMessage(Session, "No valid number was found.");
                return;
            }

            int n;
            if (!int.TryParse(numberString, out n))
            {
                WrmChargenCommon.SendErrorMessage(Session, "Could not process the number.");
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
                WrmChargenCommon.SendErrorMessage(Session, "No attribute can be greater than 6.");
            }
            else if (newValue < 0)
            {
                WrmChargenCommon.SendErrorMessage(Session, "No attribute can be less than 0.");
            }
            else if (SpentPoints + netChange > MaxPoints)
            {
                WrmChargenCommon.SendErrorMessage(Session, "You do not have enough points to spend.");
            }
            else
            {
                targetPoints = newValue;
            }
        }

        private void RefreshScreen()
        {
            var output = new OutputBuilder();
            output.AppendLine();
            output.AppendLine("You have 10 character points to be divided between 3 attributes.");
            output.AppendLine("No attribute can have more than 6 points. Attributes can be zero.");
            output.AppendLine();
            output.AppendLine($"Warrior : {warriorPoints}");
            output.AppendLine($"Rogue   : {roguePoints}");
            output.AppendLine($"Mage    : {magePoints}");
            output.AppendLine();
            output.AppendLine($"You have {MaxPoints - SpentPoints} character points left.");
            output.AppendLine();
            output.AppendLine("<%yellow%>====================================================================");
            output.AppendLine("To add points to an attribute, use the + operator. Example: warrior +6");
            output.AppendLine("To subtract points from an attribute, use the - operator. Example: warrior -6");
            output.AppendLine("When you are done distributing the character points, type done.");
            output.AppendLine("====================================================================<%n%>");
            Session.Write(output);
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
            var playerBehavior = Session.Thing.FindBehavior<PlayerBehavior>();
            var attributes = playerBehavior.Parent.Attributes;
            var character = Session.Thing;

            attributes["WAR"].SetValue(warriorPoints, character);
            attributes["ROG"].SetValue(roguePoints, character);
            attributes["MAG"].SetValue(magePoints, character);
        }
    }
}
