using System.Collections.Generic;
using System.Linq;

namespace Blaven
{
    internal class BlogRefreshServiceException : BlavenBlogException
    {
        public BlogRefreshServiceException(IList<BlogRefreshResult> criticalErrors, string message = null)
            : base(criticalErrors.Select(x => x.BlogKey), message: message)
        {
            this.CriticalErrors = criticalErrors;
        }

        public IEnumerable<BlogRefreshResult> CriticalErrors { get; private set; }
    }
}