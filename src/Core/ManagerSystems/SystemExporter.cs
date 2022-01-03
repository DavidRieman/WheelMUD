﻿//-----------------------------------------------------------------------------
// <copyright file="SystemExporter.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Core
{
    /// <summary>
    /// A class for exporting/importing system singleton through MEF, without necessarily
    /// instantiating each one.  (In particular, this prevents creating multiple instances
    /// of systems where we generally only want the instantiate the most recent version.)
    /// </summary>
    public abstract class SystemExporter
    {
        /// <summary>Gets the singleton system instance.</summary>
        /// <returns>A new instance of the singleton system.</returns>
        public abstract ISystem Instance { get; }

        /// <summary>Gets the Type of the singleton system, without instantiating it.</summary>
        /// <returns>The Type of the singleton system.</returns>
        public abstract Type SystemType { get; }
    }
}