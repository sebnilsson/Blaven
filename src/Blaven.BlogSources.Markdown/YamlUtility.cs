using System;
using Blaven.BlogSources.Markdown.Yaml;
using YamlDotNet.Serialization;

namespace Blaven.BlogSources.Markdown
{
    internal static class YamlUtility
    {
        private static readonly IDeserializer s_yamlDeserializer =
            new DeserializerBuilder()
                .WithTypeConverter(new BlogKeyYamlTypeConverter())
                .WithTypeConverter(new EnumerableStringTypeConverter())
                .IgnoreUnmatchedProperties()
                .Build();

        public static T Deserialize<T>(string input)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            return s_yamlDeserializer.Deserialize<T>(input);
        }
    }
}
