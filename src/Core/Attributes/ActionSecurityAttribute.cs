//-----------------------------------------------------------------------------
// <copyright file="ActionSecurityAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   The ActionSecurity attribute can be assigned to Actions to restrict the 
//   action from being utilized by users who do not have the right permissions.
//   Updated: April 2009 by Karak: Utilizing enumeration for SecurityRoles now.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Attributes
{
    using System;

    /// <summary>
    /// SecurityRole can be used with bitwise logic.  For instance, to have a command that 
    /// is available to mobiles and items, but not players and such, you can set the role:
    /// [ActionSecurity(SecurityRole.mobile &amp; SecurityRole.item)]
    /// </summary>
    /// <remarks>
    /// Actions should generally only specify the most basic roles that are needed.  The
    /// players will tend to accumulate flags as they are promoted, or be stripped of flags 
    /// as they are demoted.  For instance, a player who also has basic builder permissions 
    /// will have "SecurityRole.player | SecurityRole.minorBuilder" and if they then get 
    /// promoted to a full builder, they will have "SecurityRole.player | SecurityRole.minor
    /// Builder | SecurityRole.fullBuilder".  Thus commands that are available to minor 
    /// builders will by extension be available to higher level builders, and one does not 
    /// need to chain [ActionSecurity(SecurityRole.minorBuilder | SecurityRole.fullBuilder)]
    /// and the like;  instead just use [ActionSecurity(SecurityRole.minorBuilder)].
    /// </remarks>
    [Flags]
    public enum SecurityRole
    {
        /// <summary>Commands flagged with 'SecurityRole.none' can not be executed.</summary>
        /// <remarks>This should not be used on actual commands.</remarks>
        none = 0x0000,

        /// <summary>Commands flagged with this role are available to mobiles.</summary>
        mobile = 0x0001,

        /// <summary>
        /// Commands flagged with this role are available to items.  This may seem strange, 
        /// but the events system may more properly trigger 'commands' rather than redundant 
        /// 'events' at which time commands specific to items may be appropriate.  For example,
        /// a command for 'self-destruct' may make sense for an item, but not a player or mob.
        /// </summary>
        item = 0x0002,

        /// <summary>Commands flagged with this role are available to rooms.</summary>
        /// <remarks>As above, this is not in use yet but may be useful to future events systems.</remarks>
        room = 0x0004,

        /// <summary>Commands flagged with this role are available to brand new tutorial players.</summary>
        /// <remarks>
        /// This may be used to lock out advanced commands until a tutorial is completed, or to provide
        /// tutorial-specific commands such as opting-out of the tutorial / marking it completed early.
        /// </remarks>
        tutorialPlayer = 0x0010,

        /// <summary>Commands flagged with this role are available to the typical player.</summary>
        /// <remarks>Most players will only have this role, but may additionally have others depending on the situation.</remarks>
        player = 0x0020,

        /// <summary>Commands flagged with this role are available to 'helper' players.</summary>
        /// <remarks>Intended for commands that further allow them to assist new players.</remarks>
        helper = 0x0040,

        /// <summary>Commands flagged with this role are available to 'married' players.</summary>
        /// <remarks>These commands are specific to MUD-married pairs, such as divorce commands and such.</remarks>
        married = 0x0080,

        /// <summary>Commands flagged with this role are available to 'minor builders'.</summary>
        /// <remarks>Intended for commands that create and set properties for new rooms, new mobiles, new items, etc.</remarks>
        minorBuilder = 0x0100,

        /// <summary>Commands flagged with this role are available to 'full builders'.</summary>
        /// <remarks>Intended for commands that administrate entire zones, mark zones as approved, etc.</remarks>
        fullBuilder = 0x0200,

        /// <summary>Commands flagged with this role are available to 'minor admins'.</summary>
        /// <remarks>Intended for commands which 'watch' a player for bot-playing, send players to 'jail', etc.</remarks>
        minorAdmin = 0x1000,

        /// <summary>Commands flagged with this role are available to 'full admins' only.</summary>
        /// <remarks>Intended for commands which 'lockdown' or 'shutdown' the entire MUD, delete characters, etc.</remarks>
        fullAdmin = 0x2000,

        /// <summary>Commands flagged with this role are available to everyone and everything.</summary>
        all = 0xFFFF
    }

    /// <summary>An action security attribute.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ActionSecurityAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the ActionSecurityAttribute class.</summary>
        /// <param name="role">The security role.</param>
        public ActionSecurityAttribute(SecurityRole role)
        {
            this.Role = role;
        }

        /// <summary>Gets the security role specified by this attribute.</summary>
        public SecurityRole Role { get; private set; }
    }
}