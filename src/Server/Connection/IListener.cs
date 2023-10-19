using Shared.Connection;
using System;
using System.Net.Sockets;

namespace Server.Connection
{
    public interface IListener : ISocket
    {
        public void Start(string ip, int port);
        public void Send(byte[] buffer);
    }
}
