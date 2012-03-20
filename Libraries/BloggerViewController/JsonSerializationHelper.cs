using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace BloggerViewController {
    /// <summary>
    /// A static helper-class to help serialize and deserialze to JSON.
    /// </summary>
    public static class JsonSerializationHelper {
        /// <summary>
        /// Gets a deserialized object from a serialized string.
        /// </summary>
        /// <typeparam name="TDeserialized">The type of the deserialized object.</typeparam>
        /// <param name="serializedData">The string containing the serialized data.</param>
        /// <param name="defaultValue">The default value to return if the deserialized object was null.</param>
        /// <returns>Returns a deserialized object the give type.</returns>
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

        /// <summary>
        /// Gets a serialized string from an object.
        /// </summary>
        /// <typeparam name="TSerialized">The type of the object to serialize.</typeparam>
        /// <param name="deserializedData">The object to serialize.</param>
        /// <returns>Returns a serialized string of an object.</returns>
        public static string GetSerializedString<TSerialized>(TSerialized deserializedData)
            where TSerialized : class {
            var type = deserializedData.GetType();
            return GetSerializedString(deserializedData, type);
        }

        /// <summary>
        /// Gets a serialized string from an object.
        /// </summary>
        /// <param name="deserializedData">The object to serialize.</param>
        /// <param name="objectType">The type of the object to serialize.</param>
        /// <returns>Returns a serialized string of an object.</returns>
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
