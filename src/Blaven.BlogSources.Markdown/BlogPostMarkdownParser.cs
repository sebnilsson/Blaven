using System;

namespace Blaven.BlogSources.Markdown
{
    internal static class BlogPostMarkdownParser
    {
        public static BlogPost? Parse(BlogKey blogKey, string markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
            {
                return null;
            }

            throw new NotImplementedException();
        }
    }
}
