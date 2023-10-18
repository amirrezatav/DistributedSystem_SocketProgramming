namespace Shared.Packets;
public static class PacketSerializer
{
    // Serialize an object to a binary stream
    public static byte[] Serialize(object obj)
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
            writer.Write(obj.GetType().FullName);
            writer.Write(SerializeObject(obj));
            return memoryStream.ToArray();
        }
    }

    // Deserialize a binary stream into an object
    public static T Deserialize<T>(byte[] data,int count)
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
            Type objectType = Type.GetType(typeName);
            if (objectType == null)
            {
                throw new InvalidOperationException($"Type '{typeName}' not found.");
            }

            return DeserializeObject<T>(reader);
        }
    }

    // Helper method to serialize an object to a binary stream
    private static byte[] SerializeObject(object obj)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(memoryStream))
        {
            // Implement your custom serialization logic here
            // You can serialize individual properties or fields manually
            // For simplicity, this example assumes obj is serializable.
            writer.Write(obj.ToString());
            return memoryStream.ToArray();
        }
    }

    // Helper method to deserialize an object from a binary stream
    private static T DeserializeObject<T>(BinaryReader reader)
    {
        // Implement your custom deserialization logic here
        // You can read and reconstruct the object's properties or fields
        // For simplicity, this example assumes obj is deserializable.
        string objString = reader.ReadString();
        return (T)Convert.ChangeType(objString, typeof(T));
    }
}
