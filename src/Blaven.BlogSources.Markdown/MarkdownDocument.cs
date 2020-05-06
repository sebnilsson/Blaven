namespace Blaven.BlogSources.Markdown
{
    internal readonly struct MarkdownDocument
    {
        public MarkdownDocument(string body)
            : this(yaml: string.Empty, body: body)
        {
        }

        public MarkdownDocument(string yaml, string body)
        {
            Yaml = yaml;
            Body = body;
        }

        public readonly string Yaml { get; }
        public readonly string Body { get; }

        public static MarkdownDocument Empty { get; }
            = new MarkdownDocument(string.Empty, string.Empty);
    }
}
