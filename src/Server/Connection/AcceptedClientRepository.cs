using System.Net.Sockets;
using Shared.Connection;
using Shared.Utilities;
using System.Collections.Concurrent;
using System.Linq;
using System;

namespace Server.Connection
{
    public class AcceptedClientRepository : IAcceptedClientRepository
    {
        private ConcurrentDictionary<long, AcceptedClient> acceptedClients;
        private long nextId;

        public AcceptedClientRepository()
        {
            acceptedClients = new ConcurrentDictionary<long, AcceptedClient>();
            nextId = 1;
        }

        // Create a new AcceptedClient and add it to the repository
        public AcceptedClient Create(Socket acceptedSocket, IPacketProcessor receiveHandler, ExceptionHandler exceptionHander)
        {
            long id = nextId++;
            AcceptedClient newClient = new AcceptedClient(acceptedSocket, receiveHandler, exceptionHander);
            acceptedClients.TryAdd(id, newClient);
            newClient.Id = id;
            return newClient;
        }

        // Retrieve an AcceptedClient by Id
        public AcceptedClient GetById(long id)
        {
            acceptedClients.TryGetValue(id, out var client);
            return client;
        }

        // Retrieve all AcceptedClients
        public ConcurrentDictionary<long, AcceptedClient> GetAll()
        {
            return acceptedClients;
        }

        // Update an AcceptedClient
        public void Update(AcceptedClient updatedClient)
        {
            acceptedClients.AddOrUpdate(updatedClient.Id, updatedClient, (key, oldValue) => updatedClient);
        }

        // Delete an AcceptedClient
        public void Delete(long id)
        {
            acceptedClients.TryRemove(id, out _);
        }

        public AcceptedClient GetRandom()
        {
            var size = acceptedClients.Count();
            var rand = new Random().Next(0, size-1);
            return acceptedClients.ToList()[rand].Value;
        }

        public int Count()
        {
            return acceptedClients.Count();
        }
    }


}
