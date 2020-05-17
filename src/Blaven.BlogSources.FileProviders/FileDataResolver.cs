using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Blaven.BlogSources.FileProviders
{
    internal class FileDataResolver
    {
        private readonly Encoding _encoding;

        public FileDataResolver(Encoding encoding)
        {
            _encoding = encoding
                ?? throw new ArgumentNullException(nameof(encoding));
        }

        public async Task<FileData> GetFileData(
            DirectoryInfo baseDirectory,
            FileInfo fileInfo)
        {
            if (baseDirectory is null)
                throw new ArgumentNullException(nameof(baseDirectory));
            if (fileInfo is null)
                throw new ArgumentNullException(nameof(fileInfo));

            var content =
                await ReadTextAsync(fileInfo.FullName).ConfigureAwait(false);

            var fileName = fileInfo.Name;

            var relativeFolderPath =
                fileInfo.DirectoryName.StartsWith(baseDirectory.FullName)
                ? fileInfo.DirectoryName.Substring(baseDirectory.FullName.Length)
                : null;

            return
                new FileData(
                    content,
                    fileName: fileName,
                    relativeFolderPath: relativeFolderPath,
                    createdAt: fileInfo.CreationTimeUtc,
                    updatedAt: fileInfo.LastWriteTimeUtc);
        }

        private string GetTrimmedContent(string content)
        {
            var bom =
                _encoding.GetString(_encoding.GetPreamble()).ToCharArray();

            return content.TrimStart(bom);
        }

        private async Task<string> ReadTextAsync(string filePath)
        {
            using var sourceStream =
                new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 4096,
                    useAsync: true);

            var sb = new StringBuilder();

            var buffer = new byte[0x1000];

            int numRead;
            while ((numRead =
                await sourceStream
                    .ReadAsync(buffer, 0, buffer.Length)
                    .ConfigureAwait(false))
                    != 0)
            {
                var text = _encoding.GetString(buffer, 0, numRead);
                sb.Append(text);
            }

            return GetTrimmedContent(sb.ToString());
        }

    }
}
