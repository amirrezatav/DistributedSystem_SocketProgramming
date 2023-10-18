﻿using Shared.Utilities;
using System;
using System.Net.Sockets;
using System.Net;

namespace Client.Connection
{
    public class Client : IClient
    {
        private Socket _clientSocket;
        private EndPoint _endpoint;
        private bool _isRunning = false;

        private event ConnectedHandler _connectedHandler;
        private event DisconnectedHandler _disconnectedHandler;

        public EndPoint ServerEndpoint { get { return _endpoint; } }
        public Socket ClientSocket { get { return _clientSocket; } }
        public bool IsRunning { get { return _isRunning; } }
        public Socket TransferSocket { get { return _clientSocket; } }

        public Client(DisconnectedHandler disconnectedHandler)
        {
            _disconnectedHandler = disconnectedHandler;
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
    }
}