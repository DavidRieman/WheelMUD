//-----------------------------------------------------------------------------
// <copyright file="Crypto.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.SystemFramework.Encryption
{
    using System.Globalization;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Cryptographic class for encryption and decryption of string values.
    /// </summary>
    public static class Crypto
    {
        // Arbitrary key and iv vector. 
        // You will want to generate (and protect) your own when using encryption.
        
        /// <summary>Arbitrary action key.</summary>
        private const string ActionKey = "EA81AA1D5FC1EC53E84F30AA746139EEBAFF8A9B76638895";
        
        /// <summary>iv vector.</summary>
        private const string ActionIv = "87AF7EA221F3FFF5";

        /// <summary>@@@ What is this?</summary>
        private static readonly TripleDESCryptoServiceProvider des3;

        /// <summary>
        /// Initializes static members of the Crypto class.
        /// Default constructor. Initializes the DES3 encryption provider. 
        /// </summary>
        static Crypto()
        {
            des3 = new TripleDESCryptoServiceProvider { Mode = CipherMode.CBC };
        }

        /// <summary>
        /// Generates a 24 byte Hex key.
        /// </summary>
        /// <returns>Generated Hex key.</returns>
        public static string GenerateKey()
        {
            // Length is 24
            des3.GenerateKey();
            return BytesToHex(des3.Key);
        }

        /// <summary>
        /// Generates an 8 byte Hex IV (Initialization Vector).
        /// </summary>
        /// <returns>Initialization vector.</returns>
        public static string GenerateIV()
        {
            // Length = 8
            des3.GenerateIV();
            return BytesToHex(des3.IV);
        }

        /// <summary>
        /// Encrypts a memory string (i.e. variable).
        /// </summary>
        /// <param name="data">String to be encrypted.</param>
        /// <param name="key">Encryption key.</param>
        /// <param name="iv">Encryption initialization vector.</param>
        /// <returns>Encrypted string.</returns>
        public static string Encrypt(string data, string key, string iv)
        {
            byte[] bdata = Encoding.ASCII.GetBytes(data);
            byte[] bkey = HexToBytes(key);
            byte[] biv = HexToBytes(iv);

            var stream = new MemoryStream();
            var encStream = new CryptoStream(
                    stream,
                    des3.CreateEncryptor(bkey, biv),
                    CryptoStreamMode.Write);

            encStream.Write(bdata, 0, bdata.Length);
            encStream.FlushFinalBlock();
            encStream.Close();

            return BytesToHex(stream.ToArray());
        }

        /// <summary>
        /// Decrypts a memory string (i.e. variable).
        /// </summary>
        /// <param name="data">String to be decrypted.</param>
        /// <param name="key">Original encryption key.</param>
        /// <param name="iv">Original initialization vector.</param>
        /// <returns>Decrypted string.</returns>
        public static string Decrypt(string data, string key, string iv)
        {
            byte[] bdata = HexToBytes(data);
            byte[] bkey = HexToBytes(key);
            byte[] biv = HexToBytes(iv);

            var stream = new MemoryStream();
            var encStream = new CryptoStream(
                    stream,
                    des3.CreateDecryptor(bkey, biv),
                    CryptoStreamMode.Write);

            encStream.Write(bdata, 0, bdata.Length);
            encStream.FlushFinalBlock();
            encStream.Close();

            return Encoding.ASCII.GetString(stream.ToArray());
        }

        /// <summary>
        /// Standard encrypt method for Patterns in DoFactory.
        /// Uses the predefined key and iv.
        /// </summary>
        /// <param name="data">String to be encrypted.</param>
        /// <returns>Encrypted string</returns>
        public static string ActionEncrypt(string data)
        {
            return Encrypt(data, ActionKey, ActionIv);
        }

        /// <summary>
        /// Standard decrypt method for Patterns in DoFactory.
        /// Uses the predefined key and iv.
        /// </summary>
        /// <param name="data">String to be decrypted.</param>
        /// <returns>Decrypted string.</returns>
        public static string ActionDecrypt(string data)
        {
            return Decrypt(data, ActionKey, ActionIv);
        }

        /// <summary>
        /// Converts a hex string to a byte array.
        /// </summary>
        /// <param name="hex">Hex string.</param>
        /// <returns>Byte array.</returns>
        private static byte[] HexToBytes(string hex)
        {
            var bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length / 2; i++)
            {
                string code = hex.Substring(i * 2, 2);
                bytes[i] = byte.Parse(code, NumberStyles.HexNumber);
            }
            
            return bytes;
        }

        /// <summary>
        /// Converts a byte array to a hex string.
        /// </summary>
        /// <param name="bytes">Byte array.</param>
        /// <returns>Converted hex string</returns>
        private static string BytesToHex(byte[] bytes)
        {
            var hex = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                hex.AppendFormat("{0:X2}", bytes[i]);
            }
            
            return hex.ToString();
        }
    }
}
