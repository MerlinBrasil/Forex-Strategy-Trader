// Server Pipe
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Threading;

namespace MT4Bridge.NamedPipes
{
    internal interface IPipeServer
    {
        string Serve(string request);
    }

    internal sealed class ServerPipe : NamedPipe, IDisposable
    {
        IPipeServer server;
        Thread      thread;
        bool        disposed = false;

        public ServerPipe(string name, IPipeServer server) : base(name)
        {
            this.server = server;

            thread = new Thread(new ThreadStart(PipeListener));
            thread.IsBackground = true;
            thread.Name = "Pipe Thread";
            thread.Start();
        }
        ~ServerPipe()
        {
            Dispose(false);
        }

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!disposed) {
                try {
                    thread.Abort();
                } catch (ThreadStateException) {
                }
                using (NamedPipe p = new NamedPipe(Name))
                    p.Connect();
                thread.Join();

                base.Dispose();
            }
            disposed = true;
        }

        void PipeListener()
        {
            try {
                Create();
                while (true) {
                    ClientConnect();
                    Write(server.Serve(Read()));
                    Thread.Sleep(1);
                    Disconnect();
                }
            } catch (ThreadAbortException) {
            } catch (PipeException) {
            }
        }
    }
}
