//-----------------------------------------------------------------------------
// <copyright file="OutputBuffer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections.Generic;
    using WheelMUD.Core.Enums;

    /// <summary>OutputBuffer provides a server side output buffer.</summary>
    /// <remarks>Used for paging, to avoid flooding users with too many lines in one go.</remarks>
    public class OutputBuffer
    {
        /// <summary>Current location in the buffer.</summary>
        private readonly object lockObject = new object();

        /// <summary>List of output rows.</summary>
        private readonly List<string> outputBuffer = new List<string>();

        /// <summary>Initializes a new instance of the <see cref="OutputBuffer"/> class.</summary>
        public OutputBuffer() { }

        /// <summary>Gets the total length of the output buffer.</summary>
        public int Length
        {
            get { return outputBuffer.Count; }
        }

        /// <summary>Gets the current location in the output buffer.</summary>
        public int CurrentLocation { get; private set; } = 0;

        /// <summary>Gets a value indicating whether there is more data available.</summary>
        public bool HasMoreData
        {
            get
            {
                lock (lockObject)
                {
                    int length = outputBuffer.Count;

                    if (length > 0 && CurrentLocation < length)
                    {
                        return true;
                    }

                    return false;
                }
            }
        }

        /// <summary>Add a row to the buffer.</summary>
        /// <param name="outputRow">Single row to add to the buffer.</param>
        public void Append(string outputRow)
        {
            lock (lockObject)
            {
                outputBuffer.Add(outputRow);
            }
        }

        /// <summary>Add an array of rows to the buffer.</summary>
        /// <param name="outputRows">Rows to add to the buffer.</param>
        public void Append(string[] outputRows)
        {
            lock (lockObject)
            {
                foreach (string outputRow in outputRows)
                {
                    outputBuffer.Add(outputRow);
                }
            }
        }

        /// <summary>Clears out the output buffer.</summary>
        public void Clear()
        {
            lock (lockObject)
            {
                outputBuffer.Clear();
                CurrentLocation = 0;
            }
        }

        /// <summary>Returns the requested rows from the Output Buffer.</summary>
        /// <param name="bufferDirection">The direction to move in the buffer.</param>
        /// <param name="maxRows">The maximum rows to return.</param>
        /// <returns>Rows from the output buffer.</returns>
        public string[] GetRows(BufferDirection bufferDirection, int maxRows)
        {
            lock (lockObject)
            {
                int start = CurrentLocation;
                int end = 0;

                switch (bufferDirection)
                {
                    case BufferDirection.Repeat:
                        start = start - maxRows;
                        if (start < 0)
                        {
                            start = 0;
                        }

                        end = start + maxRows;
                        break;
                    case BufferDirection.Backward:
                        start = start - (2 * maxRows);
                        if (start < 0)
                        {
                            start = 0;
                        }

                        end = start + maxRows;
                        break;
                    case BufferDirection.Forward:
                        end = start + maxRows;
                        break;
                    case BufferDirection.ForwardAllData:
                        end = outputBuffer.Count;
                        break;
                }

                if (end > outputBuffer.Count)
                {
                    end = outputBuffer.Count;
                }

                string[] output = new string[end - start];

                for (int i = start; i < end; i++)
                {
                    output[i - start] = outputBuffer[i];
                }

                CurrentLocation = end;
                return output;
            }
        }
    }
}