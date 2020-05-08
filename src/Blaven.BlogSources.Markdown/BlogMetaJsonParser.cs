using Blaven.Json;

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
                    meta.BlogKey.Value.Coalesce(
                        fileData.FolderName,
                        fileData.FolderName);

                meta.Id = meta.Id.Coalesce(fileData.FileName);
                meta.PublishedAt ??= fileData.CreatedAt;

                return meta;
            }
            catch
            {
                return null;
            }
        }

        private static BlogMeta ParseInternal(string json)
        {
            return BlavenJsonSerializer.Deserialize<BlogMeta>(json);
        }
    }
}
