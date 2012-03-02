using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace BloggerViewController {
    public static class SerializationHelper {
        public static TDeserialized GetDeserializedObject<TDeserialized>(string serializedData,
            TDeserialized defaultValue = default(TDeserialized)) where TDeserialized : class {
            if(string.IsNullOrWhiteSpace(serializedData)) {
                return defaultValue;
            }

            var serializer = new DataContractJsonSerializer(typeof(TDeserialized));
            using(var stream = new MemoryStream(Encoding.UTF8.GetBytes(serializedData))) {
                var deserialized = serializer.ReadObject(stream) as TDeserialized;
                return deserialized ?? defaultValue;
            }
        }

        public static string GetSerializedString<TSerialized>(TSerialized deserializedData)
            where TSerialized : class {
            var type = deserializedData.GetType();
            return GetSerializedString(deserializedData, type);
        }

        public static string GetSerializedString(object deserializedData, Type objectType = null) {
            if(deserializedData == null) {
                return null;
            }

            var type = (objectType != null) ? objectType : deserializedData.GetType();

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);

            using(MemoryStream stream = new MemoryStream()) {
                serializer.WriteObject(stream, deserializedData);
                if(stream.Length < 1) {
                    return null;
                }

                byte[] buffer = new byte[stream.Length];

                stream.Position = 0;
                stream.Read(buffer, 0, (int)stream.Length);

                string serialized = Encoding.UTF8.GetString(buffer);
                return serialized;
            }
        }
    }
}
