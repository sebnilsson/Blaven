using System.Text.Json;

namespace Blaven.Json
{
    public static class BlavenJsonSerializer
    {
        private static readonly JsonSerializerOptions s_serializeOptions =
            GetJsonSerializerOptions();

        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new BlogKeyJsonConverter());

            return options;
        }

        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, s_serializeOptions);
        }

        public static string Serialize(object value)
        {
            return JsonSerializer.Serialize(value, s_serializeOptions);
        }
    }
}
