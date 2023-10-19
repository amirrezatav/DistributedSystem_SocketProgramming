using Shared.Model;
using Shared.Utilities;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System;
using Shared.Connection;

namespace Server.Connection
{
    public class Listener : IListener
    {
        private Socket _serverSocket;
        private int _serverPort = -1;
        private string _serverIp = "";
        private bool _isRunning = false;

        private event SocketAcceptedHandler _accepteHandler;
        private event ExceptionHandler _exceptionHandler;


        public bool IsRunning
        {
            get { return _isRunning; }
        }
        public int Port
        {
            get { return _serverPort; }
        }
        public string Ip
        {
            get { return _serverIp; }
        }
        public Socket BaseSocket
        {
            get { return _serverSocket; }
        }


        public Listener(SocketAcceptedHandler handler, ExceptionHandler exceptionHandler)
        {
            _accepteHandler = handler;
            _exceptionHandler = exceptionHandler;
        }
        public async void Send(byte[] buffer)
        {
            if (!_serverSocket.Connected)
                return;
            try
            {
                _serverSocket.Send(BitConverter.GetBytes(buffer.Length), 0, 4, SocketFlags.None);
                int Offset = 0;
                while (true)
                {
                    var sent = Offset * Configuration.MaxPacketSize;
                    var remine = buffer.Length - sent;
                    if (remine <= 0)
                        break;
                    int sendSize = remine < Configuration.MaxPacketSize ? remine : Configuration.MaxPacketSize;
                    int realSent = _serverSocket.Send(buffer, sent, sendSize, SocketFlags.None);
                    Offset++;
                }
            }
            catch (Exception ex)
            {
                _serverSocket.Close();
                _exceptionHandler(this, ex);
            }
        }

        public static Task<List<string>> GetAllIpAsync()
        {
            List<string> list = new List<string>();
            string myHost = System.Net.Dns.GetHostName();
            list.Add("0.0.0.0");
            System.Net.IPHostEntry myIPs = System.Net.Dns.GetHostEntry(myHost);

            foreach (var item in myIPs.AddressList)
            {
                if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    list.Add(item.ToString());
            }

            return Task.FromResult(list);
        }

        public void Start(string ip, int port)
        {
            if (_isRunning)
                return;
            _serverPort = port;
            _serverIp = ip;
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Parse(_serverIp), _serverPort));
            _serverSocket.Listen(int.MaxValue);
            _isRunning = true;
            _serverSocket.BeginAccept(callback, null);
        }
        private void callback(IAsyncResult ar)
        {
            try
            {
                Socket sck = _serverSocket.EndAccept(ar);
                if (sck != null)
                {
                    _accepteHandler(this, sck);
                }
            }
            catch (Exception ex)
            {
                _isRunning = false;
                _exceptionHandler(this, ex);
                Close();
            }
            if (_isRunning)
                _serverSocket.BeginAccept(callback, null);
            else return;
        }

        public void Close()
        {
            if (_serverSocket != null)
            {
                _serverSocket.Close();
                _serverSocket.Dispose();
            }
        }

    }

}
