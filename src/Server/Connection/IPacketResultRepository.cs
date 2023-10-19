using Shared.Models;
using System.Collections.Generic;

namespace Server.Connection
{
    public interface IPacketResultRepository
    {
        PacketResult Add(PacketResult packetResult);
        PacketResult GetById(long clientId,long requestId);
        List<PacketResult> GetAll();
        PacketResult AddResult(long clientId, long requestId,List<Person> people);
        bool Delete(long clientId, long requestId);
    }


}
