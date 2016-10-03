//-----------------------------------------------------------------------------
// <copyright file="Finger.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Finger individual player or npc ?.
//   Created: January 2007 by Saquivor.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Threading;

    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Utilities;

    /// <summary>A command that gathers various information about an entity.</summary>
    [ExportGameAction]
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

            PlayerBehavior playerBehavior = this.target.Behaviors.FindFirst<PlayerBehavior>();
            if (playerBehavior != null)
            {
                // @@@ TODO: Mine this data in a less invasive/dangerous way; maybe the PlayerBehavior
                //     gets properties assigned for their last IP address upon connecting, etc..
                ////string sessionId = playerBehavior.SessionId;
                ////IConnection connection = bridge.ServerManager.GetConnection(sessionId);
                ////if (connection != null)
                ////{
                ////    isOnline = true;
                ////    addressIP = connection.CurrentIPAddress;
                ////}

                sb.AppendLine("<%yellow%><%b%>Name: " + this.target.Name + " Title: " + this.target.Title + "<%n%>");
                sb.AppendLine("Description: " + this.target.Description);
                sb.AppendLine("Full Name: " + this.target.FullName);
                sb.AppendLine("ID: " + this.target.ID);
                sb.AppendLine("Type: " + this.target.GetType().Name); // identify npc ?
                sb.AppendLine("Race: TBA      Guild: TBA     Class: TBA");
                sb.AppendLine("Religion: TBA");
                sb.AppendLine("Gender: TBA");
                sb.AppendLine("Email: TBA         Messenger Id: TBA");
                sb.AppendLine("Home Page: TBA");
                sb.AppendLine("Spouse: TBA");
                sb.AppendLine("Creation Date: TBA");
                sb.AppendLine("Age: TBA");

                if (isOnline)
                {
                    sb.AppendLine("Status: <%green%>Online<%n%>"); // need way to report both offline and online
                    sb.AppendLine("Location: " + this.target.Parent.Name);
                    sb.AppendLine(string.Format("Last Login: {0}", playerBehavior.PlayerData.LastLogin));
                    sb.AppendLine(string.Format("Current IP Address: {0}", addressIP));
                }
                else
                {
                    sb.AppendLine("Status: <%red%>Offline<%n%>"); // need way to report both offline and online
                    sb.AppendLine(string.Format("Last Login: {0}", playerBehavior.PlayerData.LastLogin));
                }

                sb.AppendLine("Plan: TBA");
                sb.AppendLine("MXP: TBA");
            }
            else
            {
                sb.AppendLine("<%yellow%><%b%>Name: " + this.target.Name + " Title: " + this.target.Title + "<%n%>");
                sb.AppendLine("Description: " + this.target.Description);
                sb.AppendLine("Full Name: " + this.target.FullName);
                sb.AppendLine("ID: " + this.target.ID);
                sb.AppendLine("Type: " + this.target.GetType().Name); // identify npc ?
                sb.AppendLine("Race: TBA      Guild: TBA     Class: TBA");
                sb.AppendLine("Religion: TBA");
                sb.AppendLine("Gender: TBA");
                sb.AppendLine("Email: TBA         Messenger Id: TBA");
                sb.AppendLine("Home Page: TBA");
                sb.AppendLine("Spouse: TBA");
                sb.AppendLine("Creation Date: TBA");
                sb.AppendLine("Age: TBA");
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
            this.target = GameAction.GetPlayerOrMobile(targetName);
            if (this.target == null)
            {
                // Now we need to look for the user in the database.
                // @@@ What if the player is offline? Player.Load probably adds them to the world, etc...
                //     This seems quite bad.  TEST!  Ideally we should be able to get the info of an off-
                //     line player BUT if the player logs in between say, our Guards and Execute getting
                //     run, we still want nothing strange to result, we don't want this to generate any
                //     'player logged in' events (as monitored by Friends system), etc...  Probably need
                //     some sort of ghosting system for info-gathering, should be generic for all things?
                //     Maybe the players all have a 'template' that can be loaded independently and gets
                //     saved with the player instance saving.
                //this.target = PlayerBehavior.Load(targetName);
                if (this.target == null)
                {
                    return targetName + " has never visited " + MudEngineAttributes.Instance.MudName + ".";
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