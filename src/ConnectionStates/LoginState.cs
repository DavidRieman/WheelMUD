﻿//-----------------------------------------------------------------------------
// <copyright file="LoginState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server;

namespace WheelMUD.ConnectionStates
{
    using System;
    using WheelMUD.Core;
    using WheelMUD.Data;
    using WheelMUD.Data.Repositories;

    /// <summary>The 'login' session state.</summary>
    public class LoginState : SessionState
    {
        private readonly string userName;

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
                if (authenticatedUser != null)
                {
                    Session.User = authenticatedUser;
                    if (!AppConfigInfo.Instance.UserAccountIsPlayerCharacter)
                    {
                        throw new NotImplementedException("Need to build a ChooseCharacterState!");
                    }
                    else
                    {
                        var characterId = Session.User.PlayerCharacterIds[0];
                        Session.Thing = DocumentRepository<Thing>.Load(characterId);
                        // TODO: https://github.com/DavidRieman/WheelMUD/pull/66 - Clean up previous session properly (this won't always work).
                        Session.Thing.Parent?.Children.RemoveAll(t => t.Id == this.Session.Thing.Id);
                        Session.Thing.Behaviors.SetParent(Session.Thing);
                        var playerBehavior = Session.Thing.FindBehavior<PlayerBehavior>();
                        if (playerBehavior != null)
                        {
                            Session.Thing.Behaviors.FindFirst<UserControlledBehavior>().Controller = Session;
                            playerBehavior.LogIn(Session);
                            Session.AuthenticateSession();
                            Session.SetState(new PlayingState(Session));
                        }
                        else
                        {
                            Session.Write(new OutputBuilder().AppendLine("This character player state is broken. You may need to contact an admin for a possible recovery attempt."));
                            Session.InformSubscribedSystem(Session.ID + " failed to load due to missing player behavior.");
                            Session.SetState(new ConnectedState(Session));
                            Session.WritePrompt();
                        }
                    }
                }
                else
                {
                    Session.Write(new OutputBuilder().AppendLine("Incorrect user name or password."), false);
                    Session.InformSubscribedSystem(Session.ID + " failed to log in");
                    Session.SetState(new ConnectedState(Session));
                    Session.WritePrompt();
                }
            }
        }

        public override OutputBuilder BuildPrompt()
        {
            return new OutputBuilder().Append("Please enter your password: > ");
        }

        /// <summary>Authenticate the user name and password supplied.</summary>
        /// <returns>True if authenticated, else false.</returns>
        private User Authenticate(string password)
        {
            return PlayerRepositoryExtensions.Authenticate(userName, password);
        }
    }
}