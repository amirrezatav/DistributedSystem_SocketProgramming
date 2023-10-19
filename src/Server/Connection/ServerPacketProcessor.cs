using Newtonsoft.Json.Linq;
using Shared.Connection;
using Shared.Model;
using Shared.Models;
using Shared.Packets;
using System;
using System.Collections.Generic;

namespace Server.Connection
{
    public class ServerPacketProcessor : IPacketProcessor
    {
        private readonly IAcceptedClientRepository _repository;
        private readonly IPacketResultRepository _packetRepository;
        private readonly IListener _server;
        private long RequestCounter = 0;
        public ServerPacketProcessor(IAcceptedClientRepository repository, IListener server)
        {
            _repository = repository;
            _server = server;
            _packetRepository = new PacketResultRepository();
        }
        private List<List<Person>> Quantize(List<Person> source)
        {
            var parts = _repository.Count();
            List<List<Person>> Result = new List<List<Person>>();
            List<Person> shuffledList = ShuffleList(source);
            List<List<Person>> dividedParts = new List<List<Person>>();

            int elementsPerPart = shuffledList.Count / parts;
            int remainder = shuffledList.Count % parts;
            int currentIndex = 0;

            for (int i = 0; i < parts; i++)
            {
                int partSize = elementsPerPart + (i < remainder ? 1 : 0);
                List<Person> part = shuffledList.GetRange(currentIndex, partSize);
                currentIndex += partSize;
                dividedParts.Add(part);
            }
            return dividedParts;
        }
        public List<T> ShuffleList<T>(List<T> source)
        {
            List<T> shuffledList = new List<T>(source);
            Random rng = new Random();
            int n = shuffledList.Count;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = shuffledList[k];
                shuffledList[k] = shuffledList[n];
                shuffledList[n] = value;
            }

            return shuffledList;
        }
        public void Process(byte[] _buffer, int Validcount, object transfer)
        {
            lock (transfer)
            {
                var clienId = (transfer as AcceptedClient).Id;
                lock (_buffer)
                {
                    var packet = PacketSerializer.Deserialize(_buffer, Validcount);
                    switch (packet.Type)
                    {
                        case Shared.Enums.PacketType.Query:
                            // _server.Send(PacketSerializer.Serialize(packet)); broadcast
                            packet.ReuestId = ++RequestCounter;
                            packet.ClientId = clienId;
                            _packetRepository.Add(new PacketResult(Timeout) { ClientId = clienId, RequestId = packet.ReuestId });
                            var clients = _repository.GetAll();
                            foreach (var item in clients)
                            {
                                item.Value.Send(PacketSerializer.Serialize(packet));
                            }
                           
                            break;
                        case Shared.Enums.PacketType.QueryResult:
                            var reuest = _packetRepository.AddResult(packet.ClientId, packet.ReuestId, ((Newtonsoft.Json.Linq.JArray)packet.Data).ToObject<List<Person>>());
                            if(reuest.AnsNumber == _repository.Count())
                            {
                                packet.Data = reuest.People;
                                _repository.GetById(reuest.ClientId).Send(PacketSerializer.Serialize(packet));
                                _packetRepository.Delete(packet.ClientId, packet.ReuestId);
                            }
                            break;
                        case Shared.Enums.PacketType.NewData:
                            _repository.GetRandom().Send(PacketSerializer.Serialize(packet));
                            break;
                        case Shared.Enums.PacketType.Import:
                            var records = Quantize(((Newtonsoft.Json.Linq.JArray)packet.Data).ToObject<List<Person>>());
                            foreach (var record in records)
                            {
                                packet.Data = record;
                                _repository.GetRandom().Send(PacketSerializer.Serialize(packet));
                            }
                            break;
                        default: break;
                    }
                }
            }

        }

        private void Timeout(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
