using System.Collections.Generic;
using System.Linq;

namespace Blaven.DataSources
{
    public abstract class DataSourceBase : IDataSource
    {
        public IEnumerable<BlogPostMeta> GetModifiedPostMetas(
            DataSourceRefreshContext refreshInfo, IEnumerable<BlogPostMeta> blogPostsMetas)
        {
            var existingblogPostsMetas = refreshInfo.ExistingBlogPostsMetas.ToList();
            var modifiedBlogPosts = from meta in blogPostsMetas
                                    let existing =
                                        existingblogPostsMetas.FirstOrDefault(
                                            x => x.Id == meta.Id && x.DataSourceId == meta.DataSourceId)
                                    where existing == null || existing.Checksum != meta.Checksum
                                    select meta;

            return modifiedBlogPosts;
        }

        public IEnumerable<string> GetDeletedPostIds(IEnumerable<string> repositoryIds, IEnumerable<string> allStoreIds)
        {
            return repositoryIds.Where(x => !allStoreIds.Contains(x));
        }

        public abstract DataSourceRefreshResult Refresh(DataSourceRefreshContext refreshInfo);
    }
}