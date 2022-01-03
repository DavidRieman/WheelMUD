﻿//-----------------------------------------------------------------------------
// <copyright file="PlayerPromptableAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;

namespace WheelMUD.Core
{
    /// <summary>The attribute which marks a player prompting system.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PlayerPromptableAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the PlayerPromptableAttribute class.</summary>
        /// <param name="token">Token to replaced in the string, for example %PlayerCurrentHealth%</param>
        /// <param name="description">String description to be provided to the player, for example "Displays your current health,"</param>
        public PlayerPromptableAttribute(string token, string description)
        {
            Token = token;
            Description = description;
        }

        public string Token { get; private set; }

        public string Description { get; private set; }
    }
}