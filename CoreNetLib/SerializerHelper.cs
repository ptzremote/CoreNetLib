using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CoreNetLib
{
    internal static class SerializerHelper
    {
        static BinaryFormatter formatter = new BinaryFormatter();
        internal static byte[] Serialize(object data)
        {
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, data);

            return stream.ToArray();
        }
        internal static object Deserialize(byte[] bytes)
        {
            return formatter.Deserialize(new MemoryStream(bytes));
        }
    }
}
