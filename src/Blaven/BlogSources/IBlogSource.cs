using System;
using System.Threading.Tasks;

namespace Blaven.BlogSources
{
    public interface IBlogSource
    {
        Task<BlogSourceData> GetData(
            BlogKey blogKey,
            DateTimeOffset? updatedAfter = null);
    }
}
