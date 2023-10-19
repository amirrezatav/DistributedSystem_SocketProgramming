namespace Shared.Connection
{
    public interface IPacketProcessor
    {
        public void Process(byte[] _buffer, int Validcount, object transfer);
    }
}
