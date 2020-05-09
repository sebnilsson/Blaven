using System;
using System.IO;

namespace Blaven.BlogSources.FileProviders
{
    public readonly struct FileData
    {
        public FileData(
            string content,
            string? fileName = null,
            string? folderName = null,
            DateTimeOffset? createdAt = null)
        {
            Content = content
                ?? throw new ArgumentNullException(nameof(content));
            FileName = fileName ?? string.Empty;
            FolderName = folderName ?? string.Empty;
            CreatedAt = createdAt;

            FileExtension = Path.GetExtension(FileName);
            Exists = true;
        }

        public readonly DateTimeOffset? CreatedAt { get; }

        public readonly string Content { get; }

        public readonly bool Exists { get; }

        public readonly string FileExtension { get; }

        public readonly string FileName { get; }

        public readonly string FolderName { get; }
    }
}
