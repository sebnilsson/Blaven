using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Blaven.BlogSources.Markdown.Yaml
{
    internal class BlogKeyYamlTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(BlogKey);
        }

        public object? ReadYaml(IParser parser, Type type)
        {
            var scalar = parser.Consume<Scalar>();

            return new BlogKey(scalar.Value);
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            var strValue = value?.ToString() ?? string.Empty;

            emitter.Emit(new Scalar(strValue));
        }
    }
}
