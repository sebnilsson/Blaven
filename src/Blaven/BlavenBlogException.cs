using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven
{
    public class BlavenBlogException : BlavenException
    {
        public BlavenBlogException(string blogKey, Exception innerException = null, string message = null)
            : this(new[] { blogKey }, innerException, message)
        {
        }

        public BlavenBlogException(IEnumerable<string> blogKeys, Exception innerException = null, string message = null)
            : base(innerException, message)
        {
            this.BlogKeys = (blogKeys ?? Enumerable.Empty<string>()).ToList();
        }

        public IEnumerable<string> BlogKeys { get; private set; }
    }
}