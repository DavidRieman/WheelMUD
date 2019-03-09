//-----------------------------------------------------------------------------
// <copyright file="SimpleWeakReference.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Utilities
{
    using System;

    /// <summary>Strongly typed weak reference.</summary>
    /// <typeparam name="T">The object type.</typeparam>
    public class SimpleWeakReference<T> : WeakReference
        where T : class
    {
        public SimpleWeakReference(T target, bool trackResurrection)
            : base(target, trackResurrection)
        { }

        public SimpleWeakReference(T target)
            : base(target)
        { }

        public new T Target
        {
            get { return base.Target as T; }
            set { base.Target = value; }
        }
    }
}