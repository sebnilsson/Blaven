using System;

namespace Blaven.DataSources
{
    public class RefreshSynchronizerResult
    {
        public RefreshSynchronizerResult(
            string blogKey, RefreshSynchronizerResultType resultType, bool hasBlogAnyData = true, Exception exception = null)
        {
            this.BlogKey = blogKey;
            this.Exception = exception;
            this.HasBlogAnyData = hasBlogAnyData;
            this.ResultType = resultType;
        }

        public string BlogKey { get; private set; }

        public TimeSpan ElapsedTime { get; set; }

        public Exception Exception { get; private set; }

        public bool HasBlogAnyData { get; private set; }

        public RefreshSynchronizerResultType ResultType { get; set; }

        public DataSourceRefreshResult RefreshResult { get; set; }
    }
}