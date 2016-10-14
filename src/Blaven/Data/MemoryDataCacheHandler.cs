using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blaven.Data
{
    public class MemoryDataCacheHandler : IDataCacheHandler
    {
        public const int DefaultTimeoutMinutes = 15;

        internal readonly Dictionary<string, DateTime> DataUpdatedAt =
            new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

        public MemoryDataCacheHandler()
            : this(DefaultTimeoutMinutes)
        {
        }

        public MemoryDataCacheHandler(int timeoutMinutes)
        {
            if (timeoutMinutes <= 0)
            {
                string message =
                    $"Value for {nameof(timeoutMinutes)} must be a positive number. Provided value: '{timeoutMinutes}'.";
                throw new ArgumentOutOfRangeException(nameof(timeoutMinutes), message);
            }

            this.TimeoutMinutes = timeoutMinutes;
        }

        public int TimeoutMinutes { get; }

        public Task<bool> IsUpdated(DateTime now, string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            DateTime blogDataUpdated;

            lock (this.DataUpdatedAt)
            {
                if (!this.DataUpdatedAt.ContainsKey(blogKey))
                {
                    return Task.FromResult(false);
                }

                blogDataUpdated = this.DataUpdatedAt[blogKey];
            }

            var limitAt = now.AddMinutes(-this.TimeoutMinutes);

            bool isUpdated = (blogDataUpdated >= limitAt);
            return Task.FromResult(isUpdated);
        }

        public Task OnUpdated(DateTime now, string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            lock (this.DataUpdatedAt)
            {
                this.DataUpdatedAt[blogKey] = now;
            }

            return Task.CompletedTask;
        }
    }
}