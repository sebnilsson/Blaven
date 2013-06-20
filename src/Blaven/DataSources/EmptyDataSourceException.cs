using System;

namespace Blaven.DataSources
{
    public class EmptyDataSourceException : DataSourceException
    {
        public EmptyDataSourceException(
            Type dataSourceType, string blogKey, Exception innerException = null, string message = null)
            : base(dataSourceType, blogKey, innerException, message)
        {

        }
    }
}