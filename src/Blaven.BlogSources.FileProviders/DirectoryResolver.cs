using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Blaven.BlogSources.FileProviders
{
    internal class DirectoryResolver
    {
        private readonly DirectoryInfo _baseDirectory;
        private readonly bool _recursive;

        public DirectoryResolver(DirectoryInfo baseDirectory, bool recursive)
        {
            if (baseDirectory is null)
                throw new ArgumentNullException(nameof(baseDirectory));

            _baseDirectory = baseDirectory;
            _recursive = recursive;
        }

        public IReadOnlyList<DirectoryInfo> GetDirectories()
        {
            return GetDirectoriesInternal().ToList();
        }

        private IEnumerable<DirectoryInfo> GetDirectoriesInternal()
        {
            yield return _baseDirectory;

            if (!_recursive)
            {
                yield break;
            }

            var subDirectories = GetSubDirectories(_baseDirectory);

            foreach (var subDirectory in subDirectories)
            {
                yield return subDirectory;
            }
        }

        private static IEnumerable<DirectoryInfo> GetSubDirectories(
            DirectoryInfo directory)
        {
            var subDirectories = directory.EnumerateDirectories();

            foreach (var subDirectory in subDirectories)
            {
                yield return subDirectory;

                var subSubDirectories = GetSubDirectories(subDirectory);

                foreach (var subSubDirectory in subSubDirectories)
                {
                    yield return subSubDirectory;
                }
            }
        }
    }
}
