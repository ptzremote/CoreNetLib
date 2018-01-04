using System;

namespace CoreNetLib
{
    public class NetSettings
    {
        public Func<object, byte[]> serializer;
        public Func<byte[], object> deserializer;

        public NetSettings()
        {
            serializer = SerializerHelper.Serialize;
            deserializer = SerializerHelper.Deserialize;
        }
    }
}
