using System;

namespace Blaven.BlogSources.Markdown
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
        }

        public DateTimeOffset? CreatedAt { get; }

        public readonly string Content { get; }

        public readonly string FileName { get; }

        public readonly string FolderName { get; }
    }
}
