namespace Shared.Utilities
{
    public static class Configuration
    {
        public static int MaxPacketSize { get; set;} = 1024 * 8;
        public static int MinPacketSize { get; set;} = 9;
    }
}
