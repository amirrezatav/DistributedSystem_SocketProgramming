using Shared.Connection;
using Shared.Utilities;

namespace Client.Connection
{
    public interface IClient : ISocket
    {
        void Run();
        void Send(byte[] buffer);
        void Connect(string serverIp, int serverPort, ConnectedHandler connectedHandler);
    }
}
