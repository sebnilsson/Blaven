using System;

namespace Blaven.BlogSources.Markdown
{
    public readonly struct FileData
    {
        public FileData(
            string content,
            string? fileName = null,
            string? folderName = null)
        {
            Content = content
                ?? throw new ArgumentNullException(nameof(content));
            FileName = fileName ?? string.Empty;
            FolderName = folderName ?? string.Empty;
        }

        public readonly string Content { get; }
        public readonly string FileName { get; }
        public readonly string FolderName { get; }
    }
}
