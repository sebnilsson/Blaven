using System;

namespace Blaven.RavenDb
{
    public class RepositoryRefreshServiceException : BlavenBlogException
    {
        public RepositoryRefreshServiceException(string blogKey, Exception inner = null, string message = null)
            : base(blogKey, inner, message)
        {
        }
    }
}