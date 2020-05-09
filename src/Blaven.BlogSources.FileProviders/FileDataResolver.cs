using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Blaven.BlogSources.FileProviders
{
    internal class FileDataResolver
    {
        private readonly DirectoryInfo _baseDirectory;
        private readonly Encoding _encoding;

        public FileDataResolver(DirectoryInfo baseDirectory, Encoding encoding)
        {
            _baseDirectory = baseDirectory
                ?? throw new ArgumentNullException(nameof(baseDirectory));
            _encoding = encoding
                ?? throw new ArgumentNullException(nameof(encoding));
        }

        public async Task<FileData> GetFileData(
            FileInfo fileInfo)
        {
            if (fileInfo is null)
                throw new ArgumentNullException(nameof(fileInfo));

            var content = await ReadTextAsync(fileInfo.FullName, _encoding);

            var fileName = fileInfo.Name;

            var baseDirectoryName = _baseDirectory?.Name;

            var folderName =
                fileInfo.Directory.Name != baseDirectoryName
                ? fileInfo.Directory.Name
                : null;

            return
                new FileData(
                    content,
                    fileName,
                    folderName,
                    fileInfo.CreationTimeUtc);
        }

        private static async Task<string> ReadTextAsync(
            string filePath,
            Encoding encoding)
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
            while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                var text = encoding.GetString(buffer, 0, numRead);
                sb.Append(text);
            }

            return sb.ToString();
        }

    }
}
