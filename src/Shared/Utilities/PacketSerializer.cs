using Newtonsoft.Json;
using Shared.Model;
using Shared.Models;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Shared.Packets;
public static class PacketSerializer
{
    // Serialize an object to a binary stream
    public static byte[] Serialize(Packet obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        using (MemoryStream memoryStream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(memoryStream))
        {
            // Serialize the object to the binary stream
            // You can customize the serialization logic here
            // For example, you can serialize individual properties or fields manually
            // For simplicity, this example assumes obj is serializable.
            writer.Write(JsonConvert.SerializeObject(obj));
            return memoryStream.ToArray();
        }
    }

    // Deserialize a binary stream into an object
    public static Packet Deserialize(byte[] data,int count)
    {
        if (data == null || data.Length == 0)
        {
            throw new ArgumentNullException(nameof(data));
        }

        using (MemoryStream memoryStream = new MemoryStream(data,0,count))
        using (BinaryReader reader = new BinaryReader(memoryStream))
        {
            // Deserialize the object from the binary stream
            string typeName = reader.ReadString();
            if (string.IsNullOrEmpty(typeName))
            {
                throw new InvalidOperationException($"Type '{typeName}' not found.");
            }
            return JsonConvert.DeserializeObject<Packet>(typeName);
        }
    }
}
