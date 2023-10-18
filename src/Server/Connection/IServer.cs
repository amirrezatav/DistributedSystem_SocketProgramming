using Shared.Connection;

namespace Server.Connection
{
    public interface IServer : ISocket
    {
        public void Start(string ip, int port);
    }
}
