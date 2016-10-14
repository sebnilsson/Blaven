using System;
using System.Threading.Tasks;

namespace Blaven.Data
{
    public interface IDataCacheHandler
    {
        int TimeoutMinutes { get; }

        Task<bool> IsUpdated(DateTime now, string blogKey);

        Task OnUpdated(DateTime now, string blogKey);
    }
}