//-----------------------------------------------------------------------------
// <copyright file="MudPrincipal.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A MUD Principal for Entity Authorization.
//   Created: December 2006 by Hector Sosa, Jr (aka Fastalanasa).
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Universe
{
    using System.Collections.Generic;
    using System.Security.Principal;
    using WheelMUD.Core.Attributes;

    /// <summary>
    /// This class implements the <see cref="System.Security.Principal.IPrincipal"/> interface.
    /// This will give <see cref="WheelMUD.Universe.Entity"/> child classes a .NET-style authorization mechanism.
    /// This will also provide a mechanism for custom roles to
    /// <see cref="WheelMUD.Universe.Entity">entities</see> in the mud.
    /// </summary>
    public class MudPrincipal : IPrincipal
    {
        /// <summary>The synchronization locking object.</summary>
        private readonly object lockObject = new object();

        /// <summary>A string array that holds role names.</summary>
        private string[] roles;

        /// <summary>A list of RoleDataObjects for this Principal.</summary>
        ////private List<RoleRecord> roleDataObjects;

        /// <summary>A list of SecurityRole enums for this Principal.</summary>
        ////private List<SecurityRole> securityRoles;

        /// <summary>The private identity for this Principal.</summary>
        private IIdentity identity;

        /// <summary>Initializes a new instance of the <see cref="MudPrincipal"/> class.</summary>
        /// <param name="entityIdentity">The entity identity.</param>
        /// <param name="roles">The array that contains the roles.</param>
        public MudPrincipal(IIdentity entityIdentity, string[] roles)
        {
            this.identity = entityIdentity;
            this.roles = roles;
        }

        /*
        /// <summary>Initializes a new instance of the <see cref="MudPrincipal"/> class.</summary>
        /// <param name="entityIdentity">The entity identity.</param>
        /// <param name="roles">The list that contains the role data objects.</param>
        public MudPrincipal(IIdentity entityIdentity, List<RoleRecord> roles)
        {
            this.identity = entityIdentity;
            this.roleDataObjects = roles;
        }*/

        /// <summary>Initializes a new instance of the <see cref="MudPrincipal"/> class.</summary>
        /// <param name="entityIdentity">The entity identity.</param>
        /// <param name="roles">The list that contains the <see cref="SecurityRole"/> values.</param>
        public MudPrincipal(IIdentity entityIdentity, List<SecurityRole> roles)
        {
            this.identity = entityIdentity;
            ////this.securityRoles = roles;
        }

        /// <summary>Gets the roles.</summary>
        /// <value>The string array containing the roles.</value>
        public string[] Roles
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.roles;
                }
            }
        }

        /// <summary>Gets the Identity for this <see cref="MudPrincipal"/>.</summary>
        public IIdentity Identity
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.identity;
                }
            }
        }

        /// <summary>Checks whether this <see cref="MudPrincipal"/> contains the string role.</summary>
        /// <param name="role">The role to be checked.</param>
        /// <returns>Whether or not this <see cref="MudPrincipal"/> has the specified role.</returns>
        public bool IsInRole(string role)
        {
            foreach (var r in this.roles)
            {
                if (r == role)
                {
                    return true;
                }
            }

            return false;
        }

        /*
        /// <summary>Checks whether this MudPrincipal contains the specific <see cref="RoleRecord"/>.</summary>
        /// <param name="roleDataObject">The <see cref="RoleRecord"/> to be checked.</param>
        /// <returns>Whether or not this <see cref="MudPrincipal"/> has the specified role.</returns>
        public bool IsInRole(RoleRecord roleDataObject)
        {
            foreach (var r in this.roleDataObjects)
            {
                if (r == roleDataObject)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Checks whether this <see cref="MudPrincipal"/> contains the specific RoleDataObject.</summary>
        /// <param name="securityRole">The <see cref="SecurityRole"/> enum to check.</param>
        /// <returns>Whether or not this <see cref="MudPrincipal"/> has the specified role.</returns>
        public bool IsInRole(SecurityRole securityRole)
        {
            foreach (var r in this.roleDataObjects)
            {
                if (r.SecurityRoleMask == (int) securityRole)
                {
                    return true;
                }
            }

            return false;
        }*/
    }
}