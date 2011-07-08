// Client Pipe
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace MT4Bridge.NamedPipes
{
    internal sealed class ClientPipe : NamedPipe, IDisposable
    {
        public ClientPipe(string name) : base(name) { }

        public string Command(string command)
        {
            Write(command);
            return Read();
        }
    }
}
