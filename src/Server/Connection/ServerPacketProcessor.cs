using Shared.Connection;
using System;
using System.Threading.Tasks;

namespace Server.Connection
{
    public class ServerPacketProcessor : IPacketProcessor
    {
        public Task ConnectionFailed(Exception error)
        {
            throw new NotImplementedException();
        }

        public Task Process(byte[] _buffer, int Validcount, Transfer transfer)
        {
            throw new NotImplementedException();
        }
    }
}
