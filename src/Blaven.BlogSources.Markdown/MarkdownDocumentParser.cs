namespace Blaven.BlogSources.Markdown
{
    internal static class MarkdownDocumentParser
    {
        private const string YamlSeparator = "---";

        public static MarkdownDocument Parse(string content)
        {
            var yamlStart = content.IndexOf(YamlSeparator);

            var yamlIndex = yamlStart + YamlSeparator.Length;

            var yamlEnd = content.IndexOf(YamlSeparator, yamlIndex);

            var containsYaml =
                yamlStart == 0 && yamlEnd > (yamlStart + YamlSeparator.Length);
            if (!containsYaml)
            {
                return new MarkdownDocument(content);
            }

            var yamlLength = yamlEnd - yamlIndex;

            var yaml = content.Substring(yamlIndex, yamlLength);

            var bodyIndex = yamlEnd + YamlSeparator.Length;

            var body = content.Substring(bodyIndex);

            var trimmedYaml = TrimNewLines(yaml);
            var trimmedBody = TrimNewLines(body);

            return new MarkdownDocument(yaml: trimmedYaml, body: trimmedBody);
        }

        private static string TrimNewLines(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.Trim('\r', '\n');
        }
    }
}
