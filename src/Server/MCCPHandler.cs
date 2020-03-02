//-----------------------------------------------------------------------------
// <copyright file="MCCPHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server
{
    using System.IO;
    using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

    /// <summary>The MUD Client Compression Protocol (MCCP) handler.</summary>
    /// <remarks>Compresses our data into a byte array using zlib compression.</remarks>
    internal static class MCCPHandler
    {
        /// <summary>Compresses data using zlib.</summary>
        /// <param name="data">The data to compress</param>
        /// <returns>A byte array containing the compressed data</returns>
        public static byte[] Compress(string data)
        {
            byte[] bytes = Connection.CurrentEncoding.GetBytes(data);
            byte[] returnBytes;
            
            using var stream = new MemoryStream();
            using var compressedStream = new DeflaterOutputStream(stream);
            compressedStream.Write(bytes, 0, bytes.Length);
            compressedStream.Finish();
            stream.Position = 0;

            returnBytes = new byte[stream.Length];
            stream.Read(returnBytes, 0, returnBytes.Length);

            return returnBytes;
        }
    }
}