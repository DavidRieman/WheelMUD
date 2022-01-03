//-----------------------------------------------------------------------------
// <copyright file="GlobalAssemblyInfo.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Global Assembly information for the WheelMUD solution. (Combines with individual AssemblyInfo files for each project.)
// </summary>
//-----------------------------------------------------------------------------

using System.Reflection;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyProduct("WheelMUD Multi-User Dungeon/Dimension Server")]
[assembly: AssemblyCompany("WheelMUD Development Team")]
[assembly: AssemblyCopyright("Copyright � WheelMUD Development Team 1998-2021")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

// Version information for an assembly consists of the following four values:
//   Major Version,  Minor Version,  Build Number,  Revision
[assembly: AssemblyVersion("0.4.5.0")]
[assembly: AssemblyFileVersion("0.4.5.0")]
