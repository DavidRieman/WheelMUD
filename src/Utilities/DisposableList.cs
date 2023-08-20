//-----------------------------------------------------------------------------
// <copyright file="DisposableList.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace WheelMUD.Utilities
{
    /// <summary>
    /// A List derivative which implements IDisposable, and cleans up after each of
    /// its IDisposable items whenever it is destructed or disposed of itself.
    /// </summary>
    /// <typeparam name="T">The IDisposable type managed by this list.</typeparam>
    public class DisposableList<T>
        : List<T>, IDisposable where T : IDisposable
    {
        /// <summary>The synchronization locking object to prevent simultaneous disposal from multiple threads.</summary>
        private readonly object lockObject = new();

        /// <summary>Finalizes an instance of the DisposableList class.</summary>
        ~DisposableList()
        {
            Dispose();
        }

        /// <summary>Dispose of all disposable resources that are managed by this list.</summary>
        public void Dispose()
        {
            lock (lockObject)
            {
                if (Count > 0)
                {
                    foreach (T t in this)
                    {
                        t.Dispose();
                    }

                    Clear();
                }
            }
        }
    }
}