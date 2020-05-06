using System.Text.Json;

namespace Blaven.BlogSources.Markdown
{
    internal static class BlogMetaJsonParser
    {
        public static BlogMeta? Parse(BlogKey blogKey, string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                var meta = ParseInternal(json);

                meta.BlogKey =
                    meta.BlogKey.HasValue
                    ? meta.BlogKey
                    : blogKey;

                return meta;
            }
            catch
            {
                return null;
            }
        }

        private static BlogMeta ParseInternal(string json)
        {
            return JsonSerializer.Deserialize<BlogMeta>(json);
        }
    }
}
