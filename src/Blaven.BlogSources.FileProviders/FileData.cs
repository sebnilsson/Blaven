using System;
using System.IO;
using System.Linq;

namespace Blaven.BlogSources.FileProviders
{
    public readonly struct FileData
    {
        public const char KeyFolderChar = '.';
        public const string KeyFolderString = ".";

        private static readonly char[] s_directorySeparatorChars =
            [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar];

        public FileData(
            string content,
            string? fileName = null,
            string? relativeFolderPath = null,
            DateTimeOffset? createdAt = null,
            DateTimeOffset? updatedAt = null)
        {
            Content = content
                ?? throw new ArgumentNullException(nameof(content));
            FileName = fileName ?? string.Empty;
            RelativeFolderPath =
                (relativeFolderPath ?? string.Empty)
                .Trim(s_directorySeparatorChars);
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;

            FileExtension = Path.GetExtension(FileName);
            Exists = true;

            KeyFolderName = GetKeyFolderName(RelativeFolderPath);
        }

        public readonly DateTimeOffset? CreatedAt { get; }

        public readonly string Content { get; }

        public readonly bool Exists { get; }

        public readonly string FileExtension { get; }

        public readonly string FileName { get; }

        public readonly string KeyFolderName { get; }

        public readonly string RelativeFolderPath { get; }

        public readonly DateTimeOffset? UpdatedAt { get; }

        private static string GetKeyFolderName(string relativeFolderPath)
        {
            if (string.IsNullOrWhiteSpace(relativeFolderPath))
            {
                return string.Empty;
            }

            var directories =
                relativeFolderPath
                    .Split(s_directorySeparatorChars)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim(s_directorySeparatorChars))
                    .Reverse();

            var keyFolderName =
                directories.FirstOrDefault(x => x.StartsWith(KeyFolderString));

            return
                keyFolderName?.TrimStart(KeyFolderChar) ?? string.Empty;
        }
    }
}
