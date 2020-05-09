using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.BlogSources.FileProviders
{
    public class FileDataResult
    {
        public FileDataResult(
            IEnumerable<FileData> metas,
            IEnumerable<FileData> posts)
        {
            if (metas is null)
                throw new ArgumentNullException(nameof(metas));
            if (posts is null)
                throw new ArgumentNullException(nameof(posts));

            Metas = metas.ToList();
            Posts = posts.ToList();
        }

        public IReadOnlyList<FileData> Metas { get; }

        public IReadOnlyList<FileData> Posts { get; }
    }
}
