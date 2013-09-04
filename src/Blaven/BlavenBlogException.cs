using System;

namespace Blaven
{
    public class BlavenBlogException : BlavenException
    {
        protected internal BlavenBlogException(string blogKey)
        {
            this.BlogKey = blogKey;
        }

        protected internal BlavenBlogException(string blogKey, Exception inner = null, string message = null)
            : base(inner, message)
        {
            this.BlogKey = blogKey;
        }

        public string BlogKey { get; internal set; }
    }
}