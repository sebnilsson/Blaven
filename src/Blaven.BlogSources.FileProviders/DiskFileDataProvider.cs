using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaven.BlogSources.FileProviders
{
    public class DiskFileDataProvider : IFileDataProvider
    {
        private readonly DirectoryInfo _baseDirectory;
        private readonly DirectoryResolver _directoryResolver;
        private readonly FileResolver _fileResolver;
        private readonly IReadOnlyList<string> _metaExtensions;
        private readonly IReadOnlyList<string> _postExtensions;

        public DiskFileDataProvider(
            string baseDirectory,
            bool recursive,
            IEnumerable<string> metaExtensions,
            IEnumerable<string> postExtensions)
            : this(
                  baseDirectory,
                  recursive: recursive,
                  encoding: Encoding.UTF8,
                  metaExtensions: metaExtensions,
                  postExtensions: postExtensions)
        {
        }

        public DiskFileDataProvider(
            string baseDirectory,
            bool recursive,
            Encoding encoding,
            IEnumerable<string> metaExtensions,
            IEnumerable<string> postExtensions)
        {
            if (baseDirectory is null)
                throw new ArgumentNullException(nameof(baseDirectory));
            if (encoding is null)
                throw new ArgumentNullException(nameof(encoding));
            if (metaExtensions is null)
                throw new ArgumentNullException(nameof(metaExtensions));
            if (postExtensions is null)
                throw new ArgumentNullException(nameof(postExtensions));

            _baseDirectory = new DirectoryInfo(baseDirectory);

            _metaExtensions = EnsureFileExtensionFormat(metaExtensions);
            _postExtensions = EnsureFileExtensionFormat(postExtensions);

            var extensions = _metaExtensions.Concat(_postExtensions).ToList();

            _directoryResolver =
                new DirectoryResolver(_baseDirectory, recursive);
            _fileResolver =
                new FileResolver(_baseDirectory, extensions, encoding);
        }

        public async Task<FileDataResult> GetFileData()
        {
            EnsureBaseDirectoryExists();

            var directories = _directoryResolver.GetDirectories();

            var files = await _fileResolver.GetFiles(directories);

            return GetFileDataResult(files);
        }

        private void EnsureBaseDirectoryExists()
        {
            if (!_baseDirectory.Exists)
            {
                throw new DirectoryNotFoundException(
                    $"Directory does not exist at path '{_baseDirectory.FullName}'.");
            }
        }

        private static IReadOnlyList<string> EnsureFileExtensionFormat(
            IEnumerable<string> extensions)
        {
            return
                extensions
                    .Select(x => x.TrimStart('.'))
                    .Select(x => $".{x}")
                    .ToList();
        }

        private IReadOnlyList<FileData> FilterByExtensions(
            IEnumerable<FileData> fileDatas,
            IEnumerable<string> extensions)
        {
            return
                fileDatas
                    .Where(x =>
                        extensions.Contains(
                            x.FileExtension,
                            StringComparer.InvariantCultureIgnoreCase))
                    .ToList();
        }

        private FileDataResult GetFileDataResult(IReadOnlyList<FileData> files)
        {
            var metas = FilterByExtensions(files, _metaExtensions);
            var posts = FilterByExtensions(files, _postExtensions);

            return new FileDataResult(metas: metas, posts: posts);
        }
    }
}
