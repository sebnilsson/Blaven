using System;

namespace Blaven
{
    public class BlogRefreshResult
    {
        public BlogRefreshResult(
            string blogKey, BlogRefreshResultType resultType, bool hasBlogAnyData = true, Exception exception = null)
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

        public BlogRefreshResultType ResultType { get; set; }
    }
}