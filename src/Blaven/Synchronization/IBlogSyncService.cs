using System;
using System.Threading.Tasks;

namespace Blaven.Synchronization
{
    public interface IBlogSyncService
    {
        Task<SyncResult> Synchronize(
            BlogKey blogKey = default,
            DateTimeOffset? updatedAfter = null);
    }
}
