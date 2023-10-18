using Shared.Enums;

namespace Shared.Model
{
    public class Packet
    {
        public long ReuestId { get; set; }
        public PacketType Type { get; set; }
        public object Data { get; set; }
    }
}
