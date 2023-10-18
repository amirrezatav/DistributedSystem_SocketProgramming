using Shared.Connection;
using Shared.Utilities;

namespace Client.Connection
{
    public interface IClient : ISocket
    {
        public void Connect(string serverIp, int serverPort, ConnectedHandler connectedHandler);
    }
}
