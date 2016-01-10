using System;

using Google.Apis.Blogger.v3;

namespace Blaven.BlogSources.Blogger.Tests
{
    public class TestBloggerService : BloggerService
    {
        public TestBloggerService()
        {
        }

        public override PostsResource Posts
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}