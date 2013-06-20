using System.Collections.Generic;
using System.Linq;

namespace Blaven.DataSources
{
    public class DataSourceRefresherException : BlavenBlogsException
    {
        public DataSourceRefresherException(IEnumerable<RefreshResult> results, string message = null)
            : base(results.Select(x => x.BlogKey), message: message)
        {
            this.RefreshResults = results;
        }

        public IEnumerable<RefreshResult> RefreshResults { get; private set; }
    }
}