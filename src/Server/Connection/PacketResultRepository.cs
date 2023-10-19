using Shared.Models;
using System.Collections.Generic;
using System.Linq;

namespace Server.Connection
{
    public class PacketResultRepository : IPacketResultRepository
    {
        private List<PacketResult> packetResults;
        private long nextRequestId = 1;

        public PacketResultRepository()
        {
            packetResults = new List<PacketResult>();
        }

        public PacketResult Add(PacketResult packetResult)
        {
            packetResult.RequestId = nextRequestId++;
            packetResults.Add(packetResult);
            return packetResult;
        }


        public List<PacketResult> GetAll()
        {
            return packetResults;
        }

        public bool Delete(long clientId, long requestId)
        {
            PacketResult resultToRemove = packetResults.FirstOrDefault(result => result.RequestId == requestId && result.ClientId == clientId);
            if (resultToRemove != null)
            {
                packetResults.Remove(resultToRemove);
                return true;
            }
            return false;
        }

        public PacketResult AddResult(long clientId, long requestId, List<Person> people)
        {
            if (people == null)
                return null;
            var pack = packetResults.FirstOrDefault(result => result.RequestId == requestId && result.ClientId == clientId);
            if (pack != null)
            {
                pack.People = pack.People.Union(people).ToList();
                pack.AnsNumber++;
            }
            return pack;
        }

        PacketResult IPacketResultRepository.GetById(long clientId, long requestId)
        {
            return packetResults.FirstOrDefault(result => result.RequestId == requestId && result.ClientId == clientId);
        }
    }
}
