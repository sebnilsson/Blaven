using System.Collections.Generic;
using System.Linq;

namespace Blaven.Testing
{
    public class StorageSyncRepositoryConfig
    {
        public StorageSyncRepositoryConfig(
            IEnumerable<BlogPost>? storagePosts = null)
        {
            StoragePosts = storagePosts ?? Enumerable.Empty<BlogPost>();
        }

        public IEnumerable<BlogPost> StoragePosts { get; }
    }
}
