using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Blaven.BlogSources.Markdown.Yaml
{
    internal class EnumerableStringTypeConverter : IYamlTypeConverter
    {
        private static readonly Type s_type = typeof(IEnumerable<string>);

        public bool Accepts(Type type)
        {
            return s_type.IsAssignableFrom(type);
        }

        public object? ReadYaml(IParser parser, Type type)
        {
            parser.Consume<SequenceStart>();

            var list = new List<string>();

            while (parser.TryConsume<Scalar>(out var scalar))
            {
                list.Add(scalar.Value);
            }

            parser.Consume<SequenceEnd>();

            return list;
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            throw new NotImplementedException();
        }
    }
}
