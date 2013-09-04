using System;

namespace Blaven.DataSources.Blogger
{
    public class BloggerApiHelperException : BlavenBlogException
    {
        public BloggerApiHelperException(BlavenBlogSetting bloggerSetting, Exception inner)
            : base(bloggerSetting.BlogKey, inner, "There was an error when retrieving data from Blogger.")
        {
            this.BloggerSetting = bloggerSetting;
        }

        public BlavenBlogSetting BloggerSetting { get; private set; }
    }
}