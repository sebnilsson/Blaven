using System;

namespace Blaven.DataSources
{
    [Serializable]
    public class DataSourceException : BlavenBlogException
    {
        public DataSourceException(Type dataSourceType, string blogKey, Exception innerException = null, string message = null)
            : base(blogKey, innerException, message)
        {
            this.BlogKey = blogKey;
            this.DataSourceType = dataSourceType;
        }

        public Type DataSourceType { get; private set; }
    }
}