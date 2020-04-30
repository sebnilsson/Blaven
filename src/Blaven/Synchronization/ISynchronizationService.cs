using System;
using System.Threading.Tasks;

namespace Blaven.Synchronization
{
    public interface ISynchronizationService
    {
        Task<SynchronizationResult> Synchronize(
            BlogKey blogKey = default,
            DateTimeOffset? lastUpdatedAt = null);
    }
}
