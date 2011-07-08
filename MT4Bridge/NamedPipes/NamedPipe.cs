// Named Pipe
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace MT4Bridge.NamedPipes
{
    public class PipeException : Exception
    {
        uint code;
        public uint Code { get { return code; } }

        public PipeException(String text)
            : base(text)
        {
        }
        public PipeException(String text, uint code)
            : base(text)
        {
            this.code = code;
        }
        protected PipeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    internal class NamedPipe : IDisposable
    {
        const int IN_BUFFER_SIZE    = 50 * 1024;
        const int OUT_BUFFER_SIZE   = 2  * 1024;
        const int CONNECT_TIMEOUT   = 100;
        const int RECONNECT_TIMEOUT = 10;
        const int CONNECT_RETRY     = 2;

        const uint MODE_NOWAIT = NamedPipeNative.PIPE_TYPE_MESSAGE | NamedPipeNative.PIPE_READMODE_MESSAGE | NamedPipeNative.PIPE_NOWAIT;
        const uint MODE_WAIT   = NamedPipeNative.PIPE_TYPE_MESSAGE | NamedPipeNative.PIPE_READMODE_MESSAGE | NamedPipeNative.PIPE_WAIT;

        IntPtr handle;
        string name;

        public string Name { get { return name.Substring(9); } }

        public NamedPipe(string name)
        {
            this.name = @"\\.\pipe\" + name;
            handle = IntPtr.Zero;
        }

        public void Dispose()
        {
            if (handle != IntPtr.Zero)
                Close();
        }

        public string Read()
        {
            byte[] bytes = ReadBytes();
            if (bytes == null)
                return "";
            string r = Encoding.UTF8.GetString(bytes);
            Bridge.Log(string.Format("Read: '{0}'", r));
            return r;
        }
        public byte[] ReadBytes()
        {
            if (handle == IntPtr.Zero)
                throw new PipeException("Pipe is not connected!");

            byte[] intBytes = new byte[4];
            byte[] msgBytes = new byte[IN_BUFFER_SIZE];
            if (!NamedPipeNative.ReadFile(handle, msgBytes, IN_BUFFER_SIZE, intBytes, 0))
                throw new PipeException("Error reading from pipe!", NamedPipeNative.GetLastError());
            
            Array.Resize<byte>(ref msgBytes, BitConverter.ToInt32(intBytes, 0));
            return msgBytes;
        }

        public void Write(string text)
        {
            Bridge.Log(string.Format("Write: '{0}'", text));
            WriteBytes(Encoding.UTF8.GetBytes(text));
        }
        public void WriteBytes(byte[] bytes)
        {
            if (handle == IntPtr.Zero)
                throw new PipeException("Pipe is not connected!");

            byte[] intBytes = new byte[4];
            if (!NamedPipeNative.WriteFile(handle, bytes, (uint)bytes.Length, intBytes, 0))
                throw new PipeException("Error writing to pipe!", NamedPipeNative.GetLastError());
            
            int written = BitConverter.ToInt32(intBytes, 0);
            if (written != bytes.Length)
                throw new PipeException(string.Format("Error writing to pipe. Bytes requested {0} written {1}", bytes.Length, written));
        }

        public bool Connect()
        {
            for (int i = 0; i <= CONNECT_RETRY; i++) {
                handle = NamedPipeNative.CreateFile(name, NamedPipeNative.GENERIC_READ | NamedPipeNative.GENERIC_WRITE, 0, null, NamedPipeNative.OPEN_EXISTING, 0, 0);
                if (handle.ToInt32() != NamedPipeNative.INVALID_HANDLE_VALUE)
                    return true;
                handle = IntPtr.Zero;
                uint code = NamedPipeNative.GetLastError();
                if (code != NamedPipeNative.ERROR_PIPE_BUSY)
                    break;
                Thread.Sleep(RECONNECT_TIMEOUT);
            }
            return false;
        }

        public void Create()
        {
            handle = NamedPipeNative.CreateNamedPipe(
                name,
                NamedPipeNative.PIPE_ACCESS_DUPLEX,
                MODE_WAIT,
                1, //NamedPipeNative.PIPE_UNLIMITED_INSTANCES,
                OUT_BUFFER_SIZE,
                IN_BUFFER_SIZE,
                NamedPipeNative.NMPWAIT_WAIT_FOREVER,
                IntPtr.Zero);

            if (handle.ToInt32() == NamedPipeNative.INVALID_HANDLE_VALUE) {
                handle = IntPtr.Zero;
                throw new PipeException("Error creating named pipe '" + name + "'!", NamedPipeNative.GetLastError());
            }
        }

        public void ClientConnect()
        {
            if (handle == IntPtr.Zero)
                throw new PipeException("Pipe is not created!");

            bool connected = NamedPipeNative.ConnectNamedPipe(handle, IntPtr.Zero);
            if (!connected && NamedPipeNative.GetLastError() != NamedPipeNative.ERROR_PIPE_CONNECTED)
                throw new PipeException("Error connecting to client pipe!", NamedPipeNative.GetLastError());
        }

        public void Close()
        {
            if (handle == IntPtr.Zero)
                throw new PipeException("Pipe is not connected!");

            bool rc = NamedPipeNative.CloseHandle(handle);
            handle = IntPtr.Zero;
            if (!rc)
                throw new PipeException("Error closing the pipe!", NamedPipeNative.GetLastError());
        }

        public void Flush()
        {
            if (handle == IntPtr.Zero)
                throw new PipeException("Pipe is not connected!");

            if (!NamedPipeNative.FlushFileBuffers(handle))
                throw new PipeException("Error flushing the pipe!", NamedPipeNative.GetLastError());
        }

        public void Disconnect()
        {
            if (handle == IntPtr.Zero)
                throw new PipeException("Pipe is not connected!");

            if (!NamedPipeNative.DisconnectNamedPipe(handle))
                throw new PipeException("Error disconnecting the pipe!", NamedPipeNative.GetLastError());
        }

        public void SetMode(uint mode)
        {
            if (handle == IntPtr.Zero)
                throw new PipeException("Pipe is not connected!");

            if (!NamedPipeNative.SetNamedPipeHandleState(handle, ref mode, IntPtr.Zero, IntPtr.Zero))
                throw new PipeException("Error setting pipe state!", NamedPipeNative.GetLastError());
        }
    }
}
