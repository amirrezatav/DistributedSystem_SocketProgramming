namespace Shared.Connection
{
    public interface ISocket
    {
        bool IsRunning { get; }
        void Close();
    }
}
