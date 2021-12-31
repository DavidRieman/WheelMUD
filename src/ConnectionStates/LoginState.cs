//-----------------------------------------------------------------------------
// <copyright file="LoginState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Linq;
using WheelMUD.Core;
using WheelMUD.Data;
using WheelMUD.Data.Repositories;
using WheelMUD.Server;

namespace WheelMUD.ConnectionStates
{
    /// <summary>The 'login' session state.</summary>
    public class LoginState : SessionState
    {
        private static readonly OutputBuilder promptPasswordOutput = new OutputBuilder().Append("Please enter your password: > <%hidden%>");
        private readonly string userName;
        private bool isLoggingIn = false;

        /// <summary>Initializes a new instance of the LoginState class.</summary>
        /// <param name="session">The session entering this state.</param>
        public LoginState(Session session, string userName)
            : base(session)
        {
            this.userName = userName;
        }

        /// <summary>Process the specified input.</summary>
        /// <param name="command">The input to process.</param>
        public override void ProcessInput(string command)
        {
            if (command != string.Empty)
            {
                var authenticatedUser = Authenticate(command);
                if (authenticatedUser == null)
                {
                    Session.WriteLine("Incorrect user name or password.", false);
                    Session.InformSubscribedSystem(Session.ID + " failed to log in");
                    Session.SetState(new ConnectedState(Session));
                    Session.WritePrompt();
                    return;
                }

                isLoggingIn = true;
                Session.User = authenticatedUser;
                if (!AppConfigInfo.Instance.UserAccountIsPlayerCharacter)
                {
                    // TODO: https://github.com/DavidRieman/WheelMUD/issues/32
                    throw new NotImplementedException("Need to build a ChooseCharacterState!");
                }
                else
                {
                    var characterId = Session.User.PlayerCharacterIds[0];

                    var existingPlayer = PlayerManager.Instance.Players.Where(p => p.Parent?.Id == characterId).FirstOrDefault();
                    Session.Thing = existingPlayer?.Parent ?? DocumentRepository<Thing>.Load(characterId);

                    Session.Thing.Behaviors.RepairParent(Session.Thing);
                    var playerBehavior = Session.Thing.FindBehavior<PlayerBehavior>();
                    if (playerBehavior != null)
                    {
                        Session.Thing.FindBehavior<UserControlledBehavior>().Session = Session;
                        playerBehavior.LogIn(Session);
                        Session.AuthenticateSession();
                        Session.SetState(new PlayingState(Session));
                    }
                    else
                    {
                        Session.WriteLine("This character player state is broken. You may need to contact an admin for a possible recovery attempt.");
                        Session.InformSubscribedSystem(Session.ID + " failed to load due to missing player behavior.");
                        Session.SetState(new ConnectedState(Session));
                        Session.WritePrompt();
                    }

                    Session.Thing.RepairParentTree();
                }
                isLoggingIn = false;
            }
        }

        public override OutputBuilder BuildPrompt()
        {
            // If we are currently processing a possible successful login, we want to avoid prompting for password again.
            return isLoggingIn ? null : promptPasswordOutput;
        }

        /// <summary>Authenticate the user name and password supplied.</summary>
        /// <returns>True if authenticated, else false.</returns>
        private User Authenticate(string password)
        {
            return PlayerRepositoryExtensions.Authenticate(userName, password);
        }
    }
}