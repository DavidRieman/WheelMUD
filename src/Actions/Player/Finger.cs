//-----------------------------------------------------------------------------
// <copyright file="Finger.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Threading;
    using WheelMUD.Core;
    using WheelMUD.Utilities;

    /// <summary>A command that gathers various information about an entity.</summary>
    [ExportGameAction(0)]
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
        private Thing target = null;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <remarks>
        /// TODO: will not work players offline at present ? where as tell should do this ?
        /// </remarks>
        public override void Execute(ActionInput actionInput)
        {
            var sb = new StringBuilder();

            bool isOnline = false;
            string addressIP = null;

            PlayerBehavior playerBehavior = target.Behaviors.FindFirst<PlayerBehavior>();
            
            sb.AppendAnsiLine($"<%yellow%><%b%>Name: {target.Name} Title: {target.Title}<%n%>");
            sb.AppendAnsiLine($"Description: {target.Description}");
            sb.AppendAnsiLine($"Full Name: {target.FullName}");
            sb.AppendAnsiLine($"ID: {target.Id}");
            sb.AppendAnsiLine($"Type: {target.GetType().Name}"); // identify npc ?
            sb.AppendAnsiLine("Race: TBA      Guild: TBA     Class: TBA");
            sb.AppendAnsiLine("Religion: TBA");
            sb.AppendAnsiLine("Gender: TBA");
            sb.AppendAnsiLine("Email: TBA         Messenger Id: TBA");
            sb.AppendAnsiLine("Home Page: TBA");
            sb.AppendAnsiLine("Spouse: TBA");
            sb.AppendAnsiLine("Creation Date: TBA");
            sb.AppendAnsiLine("Age: TBA");
            
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
                
                string statusString = isOnline ? "<%green%>Online<%n%>" : "<%red%>Offline<%n%>";
                
                sb.AppendAnsiLine($"Status: {statusString}"); // need way to report both offline and online
                sb.AppendAnsiLine($"Location: {target.Parent.Name}");
                sb.AppendAnsiLine($"Last Login: {playerBehavior.PlayerData.LastLogin}");
                
                if(isOnline)
                    sb.AppendAnsiLine($"Current IP Address: {addressIP}");
                
                sb.AppendAnsiLine("Plan: TBA");
                sb.AppendAnsiLine("MXP: TBA");
                
            }

            actionInput.Controller.Write(sb.ToString().TrimEnd());
        }

        /// <summary>Prepare for, and determine if the command's prerequisites have been met.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            string commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            ////string targetName = command.Action.Tail.Trim().ToLower();
            string targetName = textInfo.ToTitleCase(actionInput.Tail.Trim().ToLower());

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
                    return targetName + " has never visited " + GameConfiguration.Name + ".";
                }
            }

            // Rule: If there is less than 1 parameter, show help.
            if (actionInput.Params.Length != 1)
            {
                return "Syntax:\n<finger [entity]>\n\nRemarking line 31 in this file will help crash test the mud :)\n\nSee also who";
            }

            return null;
        }
    }
}
