//-----------------------------------------------------------------------------
// <copyright file="Buffer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>An action to get or set your command prompt display.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("buffer", CommandCategory.Configure)]
    [ActionDescription("Sets the maximum number of lines to send before pausing.")]
    [ActionExample("Valid values are between 0 and 100, or 'auto'. 0 means never pause, and 'auto' will attempt to determine the screen size upon connecting.")]
    [ActionSecurity(SecurityRole.player)]
    public class Buffer : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAPlayer
        };

        /// <summary>Sender of the command, in the form of a Session object.</summary>
        private Session session;

        /// <summary>The new buffer length, if one was provided by the sender.</summary>
        /// <remarks>Defaults to 0, and can be 0 if parsing failed; be sure to check whether <see cref="parseSucceeded"/>.</remarks>
        private int parsedBufferLength;

        /// <summary>This will be true if the submitted buffer length was the string "auto" or was successfully parsed as an integer.</summary>
        private bool parseSucceeded;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var userControlledBehavior = actionInput.Actor.FindBehavior<UserControlledBehavior>();
            if (userControlledBehavior == null) return;

            // No arguments were provided. Just show the current buffer setting and exit.
            if (string.IsNullOrEmpty(actionInput.Tail))
            {
                ShowCurrentBuffer(userControlledBehavior);
                return;
            }

            // Set the value for the current session
            if (session.Connection != null)
            {
                session.Connection.PagingRowLimit = parsedBufferLength == -1 ? session.TerminalOptions.Height : parsedBufferLength;
            }

            userControlledBehavior.PagingRowLimit = parsedBufferLength;

            ShowCurrentBuffer(userControlledBehavior);
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            var commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            PreprocessInput(actionInput);

            if (!parseSucceeded || parsedBufferLength > 100 || parsedBufferLength < -1)
            {
                return "The screen buffer must be between 0 and 100 lines, or 'auto'.";
            }

            return null;
        }

        /// <summary>Pre-process the important bits of input and store in private fields.</summary>
        /// <remarks>This allows the input to be available for validation and the main execute method.</remarks>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        private void PreprocessInput(ActionInput actionInput)
        {
            session = actionInput.Session;
            if (session == null) return;

            // Parse and store the desired buffer length, if one was provided.
            var lengthText = actionInput.Tail.ToLower().Trim();
            if (string.IsNullOrEmpty(lengthText))
            {
                parseSucceeded = true;
            }
            else if (lengthText == "auto")
            {
                parsedBufferLength = -1;
                parseSucceeded = true;
            }
            else
            {
                parseSucceeded = TryParse(lengthText, out parsedBufferLength);
            }
        }

        /// <summary>
        /// Extracts the integer value contained in the specified string. Similar to
        /// the .NET Framework's TryParse methods, but also handles the case where
        /// the string contains "auto".
        /// </summary>
        /// <param name="str">The string to be parsed.</param>
        /// <param name="bufferLength">The resulting integer. This will potentially be set to 0 (a valid buffer length) if parsing fails, so the return value is important.</param>
        /// <returns>True if the string was parsed successfully; otherwise, false.</returns>
        private bool TryParse(string str, out int bufferLength)
        {
            str = str.ToLower().Trim(' ', '\t', '\'', '"');

            if (str == "auto")
            {
                bufferLength = -1;
                return true;
            }

            return int.TryParse(str, out bufferLength);
        }

        /// <summary>Displays the current buffer length to the user, handling the special case of "auto" instead of -1.</summary>
        private void ShowCurrentBuffer(UserControlledBehavior userControlledBehavior)
        {
            if (userControlledBehavior.PagingRowLimit == -1)
            {
                session.WriteLine($"Your screen buffer size is 'auto' (currently {session.TerminalOptions.Height} lines).");
            }
            else
            {
                session.WriteLine($"Your screen buffer is set to {userControlledBehavior.PagingRowLimit} lines.");
            }
        }
    }
}