using System;
using Blaven.BlogSources.Markdown.Yaml;
using YamlDotNet.Serialization;

namespace Blaven.BlogSources.Markdown
{
    internal class YamlConverter
    {
        private readonly IDeserializer _yamlDeserializer =
            new DeserializerBuilder()
                .WithTypeConverter(new BlogKeyYamlTypeConverter())
                .WithTypeConverter(new EnumerableStringTypeConverter())
                .IgnoreUnmatchedProperties()
                .Build();

        public T Deserialize<T>(string input)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            return _yamlDeserializer.Deserialize<T>(input);
        }
    }
}
