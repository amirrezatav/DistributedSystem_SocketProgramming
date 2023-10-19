namespace Shared.Enums
{
    public enum PacketType : byte
    {
        Unknown,
        Query,
        QueryResult,
        NewData,
        Import,
        Command
    }
}
