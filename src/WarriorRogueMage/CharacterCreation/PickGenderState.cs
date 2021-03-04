//-----------------------------------------------------------------------------
// <copyright file="PickGenderState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Linq;
using WheelMUD.ConnectionStates;
using WheelMUD.Core;
using WheelMUD.Utilities;

namespace WarriorRogueMage.CharacterCreation
{
    /// <summary>The character creation step where the player will pick their gender.</summary>
    /// <remarks>TODO: https://github.com/DavidRieman/WheelMUD/issues/68 - Change order, and refine to selecting character pronoun set instead of gender.</remarks>
    public class PickGenderState : CharacterCreationSubState
    {
        private GameGender selectedGender;

        /// <summary>Initializes a new instance of the <see cref="PickGenderState"/> class.</summary>
        /// <param name="session">The session.</param>
        public PickGenderState(Session session)
            : base(session)
        {
        }

        public override void Begin()
        {
            RefreshScreen();
        }

        /// <summary>ProcessInput is used to receive the user input during this state.</summary>
        /// <param name="command">The command text to be processed.</param>
        public override void ProcessInput(string command)
        {
            if (!string.IsNullOrEmpty(command) && SetGender(command))
            {
                ProcessDone();
            }
            else if (!HandleCommand(command))
            {
                WrmChargenCommon.SendErrorMessage(Session, "Invalid command. Please select a gender.");
            }
        }

        public override string BuildPrompt()
        {
            return "Select the character's gender: > ";
        }

        private bool HandleCommand(string command)
        {
            // Special commands like "help <gender>" could be implemented here, to describe the gender
            // dynamically against the gender class properties, etc.
            return false;
        }

        private bool SetGender(string specifiedGender)
        {
            // Support strings of format "select m" and "m" by ignoring "select " from the input.
            string currentGender = specifiedGender.Replace("select ", string.Empty);

            selectedGender = (from g in GameSystemController.Instance.GameGenders
                              where g.Name.Equals(currentGender, StringComparison.CurrentCultureIgnoreCase) ||
                                    g.Abbreviation.Equals(currentGender, StringComparison.CurrentCultureIgnoreCase)
                              select g).FirstOrDefault();
            if (selectedGender == null)
            {
                WrmChargenCommon.SendErrorMessage(Session, $"'{currentGender}' is an invalid gender selection.");
                RefreshScreen();
            }

            return selectedGender != null;
        }

        private void ProcessDone()
        {
            var playerBehavior = Session.Thing.Behaviors.FindFirst<PlayerBehavior>();
            playerBehavior.Gender = selectedGender;

            var ab = new AnsiBuilder();
            ab.AppendLine($"The chosen gender is <%green%>{selectedGender.Name}<%n%>.");
            Session.Write(ab.ToString(), false);

            // Proceed to the next step.
            StateMachine.HandleNextStep(this, StepStatus.Success);
        }

        private void RefreshScreen()
        {
            var ab = new AnsiBuilder();
            ab.AppendLine();
            ab.AppendLine("You have the following gender choices:");
            foreach (var gender in GameSystemController.Instance.GameGenders)
            {
                ab.AppendLine(gender.Name);
            }
            ab.AppendLine();
            ab.AppendSeparator('-', "yellow");
            ab.AppendLine("Type your gender selection.");
            ab.AppendSeparator('-', "yellow");
            Session.Write(ab.ToString());
        }
    }
}