//-----------------------------------------------------------------------------
// <copyright file="MCCPHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Compresses our data into a byte array using zlib compression
//   Created: January 2007 by Foxedup
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server
{
    using System.IO;
    using System.Text;
    using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

    /// <summary>The MUD Client Compression Protocol (MCCP) handler.</summary>
    internal static class MCCPHandler
    {
        /// <summary>Compresses data using zlib.</summary>
        /// <param name="data">The data to compress</param>
        /// <returns>A byte array containing the compressed data</returns>
        public static byte[] Compress(string data)
        {
            // Commented out below as it loses extended ASCII (127+) and replaces with a ?
            // byte[] bytes = ASCIIEncoding.ASCII.GetBytes(data);

            // Encoding using default terminal extended ascii codepage 437
            byte[] bytes = Encoding.GetEncoding(437).GetBytes(data);
            byte[] returnBytes;
            using (var stream = new MemoryStream())
            {
                using (var compressedStream = new DeflaterOutputStream(stream))
                {
                    compressedStream.Write(bytes, 0, bytes.Length);
                    compressedStream.Finish();
                    stream.Position = 0;

                    returnBytes = new byte[stream.Length];
                    stream.Read(returnBytes, 0, returnBytes.Length);
                }
            }

            return returnBytes;
        }
    }
}