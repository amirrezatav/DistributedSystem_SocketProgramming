using System.Net.Sockets;
using Shared.Connection;
using Shared.Utilities;
using System;
using System.IO;

namespace Server.Connection
{
    public class AcceptedClient
    {
        public long Id { get; set; }
        public Socket Accepted { get; private set; }
        private int _bufferSize = Configuration.MaxPacketSize;
        private IPacketProcessor _receiveHandler ;
        private byte[] _buffer;
        private event ExceptionHandler _exceptionHander;

        public AcceptedClient(Socket socket, IPacketProcessor receiveHandler, ExceptionHandler exceptionHander)
        {
            Accepted = socket;
            _exceptionHander = exceptionHander;
            _receiveHandler = receiveHandler;
        }
        public void Send(byte[] buffer)
        {
            if (!Accepted.Connected)
                    throw new Exception("Socket Disconnected.");
            Accepted.Send(BitConverter.GetBytes(buffer.Length), 0, 4, SocketFlags.None);
            int Offset = 0;
            while (true)
            {
                var sent = Offset * Configuration.MaxPacketSize;
                var remine = buffer.Length - sent;
                if (remine <= 0)
                    break;
                int sendSize = remine < Configuration.MaxPacketSize ? remine : Configuration.MaxPacketSize;
                int realSent = Accepted.Send(buffer, sent, sendSize, SocketFlags.None);
                Offset++;
            }
        }
        public void Run()
        {
            _buffer = new byte[Configuration.MaxPacketSize];
            Accepted.BeginReceive(_buffer, 0, _bufferSize, SocketFlags.Peek, receiveCallback, null);
        }
        private async void receiveCallback(IAsyncResult ar)
        {
            try
            {
                if (!Accepted.Connected)
                    throw new Exception("Socket Disconnected.");
                int ReceiveSize = Accepted.EndReceive(ar);
                if (ReceiveSize >= 4)
                {
                    Accepted.Receive(_buffer, 0, 4, SocketFlags.None);
                    int size = BitConverter.ToInt32(_buffer, 0);
                    using (var ms = new MemoryStream())
                    {
                        int readSize = size < Configuration.MaxPacketSize ? size : Configuration.MaxPacketSize;
                        int read = Accepted.Receive(_buffer, 0, readSize, SocketFlags.None);
                        ms.Write(_buffer, 0, read);
                        while (read != size)
                        {
                            readSize = size - read < Configuration.MaxPacketSize ? size - read : Configuration.MaxPacketSize;
                            read += Accepted.Receive(_buffer, 0, readSize, SocketFlags.None);
                            ms.Write(_buffer, 0, readSize);
                        }
                        _receiveHandler.Process(ms.ToArray(), size, this);
                    }
                }
                Run();
            }
            catch (Exception ex)
            {
                _exceptionHander(this,ex);
            }
        }
    }


}
