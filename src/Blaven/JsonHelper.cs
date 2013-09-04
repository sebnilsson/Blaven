using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Blaven
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerSettings DefaultSettings;

        static JsonHelper()
        {
            var contractResolver = new CamelCasePropertyNamesContractResolver();
            DefaultSettings = new JsonSerializerSettings { ContractResolver = contractResolver };
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, DefaultSettings);
        }

        public static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value); //, DefaultSettings);
        }
    }
}