//-----------------------------------------------------------------------------
// <copyright file="Friends.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using WheelMUD.Core;
using WheelMUD.Interfaces;
using WheelMUD.Utilities;

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
            if (actionInput.Params.Length == 1 && actionInput.Params[0].ToLower() == "list" || !actionInput.Params.Any())
            {
                if (playerBehavior.Friends.Count == 0)
                {
                    sender.Write("You currently have no friends listed.");
                }
                else
                {
                    var ab = new AnsiBuilder();
                    ab.AppendLine("Your Friends:");
                    foreach (var friendName in playerBehavior.Friends)
                    {
                        var status = PlayerManager.Instance.FindLoadedPlayerByName(friendName, false) == null ? "Offline" : "Online";
                        ab.AppendLine($"{friendName} [{status}]");
                    }

                    sender.Write(ab.ToString());
                }

                return;
            }

            if (actionInput.Params.Length != 2 &&
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

            // The common guards already guarantees the sender is a player, hence no null checks here.
            player = actionInput.Controller.Thing;
            playerBehavior = player.Behaviors.FindFirst<PlayerBehavior>();

            return null;
        }

        private void AddFriend(IController sender, Thing targetFriend)
        {
            var ab = new AnsiBuilder();
            
            if (targetFriend == null)
            {
                ab.AppendLine("Doesn't appear to be online at the moment.");
                sender.Write(ab.ToString());
                return;
            }

            if (targetFriend == player)
            {
                ab.AppendLine("You cannot add yourself as a friend.");
                sender.Write(ab.ToString());
                return;
            }

            if (playerBehavior.Friends.Contains(targetFriend.Name))
            {
                
                ab.AppendLine($"{player.Name} is already on your friends list.");
                sender.Write(ab.ToString());
                return;
            }

            playerBehavior.AddFriend(player.Name);

            ab.AppendLine($"You have added {targetFriend.Name} to your friends list.");
            sender.Write(ab.ToString());
        }

        private void RemoveFriend(IController sender, string targetedFriendName)
        {
            string playerName = (from string f in playerBehavior.Friends
                                 where f.Equals(targetedFriendName, StringComparison.CurrentCultureIgnoreCase)
                                 select f).FirstOrDefault();

            var ab = new AnsiBuilder();
            
            if (string.IsNullOrEmpty(playerName))
            {
                ab.AppendLine($"{targetedFriendName} is not on your friends list.");
                sender.Write(ab.ToString());
                return;
            }

            playerBehavior.RemoveFriend(playerName);

            ab.AppendLine($"{player.Name} has been removed from your friends list.");
            sender.Write(ab.ToString());
        }
    }
}