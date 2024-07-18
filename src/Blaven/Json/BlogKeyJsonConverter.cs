using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blaven.Json
{
    public class BlogKeyJsonConverter : JsonConverter<BlogKey>
    {
        public override BlogKey Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var value = reader.GetString() ?? string.Empty;

            return new BlogKey(value);
        }

        public override void Write(
            Utf8JsonWriter writer,
            BlogKey value,
            JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
