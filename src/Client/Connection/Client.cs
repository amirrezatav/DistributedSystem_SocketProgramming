using Shared.Utilities;
using System;
using System.Net.Sockets;
using System.Net;
using Shared.Connection;
using System.IO;

namespace Client.Connection
{
    public class Client : IClient
    {
        private Socket _clientSocket;
        private IPacketProcessor _processPacket;
        private EndPoint _endpoint;
        private bool _isRunning = false;
        private event ExceptionHandler _exceptionHandler;
        private event ConnectedHandler _connectedHandler;
        private event DisconnectedHandler _disconnectedHandler;
        private int _bufferSize = Configuration.MaxPacketSize;
        private byte[] _buffer;
        public EndPoint ServerEndpoint { get { return _endpoint; } }
        public Socket ClientSocket { get { return _clientSocket; } }
        public bool IsRunning { get { return _isRunning; } }
        public Socket TransferSocket { get { return _clientSocket; } }

        public Client(DisconnectedHandler disconnectedHandler, ExceptionHandler exceptionHandler, IPacketProcessor processPacket)
        {
            _disconnectedHandler = disconnectedHandler;
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _exceptionHandler = exceptionHandler;
            _processPacket = processPacket;
        }

        public void Connect(string serverIp, int serverPort, ConnectedHandler connectedHandler)
        {
            if (_isRunning)
                return;

            _connectedHandler = connectedHandler;
            _isRunning = true;
            _clientSocket.BeginConnect(serverIp, serverPort, connectedcallback, null);
        }

        private void connectedcallback(IAsyncResult ar)
        {
            string error = null;
            try
            {
                _clientSocket.EndConnect(ar);
                _endpoint = (EndPoint)_clientSocket.RemoteEndPoint;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            _connectedHandler(this, error);
        }

        public void Close()
        {
            if (!_isRunning)
                return;

            _clientSocket.Close();
            _isRunning = false;
            _disconnectedHandler(this);
        }
        public async void Send(byte[] buffer)
        {
            if (!_clientSocket.Connected)
                return;
            try
            {
                _clientSocket.Send(BitConverter.GetBytes(buffer.Length), 0, 4, SocketFlags.None);
                int Offset = 0;
                while (true)
                {
                    var sent = Offset * Configuration.MaxPacketSize;
                    var remine = buffer.Length - sent;
                    if (remine <= 0)
                        break;
                    int sendSize = remine < Configuration.MaxPacketSize ? remine : Configuration.MaxPacketSize;
                    int realSent = _clientSocket.Send(buffer, sent, sendSize, SocketFlags.None);
                    Offset++;
                }
            }
            catch (Exception ex)
            {
                _clientSocket.Close();
                _exceptionHandler(this, ex);
            }
        }


        public void Run()
        {
            _buffer = new byte[Configuration.MaxPacketSize];
            _clientSocket.BeginReceive(_buffer, 0, _bufferSize, SocketFlags.Peek, receiveCallback, null);
        }
        private async void receiveCallback(IAsyncResult ar)
        {
            try
            {
                if (!_clientSocket.Connected)
                    throw new Exception("Socket Disconnected.");
                int ReceiveSize = _clientSocket.EndReceive(ar);
                if (ReceiveSize >= 4)
                {
                    _clientSocket.Receive(_buffer, 0, 4, SocketFlags.None);
                    int size = BitConverter.ToInt32(_buffer, 0);
                    using (var ms = new MemoryStream())
                    {
                        int readSize = size < Configuration.MaxPacketSize ? size : Configuration.MaxPacketSize;
                        int read = _clientSocket.Receive(_buffer, 0, readSize, SocketFlags.None);
                        ms.Write(_buffer, 0, read);
                        while (read != size)
                        {
                            readSize = size - read < Configuration.MaxPacketSize ? size - read : Configuration.MaxPacketSize;
                            read += _clientSocket.Receive(_buffer, 0, readSize, SocketFlags.None);
                            ms.Write(_buffer, 0, readSize);
                        }
                        _processPacket.Process(ms.ToArray(), size, this);
                    }

                }
                Run();
            }
            catch (Exception ex)
            {
                _clientSocket.Close();
                _exceptionHandler(this, ex);
            }
        }
    }
}
