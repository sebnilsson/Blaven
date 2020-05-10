using System.IO;
using Blaven.BlogSources.FileProviders;
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

                var fileName =
                    Path.GetFileNameWithoutExtension(fileData.FileName);

                meta.BlogKey = meta.BlogKey.Value.Coalesce(fileData.FolderName);

                meta.Id = meta.Id.Coalesce(fileName);
                meta.SourceId = meta.SourceId.Coalesce(fileData.FileName);
                meta.PublishedAt ??= fileData.CreatedAt;
                meta.UpdatedAt ??= fileData.UpdatedAt;

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
