using System.Text.Json;

namespace Blaven.BlogSources.Markdown
{
    internal static class BlogMetaJsonParser
    {
        public static BlogMeta? Parse(FileData fileData)
        {
            if (string.IsNullOrWhiteSpace(fileData.Content))
            {
                return null;
            }

            try
            {
                var meta = ParseInternal(fileData.Content);

                meta.BlogKey =
                    meta.BlogKey.HasValue
                    ? meta.BlogKey
                    : fileData.FolderName;

                meta.Id ??= fileData.FileName;

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
