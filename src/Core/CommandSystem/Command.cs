//-----------------------------------------------------------------------------
// <copyright file="Command.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Base object representing a command that can be executed by the command executor.
//   Created: January 2007 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Reflection;
    using WheelMUD.Core.Attributes;

    /// <summary>Base object representing a command that can be executed by the command executor.</summary>
    public class Command
    {
        /// <summary>Initializes a new instance of the Command class.</summary>
        /// <param name="type">The type of the command.</param>
        /// <param name="description">The description of the command.</param>
        /// <param name="example">An usage example for this command.</param>
        /// <param name="securityRole">The security role of this command.</param>
        public Command(Type type, string description, string example, SecurityRole securityRole)
        {
            if (type != null)
            {
                this.Constructor = type.GetConstructor(new Type[] { });
                this.Name = type.Name;
            }
            
            this.Description = description;
            this.Example = example;
            this.SecurityRole = securityRole;
        }

        /// <summary>Initializes a new instance of the Command class.</summary>
        /// <param name="type">The type of the command.</param>
        /// <param name="securityRole">The security role of this command.</param>
        public Command(Type type, SecurityRole securityRole)
            : this(type, null, null, securityRole)
        {
        }

        /// <summary>Gets the constructor for creating instances of the command.</summary>
        public ConstructorInfo Constructor { get; private set; }

        /// <summary>Gets the name of the command.</summary>
        public string Name { get; private set; }

        /// <summary>Gets the description of this command.</summary>
        public string Description { get; private set; }

        /// <summary>Gets the example usage of this command.</summary>
        public string Example { get; private set; }

        /// <summary>Gets or sets a value indicating whether this is the PrimaryAlias.</summary>
        public bool PrimaryAlias { get; set; }

        /// <summary>Gets or sets the category that this action is filed under.</summary>
        public CommandCategory Category { get; set; }

        /// <summary>Gets or sets the security role of this command.</summary>
        public SecurityRole SecurityRole { get; set; }
    }
}