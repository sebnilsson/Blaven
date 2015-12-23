using System;

namespace Blaven.Data
{
    public interface IDataCacheHandler
    {
        int TimeoutMinutes { get; }

        bool IsUpdated(string blogKey, DateTime now);

        void OnUpdated(string blogKey, DateTime now);
    }
}