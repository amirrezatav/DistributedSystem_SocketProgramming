namespace Shared.Connection
{
    public interface IPacketProcessor
    {
        public Task ConnectionFailed(Exception error);
        public Task Process(byte[] _buffer, int Validcount, Transfer transfer);
    }
}
