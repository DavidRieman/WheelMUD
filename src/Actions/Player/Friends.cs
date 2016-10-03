//-----------------------------------------------------------------------------
// <copyright file="Friends.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command to allow a player to manage their friends list.
//   Currently friends lists are not saved!
//   Created: May 2009 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>A command to manipulate a players friends list.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("friends", CommandCategory.Player)]
    [ActionAlias("friend", CommandCategory.Player)]
    [ActionDescription("Add or remove friends from your friends list.")]
    [ActionSecurity(SecurityRole.player)]
    public class Friends : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAPlayer,
        };

        private Thing player;
        private PlayerBehavior playerBehavior;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <remarks>Verify that the Guards pass first.</remarks>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            if ((actionInput.Params.Count() == 1 && actionInput.Params[0].ToLower() == "list") || actionInput.Params.Count() == 0)
            {
                if (this.playerBehavior.Friends.Count == 0)
                {
                    sender.Write("You currently have no friends listed.");
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Your Friends:");
                    foreach (string friendName in this.playerBehavior.Friends)
                    {
                        string status = PlayerManager.Instance.FindPlayerByName(friendName, false) == null ? "Offline" : "Online";
                        sb.AppendLine(string.Format("{0} [{1}]", friendName, status));
                    }

                    sender.Write(sb.ToString().TrimEnd());
                }

                return;
            }

            if (actionInput.Params.Count() != 2 &&
                actionInput.Params[0].ToLower() != "add" &&
                actionInput.Params[0].ToLower() != "remove")
            {
                sender.Write("Please use the format friends add/remove player name.");
                return;
            }

            string targetedFriendName = actionInput.Params[1].ToLower();
            Thing targetFriend = PlayerManager.Instance.FindPlayerByName(targetedFriendName, false);
            
            if (actionInput.Params[0].ToLower() == "add")
            {
                this.AddFriend(sender, targetFriend);
            }
            else if (actionInput.Params[0].ToLower() == "remove")
            {
                this.RemoveFriend(sender, targetedFriendName);
            }
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            string commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            // The comon guards already guarantees the sender is a player, hence no null checks here.
            this.player = actionInput.Controller.Thing;
            this.playerBehavior = this.player.Behaviors.FindFirst<PlayerBehavior>();

            return null;
        }

        private void AddFriend(IController sender, Thing targetFriend)
        {
            if (targetFriend == null)
            {
                sender.Write(string.Format("{0} doesn't appear to be online at the moment.", targetFriend.Name));
                return;
            }

            if (targetFriend == this.player)
            {
                sender.Write("You cannot add yourself as a friend.");
                return;
            }

            if (this.playerBehavior.Friends.Contains(targetFriend.Name))
            {
                sender.Write(string.Format("{0} is already on your friends list.", this.player.Name));
                return;
            }

            this.playerBehavior.AddFriend(this.player.Name);
            sender.Write(string.Format("You have added {0} to your friends list.", targetFriend.Name));
        }

        private void RemoveFriend(IController sender, string targetedFriendName)
        {
            string playerName = (from string f in this.playerBehavior.Friends
                                 where f.Equals(targetedFriendName, System.StringComparison.CurrentCultureIgnoreCase)
                                 select f).FirstOrDefault();

            if (string.IsNullOrEmpty(playerName))
            {
                sender.Write(string.Format("{0} is not on your friends list.", targetedFriendName));
                return;
            }

            this.playerBehavior.RemoveFriend(playerName);

            sender.Write(string.Format("{0} has been removed from your friends list.", this.player.Name));
        }
    }
}