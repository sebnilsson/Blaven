using System;
using System.Collections.Generic;

namespace Blaven.Data
{
    public class MemoryDataCacheHandler : IDataCacheHandler
    {
        public const int DefaultTimeoutMinutes = 15;

        internal readonly Dictionary<string, DateTime> DataUpdatedAt = new Dictionary<string, DateTime>();

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

        public bool IsUpdated(string blogKey, DateTime now)
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
                    return false;
                }

                blogDataUpdated = this.DataUpdatedAt[blogKey];
            }

            var limitAt = now.AddMinutes(-this.TimeoutMinutes);

            bool isUpdated = (blogDataUpdated >= limitAt);
            return isUpdated;
        }

        public void OnUpdated(string blogKey, DateTime now)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            lock (this.DataUpdatedAt)
            {
                this.DataUpdatedAt[blogKey] = now;
            }
        }
    }
}