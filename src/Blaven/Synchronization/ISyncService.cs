using System;
using System.Threading.Tasks;

namespace Blaven.Synchronization
{
    public interface ISyncService
    {
        Task<SyncResult> Synchronize(
            BlogKey blogKey = default,
            DateTimeOffset? lastUpdatedAt = null);
    }
}
