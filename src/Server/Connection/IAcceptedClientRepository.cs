using System.Net.Sockets;
using Shared.Connection;
using Shared.Utilities;
using System.Collections.Concurrent;

namespace Server.Connection
{
    public interface IAcceptedClientRepository
    {
        AcceptedClient Create(Socket acceptedSocket, IPacketProcessor receiveHandler, ExceptionHandler exceptionHander);
        AcceptedClient GetById(long id);
        AcceptedClient GetRandom();
        int Count();
        ConcurrentDictionary<long, AcceptedClient> GetAll();
        void Update(AcceptedClient updatedClient);
        void Delete(long id);
    }


}
