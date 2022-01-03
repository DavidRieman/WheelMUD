//-----------------------------------------------------------------------------
// <copyright file="CommandGuard.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    /// <summary>Command guard helper methods.</summary>
    public static class CommandGuardHelpers
    {
        /// <summary>Verifies that the proposed command can be performed by the acting entity.</summary>
        /// <param name="command">The issued command to be carried out.</param>
        /// <returns>A string defining why the permission was not given, else null if permission is given.</returns>
        public static string VerifyCommandPermission(ScriptingCommand command)
        {
            if (command.SecurityRole == SecurityRole.none)
            {
                // If you are debugging here after trying to set up your own action, 
                // you probably forgot to assign an appropriate [ActionSecurity(...)] 
                // attribute for your new GameAction class.
                return string.Format("Nobody can use '{0}' right now!", command.Name);
            }

            Thing entity = command.ActionInput.Actor;
            if (entity == null)
            {
                return "You must exist before issuing commands!";
            }

            var user = entity.FindBehavior<UserControlledBehavior>();
            if (user != null)
            {
                // If any of the command security roles and the user security roles overlap (such as the command is
                // marked with 'minorBuilder' and the user has the 'minorBuilder' flag) then we permit the command.
                if ((command.SecurityRole & user.SecurityRoles) != SecurityRole.none)
                {
                    return null;
                }

                // Otherwise, this player does not have permission; we do not want to 
                // check the mobile/item/room security role on players, so we're done.
                return $"You do not have permission to use '{command.Name}' right now.";
            }

            MobileBehavior mobile = entity.FindBehavior<MobileBehavior>();
            if (mobile != null)
            {
                if ((command.SecurityRole & SecurityRole.mobile) != SecurityRole.none)
                {
                    return null;
                }

                return string.Format("A mobile can not use '{0}' right now!", command.Name);
            }

            RoomBehavior room = entity.FindBehavior<RoomBehavior>();
            if (room != null)
            {
                if ((command.SecurityRole & SecurityRole.room) == SecurityRole.none)
                {
                    return null;
                }

                return string.Format("A room can not use '{0}' right now!", command.Name);
            }

            // TODO: For now, everything else which doesn't meet any above category will need the 'item' security 
            //       role. (Do we need an ItemBehavior or is there something else relevant... CanPickupBehavior etc?)
            if ((command.SecurityRole & SecurityRole.item) != SecurityRole.none)
            {
                return null;
            }

            return string.Format("An item (or unrecognized entity) can not use '{0}' right now!", command.Name);
        }
    }

    /*
    internal abstract class CommandGuard : ICommandGuard
    {
        ////private bool mustHave = false;
        private string name = string.Empty;

        public abstract bool Check(IController sender);
    }

    internal class EffectGuard : CommandGuard
    {
        public override bool Check(IController sender)
        {
            return true;
        }
    }

    internal class PermissionGuard : CommandGuard
    {
        public override bool Check(IController sender)
        {
            return true;
        }
    }

    internal class ItemGuard : CommandGuard
    {
        public override bool Check(IController sender)
        {
            return true;
        }
    }
     */
}