using Shared.Enums;
using System.Runtime.Serialization.Formatters.Binary;

namespace Shared.Model
{
    [Serializable]
    public class Packet
    {
        public long ClientId { get; set; }
        public long ReuestId { get; set; }
        public PacketType Type { get; set; }
        public object Data { get; set; }
    }
}
