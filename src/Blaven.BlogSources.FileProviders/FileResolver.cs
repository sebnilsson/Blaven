﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaven.BlogSources.FileProviders
{
    internal class FileResolver
    {
        private readonly FileDataResolver _fileDataResolver;
        private readonly IReadOnlyList<string> _extensions;

        public FileResolver(
            DirectoryInfo baseDirectory,
            IReadOnlyList<string> extensions,
            Encoding encoding)
        {
            if (baseDirectory is null)
                throw new ArgumentNullException(nameof(baseDirectory));
            if (extensions is null)
                throw new ArgumentNullException(nameof(extensions));
            if (encoding is null)
                throw new ArgumentNullException(nameof(encoding));

            _fileDataResolver = new FileDataResolver(baseDirectory, encoding);
            _extensions = extensions;
        }

        public async Task<IReadOnlyList<FileData>> GetFiles(
            IReadOnlyList<DirectoryInfo> directories)
        {
            if (directories is null)
                throw new ArgumentNullException(nameof(directories));

            var distinctDirectories =
                directories
                    .GroupBy(x => x.FullName)
                    .Select(x => x.First());

            var fileDataTasks =
                distinctDirectories
                    .Select(x => GetFileDatas(x))
                    .ToList();

            await Task.WhenAll(fileDataTasks);

            var fileDatas =
                fileDataTasks
                    .SelectMany(x => x.Result)
                    .Where(x => x.Exists)
                    .ToList();

            return fileDatas;
        }

        private async Task<IReadOnlyList<FileData>> GetFileDatas(
            DirectoryInfo directory)
        {
            var files = directory.EnumerateFiles();

            var fileDataTasks =
                files
                    .Where(x => _extensions.Contains(
                        x.Extension,
                        StringComparer.InvariantCultureIgnoreCase))
                    .Select(x => GetFileData(x))
                    .ToList();

            await Task.WhenAll(fileDataTasks);

            return fileDataTasks.Select(x => x.Result).ToList();
        }

        private async Task<FileData> GetFileData(FileInfo fileInfo)
        {
            try
            {
                return await _fileDataResolver.GetFileData(fileInfo);
            }
            catch
            {
                return default;
            }
        }
    }
}
