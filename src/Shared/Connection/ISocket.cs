namespace Shared.Connection
{
    public interface ISocket
    {
        bool IsRunning { get; }
        System.Net.Sockets.Socket TransferSocket { get; }
        void Close();
    }
}
