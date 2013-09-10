using System;

namespace Blaven.RavenDb
{
    public class RepositoryRefreshException : BlavenBlogException
    {
        public RepositoryRefreshException(string blogKey, Exception inner = null, string message = null)
            : base(blogKey, inner, message)
        {
        }
    }
}