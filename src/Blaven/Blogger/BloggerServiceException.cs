using System;

namespace Blaven.Blogger
{
    [Serializable]
    public class BloggerServiceException : BlavenBlogException
    {
        public BloggerServiceException(BloggerSetting bloggerSetting, Exception inner)
            : base(bloggerSetting.BlogKey, "There was an error when retrieving data from Blogger.", inner)
        {
            this.BloggerSetting = bloggerSetting;
        }

        public BloggerSetting BloggerSetting { get; private set; }
    }
}