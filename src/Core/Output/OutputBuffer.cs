//-----------------------------------------------------------------------------
// <copyright file="OutputBuffer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   This class provides a server side output buffer, IE it wraps the text to
//   a predefined number of lines.
//   Created: November 2009 by BenGecko for extended more capabilities.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Output
{
    using System.Collections.Generic;
    using WheelMUD.Core.Enums;

    /// <summary>Output buffer.</summary>
    public class OutputBuffer
    {
        /// <summary>Current location in the buffer.</summary>
        private readonly object lockObject = new object();

        /// <summary>List of output rows.</summary>
        private List<string> outputBuffer;

        /// <summary>Current location in the buffer.</summary>
        private int currentLocation;

        /// <summary>Initializes a new instance of the <see cref="OutputBuffer"/> class.</summary>
        public OutputBuffer()
        {
            this.outputBuffer = new List<string>();
            this.currentLocation = 0;
        }

        /// <summary>Gets the total length of the output buffer.</summary>
        public int Length
        {
            get { return this.outputBuffer.Count; }
        }

        /// <summary>Gets the current location in the output buffer.</summary>
        public int CurrentLocation
        {
            get { return this.currentLocation; }
        }

        /// <summary>Gets a value indicating whether there is more data available.</summary>
        public bool HasMoreData
        {
            get
            {
                lock (this.lockObject)
                {
                    int length = this.outputBuffer.Count;

                    if (length > 0 && this.currentLocation < length)
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
            lock (this.lockObject)
            {
                this.outputBuffer.Add(outputRow);
            }
        }

        /// <summary>Add an array of rows to the buffer.</summary>
        /// <param name="outputRows">Rows to add to the buffer.</param>
        public void Append(string[] outputRows)
        {
            lock (this.lockObject)
            {
                foreach (string outputRow in outputRows)
                {
                    this.outputBuffer.Add(outputRow);
                }
            }
        }

        /// <summary>Clears out the output buffer.</summary>
        public void Clear()
        {
            lock (this.lockObject)
            {
                this.outputBuffer.Clear();
                this.currentLocation = 0;
            }
        }

        /// <summary>Returns the requested rows from the Output Buffer.</summary>
        /// <param name="bufferDirection">The direction to move in the buffer.</param>
        /// <param name="maxRows">The maximum rows to return.</param>
        /// <returns>Rows from the output buffer.</returns>
        public string[] GetRows(BufferDirection bufferDirection, int maxRows)
        {
            lock (this.lockObject)
            {
                int start = this.currentLocation;
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
                        end = this.outputBuffer.Count;
                        break;
                }

                if (end > this.outputBuffer.Count)
                {
                    end = this.outputBuffer.Count;
                }

                string[] output = new string[end - start];

                for (int i = start; i < end; i++)
                {
                    output[i - start] = this.outputBuffer[i];
                }

                this.currentLocation = end;
                return output;
            }
        }
    }
}