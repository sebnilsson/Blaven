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

                meta.BlogKey =
                    meta.BlogKey.Value.Coalesce(
                        fileData.FolderName,
                        fileName);

                meta.Id = meta.Id.Coalesce(fileName);
                meta.PublishedAt ??= fileData.CreatedAt;
                meta.SourceId = meta.SourceId.Coalesce(fileData.FileName);

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
