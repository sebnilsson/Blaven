using System;

namespace Blaven.DataSources.Blogger
{
    [Serializable]
    public class BloggerApiException : BlavenBlogException
    {
        public BloggerApiException(BlavenBlogSetting bloggerSetting, Exception inner)
            : base(bloggerSetting.BlogKey, inner, "There was an error when retrieving data from Blogger.")
        {
            this.BloggerSetting = bloggerSetting;
        }

        public BlavenBlogSetting BloggerSetting { get; private set; }
    }
}