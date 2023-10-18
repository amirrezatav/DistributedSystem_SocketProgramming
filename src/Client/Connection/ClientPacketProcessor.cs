using Shared.Connection;
using System.Threading.Tasks;
using System;

namespace Client.Connection
{
    public class ClientPacketProcessor : IPacketProcessor
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
