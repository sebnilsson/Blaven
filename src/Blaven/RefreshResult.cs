using System;

namespace Blaven
{
    public class RefreshResult
    {
        public RefreshResult(string blogKey, TimeSpan elapsedTime, RefreshType refreshType)
        {
            this.BlogKey = blogKey;
            this.ElapsedTime = elapsedTime;
            this.RefreshType = refreshType;
        }

        public string BlogKey { get; private set; }

        public TimeSpan ElapsedTime { get; private set; }

        public RefreshType RefreshType { get; set; }
    }
}