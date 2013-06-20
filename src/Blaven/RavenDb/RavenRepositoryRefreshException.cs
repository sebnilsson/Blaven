using System;

namespace Blaven.RavenDb
{
    public class RavenRepositoryRefreshException : BlavenBlogException
    {
        public RavenRepositoryRefreshException(string blogKey, Exception inner = null, string message = null)
            : base(blogKey, inner, message)
        {
            this.BlogKey = blogKey;
        }
    }
}