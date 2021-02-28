//-----------------------------------------------------------------------------
// <copyright file="Friends.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WheelMUD.Core;
using WheelMUD.Interfaces;

namespace WheelMUD.Actions
{
    /// <summary>A command to manipulate a players friends list.</summary>
    /// <remarks>TODO: Persistence!</remarks>
    [ExportGameAction(0)]
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
                if (playerBehavior.Friends.Count == 0)
                {
                    sender.Write("You currently have no friends listed.");
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Your Friends:");
                    foreach (string friendName in playerBehavior.Friends)
                    {
                        string status = PlayerManager.Instance.FindLoadedPlayerByName(friendName, false) == null ? "Offline" : "Online";
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
            Thing targetFriend = PlayerManager.Instance.FindLoadedPlayerByName(targetedFriendName, false);

            if (actionInput.Params[0].ToLower() == "add")
            {
                AddFriend(sender, targetFriend);
            }
            else if (actionInput.Params[0].ToLower() == "remove")
            {
                RemoveFriend(sender, targetedFriendName);
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
            player = actionInput.Controller.Thing;
            playerBehavior = player.Behaviors.FindFirst<PlayerBehavior>();

            return null;
        }

        private void AddFriend(IController sender, Thing targetFriend)
        {
            if (targetFriend == null)
            {
                sender.Write(string.Format("{0} doesn't appear to be online at the moment.", targetFriend.Name));
                return;
            }

            if (targetFriend == player)
            {
                sender.Write("You cannot add yourself as a friend.");
                return;
            }

            if (playerBehavior.Friends.Contains(targetFriend.Name))
            {
                sender.Write(string.Format("{0} is already on your friends list.", player.Name));
                return;
            }

            playerBehavior.AddFriend(player.Name);
            sender.Write(string.Format("You have added {0} to your friends list.", targetFriend.Name));
        }

        private void RemoveFriend(IController sender, string targetedFriendName)
        {
            string playerName = (from string f in playerBehavior.Friends
                                 where f.Equals(targetedFriendName, StringComparison.CurrentCultureIgnoreCase)
                                 select f).FirstOrDefault();

            if (string.IsNullOrEmpty(playerName))
            {
                sender.Write(string.Format("{0} is not on your friends list.", targetedFriendName));
                return;
            }

            playerBehavior.RemoveFriend(playerName);

            sender.Write(string.Format("{0} has been removed from your friends list.", player.Name));
        }
    }
}