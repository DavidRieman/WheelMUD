//-----------------------------------------------------------------------------
// <copyright file="LoadedClass.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>//   
//   Created: 2004 By David McClarnon (dmcclarnon@ntlworld.com)
//   Modified: June 16, 2010 by Fastalanasa.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.General
{
    public interface ILoaded
    {
        bool Loaded { get; }
    }

    public class LoadedClass : ILoaded
    {
        protected bool isLoaded;

        public LoadedClass()
        {
        }

        public bool Loaded
        {
            get
            {
                return isLoaded;
            }
        }
    }
}