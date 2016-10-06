//-----------------------------------------------------------------------------
// <copyright file="DisposableList.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A List derivative which implements IDisposable, and cleans up after each of
//   its IDisposable items whenever it is destructed or disposed of itself.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Utilities
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A List derivative which implements IDisposable, and cleans up after each of
    /// its IDisposable items whenever it is destructed or disposed of itself.
    /// </summary>
    /// <typeparam name="T">The IDisposable type managed by this list.</typeparam>
    public class DisposableList<T>
        : List<T>, IDisposable where T : IDisposable
    {
        /// <summary>The synchronization locking object to prevent simultaneous disposal from multiple threads.</summary>
        private readonly object lockObject = new object();

        /// <summary>Finalizes an instance of the DisposableList class.</summary>
        ~DisposableList()
        {
            this.Dispose();
        }

        /// <summary>Dispose of all disposable resources that are managed by this list.</summary>
        public void Dispose()
        {
            lock (this.lockObject)
            {
                if (this.Count > 0)
                {
                    foreach (T t in this)
                    {
                        t.Dispose();
                    }

                    this.Clear();
                }
            }
        }
    }
}