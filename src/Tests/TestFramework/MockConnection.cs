//-----------------------------------------------------------------------------
// <copyright file="TestSession.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using WheelMUD.Core;
using WheelMUD.Interfaces;
using WheelMUD.Server;
using WheelMUD.Server.Interfaces;

namespace TestHelpers
{
    public class MockConnection : IConnection
    {
        public MockConnection() { }

        public List<string> MessagesSent { get; set; } = new List<string>();

        public string ID => throw new NotImplementedException();

        public bool AtNewLine { get; set; }

        public string LastRawInput
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public System.Net.IPAddress CurrentIPAddress => throw new NotImplementedException();

        public OutputBuffer OutputBuffer
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public TerminalOptions TerminalOptions => new TerminalOptions();

        public ITelnetCodeHandler TelnetCodeHandler => throw new NotImplementedException();

        public byte[] Data => throw new NotImplementedException();

        public StringBuilder Buffer => throw new NotImplementedException();

        public int PagingRowLimit
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public string LastInputTerminator
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public void ResetMessages()
        {
            MessagesSent.Clear();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void Send(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void Send(string data, bool sendAllData = false)
        {
            MessagesSent.Add(data);
        }

        public void ProcessBuffer(BufferDirection bufferDirection)
        {
            throw new NotImplementedException();
        }
    }
}
