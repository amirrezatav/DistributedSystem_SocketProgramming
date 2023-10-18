using Shared.Utilities;
using System.Net.Sockets;

namespace Shared.Connection
{
    public class Transfer
    {
        private int _bufferSize = Configuration.MaxPacketSize;
        private byte[] _buffer;
        private readonly ISocket _socket;
        private readonly IPacketProcessor _processPacket;

        public Transfer(ISocket socket, IPacketProcessor packetProcessor)
        {
            _socket = socket;
            _processPacket = packetProcessor;
            _buffer = new byte[_bufferSize];
        }
        public async void Send(byte[] buffer)
        {
            if (!_socket.IsRunning)
                return;
            try
            {
                _socket.TransferSocket.Send(BitConverter.GetBytes(buffer.Length), 0, 4, SocketFlags.None);
                int Offset = 0;
                while (true)
                {
                    var sent = Offset * Configuration.MaxPacketSize;
                    var remine = buffer.Length - sent;
                    if (remine <= 0)
                        break;
                    int sendSize = remine < Configuration.MaxPacketSize ? remine : Configuration.MaxPacketSize;
                    _socket.TransferSocket.Send(buffer, sent, sendSize, SocketFlags.None);
                }
            }
            catch(Exception ex) 
            {
                _socket.Close();
                await _processPacket.ConnectionFailed(ex);
            }
        }


        public void RunReceive()
        {
            _socket.TransferSocket.BeginReceive(_buffer, 0, _bufferSize, SocketFlags.Peek, receiveCallback, null);
        }
        private async void receiveCallback(IAsyncResult ar)
        {
            try
            {
                int ReceiveSize = _socket.TransferSocket.EndReceive(ar);
                if (ReceiveSize >= 4)
                {
                    _socket.TransferSocket.Receive(_buffer, 0, 4, SocketFlags.None);
                    int size = BitConverter.ToInt32(_buffer, 0);

                    int read = _socket.TransferSocket.Receive(_buffer, 0, size, SocketFlags.None);

                    while (read < size)
                    {
                        read += _socket.TransferSocket.Receive(_buffer, read, size - read, SocketFlags.None);
                    }

                    await _processPacket.Process(_buffer, size, this);
                }
                RunReceive();
            }
            catch (Exception ex)
            {
                _socket.Close();
                await _processPacket.ConnectionFailed(ex);
            }
        }
    }
}
