//-----------------------------------------------------------------------------
// <copyright file="SetAttributesState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Character creation state used to set the initial values for the player's
//   base attributes.
//   Author: Fastalanasa
//   Date: May 6, 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage.CharacterCreation
{
    using System;
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
        private static readonly string Prompt = string.Format("Select the character's starting attributes.{0}> ", Environment.NewLine);
        private static readonly int MaxPoints = 10;
        private int warriorPoints;
        private int roguePoints;
        private int magePoints;

        /// <summary>Initializes a new instance of the <see cref="SetAttributesState"/> class.</summary>
        /// <param name="session">The session.</param>
        public SetAttributesState(Session session)
            : base(session)
        {
            this.Session.Write("You will now set your basic attributes.\n\n", false);
            this.RefreshScreen(false);
        }

        /// <summary>Gets the total points spent so far by the character.</summary>
        private int SpentPoints
        {
            get { return this.warriorPoints + this.roguePoints + this.magePoints; }
        }

        /// <summary>Processes the text that the player sends while in this state.</summary>
        /// <param name="s">The command that the player just sent.</param>
        public override void ProcessInput(string s)
        {
            var command = this.FindTargetCommand(s);
            switch (command)
            {
                case SetAttributeCommand.Warrior:
                    this.ProcessAttributeCommand(command, s, ref this.warriorPoints);
                    break;
                case SetAttributeCommand.Rogue:
                    this.ProcessAttributeCommand(command, s, ref this.roguePoints);
                    break;
                case SetAttributeCommand.Mage:
                    this.ProcessAttributeCommand(command, s, ref this.magePoints);
                    break;
                case SetAttributeCommand.Done:
                    if (this.SpentPoints != MaxPoints)
                    {
                        WrmChargenCommon.SendErrorMessage(this.Session, "You have not spent all your points.");
                    }
                    else
                    {
                        // Proceed to the next step.
                        this.SetPlayerBehaviorAttributes();
                        this.StateMachine.HandleNextStep(this, StepStatus.Success);
                        return;
                    }

                    break;
                default:
                    WrmChargenCommon.SendErrorMessage(this.Session, "Unknown command. Please use warrior, rogue, mage, or done.");
                    break;
            }

            this.RefreshScreen();
        }

        /// <summary>Builds the prompt.</summary>
        /// <returns>A prompt indicating how the player should proceed.</returns>
        public override string BuildPrompt()
        {
            return Prompt;
        }

        private void ProcessAttributeCommand(SetAttributeCommand command, string s, ref int targetPoints)
        {
            var op = this.FindOperation(s);
            var numberString = Regex.Match(s, @"\d+").Value;
            if (string.IsNullOrWhiteSpace(numberString))
            {
                WrmChargenCommon.SendErrorMessage(this.Session, "No valid number was found.");
                return;
            }

            int n;
            if (!int.TryParse(numberString, out n))
            {
                WrmChargenCommon.SendErrorMessage(this.Session, "Could not process the number.");
                return;
            }

            if (string.IsNullOrWhiteSpace(op))
            {
                // No operator? Try to set the value directly to the number provided.
                this.SetAttributeTarget(ref targetPoints, n);
            }
            else if (op == "+")
            {
                this.SetAttributeTarget(ref targetPoints, targetPoints + n);
            }
            else if (op == "-")
            {
                this.SetAttributeTarget(ref targetPoints, targetPoints - n);
            }
        }

        private void SetAttributeTarget(ref int targetPoints, int newValue)
        {
            int netChange = newValue - targetPoints;
            if (newValue > 6)
            {
                WrmChargenCommon.SendErrorMessage(this.Session, "No attribute can be greater than 6.");
            }
            else if (newValue < 0)
            {
                WrmChargenCommon.SendErrorMessage(this.Session, "No attribute can be less than 0.");
            }
            else if (this.SpentPoints + netChange > MaxPoints)
            {
                WrmChargenCommon.SendErrorMessage(this.Session, "You do not have enough points to spend.");
            }
            else
            {
                targetPoints = newValue;
            }
        }

        private void RefreshScreen(bool sendPrompt = true)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("You have 10 character points to be divided between 3 attributes.");
            sb.AppendLine("No attribute can have more than 6 points. Attributes can be zero.");
            sb.AppendLine();
            sb.AppendLine(string.Format("Warrior : {0}", this.warriorPoints));
            sb.AppendLine(string.Format("Rogue   : {0}", this.roguePoints));
            sb.AppendLine(string.Format("Mage    : {0}", this.magePoints));
            sb.AppendLine();
            sb.AppendFormat("You have {0} character points left.", MaxPoints - this.SpentPoints);
            sb.AppendLine();
            sb.AppendLine("<%yellow%>====================================================================");
            sb.AppendLine("To add points to an attribute, use the + operator. Example: warrior +6");
            sb.AppendLine("To subtract points from an attribute, use the - operator. Example: warrior -6");
            sb.AppendLine("When you are done distributing the character points, type done.");
            sb.AppendLine("====================================================================<%n%>");

            this.Session.Write(sb.ToString(), sendPrompt);
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
            else if (s.StartsWith("r"))
            {
                return SetAttributeCommand.Rogue;
            }
            else if (s.StartsWith("m"))
            {
                return SetAttributeCommand.Mage;
            }
            else if (s.StartsWith("done") || s.StartsWith("end") || s.StartsWith("quit"))
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
            else if (s.Contains("+"))
            {
                return "+";
            }
            else if (s.Contains("-"))
            {
                return "-";
            }

            return string.Empty;
        }

        private void SetPlayerBehaviorAttributes()
        {
            var playerBehavior = this.StateMachine.NewCharacter.Behaviors.FindFirst<PlayerBehavior>();
            var attributes = playerBehavior.Parent.Attributes;
            var character = this.Session.Thing;

            attributes["WAR"].SetValue(this.warriorPoints, character);
            attributes["ROG"].SetValue(this.roguePoints, character);
            attributes["MAG"].SetValue(this.magePoints, character);
        }
    }
}