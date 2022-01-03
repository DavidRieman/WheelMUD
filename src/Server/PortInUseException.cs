﻿//-----------------------------------------------------------------------------
// <copyright file="PortInUseException.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;

namespace WheelMUD.Server
{
    /// <summary>Exception for the attempted socket port being in use.</summary>
    public class PortInUseException : Exception
    {
        /// <summary>Initializes a new instance of the PortInUseException class.</summary>
        /// <param name="message">The exception message.</param>
        public PortInUseException(string message) : base(message) { }
    }
}