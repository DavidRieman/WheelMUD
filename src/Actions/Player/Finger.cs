//-----------------------------------------------------------------------------
// <copyright file="Finger.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using WheelMUD.Core;
using WheelMUD.Server;
using WheelMUD.Utilities;

namespace WheelMUD.Actions
{
    /// <summary>A command that gathers various information about an entity.</summary>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("finger", CommandCategory.Player)]
    [ActionAlias("whois", CommandCategory.Player)]
    [ActionAlias("who is", CommandCategory.Player)]
    [ActionDescription("Get information about an online or offline player.")]
    [ActionSecurity(SecurityRole.player)]
    public class Finger : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.RequiresAtLeastOneArgument
        };

        /// <summary>The target entity that we wish to get information for.</summary>
        private Thing target;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <remarks>
        /// TODO: will not work players offline at present ? where as tell should do this ?
        /// </remarks>
        public override void Execute(ActionInput actionInput)
        {
            var session = actionInput.Session;
            if (session == null) return; // This action only makes sense for player sessions.

            var output = new OutputBuilder();

            var isOnline = false;
            string addressIP = null;

            var playerBehavior = target.FindBehavior<PlayerBehavior>();

            output.AppendLine($"<%yellow%><%b%>Name: {target.Name} Title: {target.Title}<%n%>");
            output.AppendLine($"Description: {target.Description}");
            output.AppendLine($"Full Name: {target.FullName}");
            output.AppendLine($"ID: {target.Id}");
            output.AppendLine($"Type: {target.GetType().Name}"); // identify npc ?
            output.AppendLine("Race: TBA      Guild: TBA     Class: TBA");
            output.AppendLine("Religion: TBA");
            output.AppendLine("Gender: TBA");
            output.AppendLine("Email: TBA         Messenger Id: TBA");
            output.AppendLine("Home Page: TBA");
            output.AppendLine("Spouse: TBA");
            output.AppendLine("Creation Date: TBA");
            output.AppendLine("Age: TBA");

            if (playerBehavior != null)
            {
                // TODO: Mine this data in a less invasive/dangerous way; maybe the PlayerBehavior
                //     gets properties assigned for their last IP address upon connecting, etc..
                ////string sessionId = playerBehavior.SessionId;
                ////IConnection connection = bridge.ServerManager.GetConnection(sessionId);
                ////if (connection != null)
                ////{
                ////    isOnline = true;
                ////    addressIP = connection.CurrentIPAddress;
                ////}

                var statusString = isOnline ? "<%green%>Online<%n%>" : "<%red%>Offline<%n%>";

                output.AppendLine($"Status: {statusString}"); // need way to report both offline and online
                output.AppendLine($"Location: {target.Parent.Name}");
                output.AppendLine($"Last Login: {playerBehavior.History.LastLogIn}");

                if (isOnline)
                    output.AppendLine($"Current IP Address: {addressIP}");

                output.AppendLine("Plan: TBA");
                output.AppendLine("MXP: TBA");
            }

            session.Write(output);
        }

        /// <summary>Prepare for, and determine if the command's prerequisites have been met.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            var commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var textInfo = cultureInfo.TextInfo;
            ////string targetName = command.Action.Tail.Trim().ToLower();
            var targetName = textInfo.ToTitleCase(actionInput.Tail.Trim().ToLower());

            // Rule: Is the target an entity?
            target = GetPlayerOrMobile(targetName);
            if (target == null)
            {
                // Now we need to look for the user in the database.
                // TODO: What if the player is offline? Player.Load probably adds them to the world, etc...
                //     This seems quite bad.  TEST!  Ideally we should be able to get the info of an off-
                //     line player BUT if the player logs in between say, our Guards and Execute getting
                //     run, we still want nothing strange to result, we don't want this to generate any
                //     'player logged in' events (as monitored by Friends system), etc...  Probably need
                //     some sort of ghosting system for info-gathering, should be generic for all things?
                //     Maybe the players all have a 'template' that can be loaded independently and gets
                //     saved with the player instance saving.
                //target = PlayerBehavior.Load(targetName);
                if (target == null)
                {
                    return $"{targetName} has never visited {GameConfiguration.Name}.";
                }
            }

            // Rule: If there is less than 1 parameter, show help.
            return actionInput.Params.Length != 1 ?
                "Syntax:\n<finger [entity]>\n\nRemarking line 31 in this file will help crash test the mud :)\n\nSee also who" : null;
        }
    }
}
