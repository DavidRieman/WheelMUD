//-----------------------------------------------------------------------------
// <copyright file="Move.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A script that allows players to move.
//   Created: November 2006 by Foxedup
// </summary>
//-----------------------------------------------------------------------------
/*
namespace WheelMUD.Actions
{
    public class Move : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive, 
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.InitiatorMustBeBalanced,
            CommonGuards.InitiatorMustBeMobile
        };

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

            // Rule: Is the param passed a correct one?
            string parameter = actionInput.Noun;
            if (parameter == null)
            {
                return "Unable to see where you are trying to move to.";
            }
            
            this.direction = ConvertShortDirectionToLong(parameter);
            
            //// @@@ TODO: Rule: Is anything blocking exit?

            // Rule: The command sender must be generally able to move.
            this.movableBehavior = actionInput.Controller.Thing.BehaviorManager.FindFirst<MovableBehavior>();
            if (this.movableBehavior == null)
            {
                return "You are unable to move.";
            }

            return null;
        }

    }
}*/