using System.Collections.Generic;
using System.Linq;

namespace Blaven.DataSources
{
    public class RefreshSynchronizerServiceException : BlavenBlogException
    {
        public RefreshSynchronizerServiceException(
            IList<RefreshSynchronizerResult> criticalErrors, string message = null)
            : base(criticalErrors.Select(x => x.BlogKey), message: message)
        {
            this.CriticalErrors = criticalErrors;
        }

        public IEnumerable<RefreshSynchronizerResult> CriticalErrors { get; private set; }
    }
}