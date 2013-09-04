using System;
using System.Collections.Generic;

namespace Blaven.DataSources
{
    public class DataSourceRefreshContext
    {
        public string BlogInfoChecksum { get; set; }

        public BlavenBlogSetting BlogSetting { get; set; }

        public bool ForceRefresh { get; set; }

        public DateTime? LastRefresh { get; set; }

        public IEnumerable<BlogPostMeta> ExistingBlogPostsMetas { get; set; }
    }
}