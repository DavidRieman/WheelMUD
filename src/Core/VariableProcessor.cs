//-----------------------------------------------------------------------------
// <copyright file="VariableProcessor.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;

    /// <summary>The variable processor.</summary>
    public static class VariableProcessor
    {
        /// <summary>Sets the specified variable's new value.</summary>
        /// <param name="name">The variable to set.</param>
        /// <param name="newValue">The new value for the variable.</param>
        public static void Set(string name, string newValue)
        {
            Environment.SetEnvironmentVariable(name, newValue, EnvironmentVariableTarget.Process);
        }

        /// <summary>Processes the specified input.</summary>
        /// <param name="input">The input to process.</param>
        /// <returns>Expanded environment variables based on the input.</returns>
        public static string Process(string input)
        {
            return Environment.ExpandEnvironmentVariables(input);
        }
    }
}