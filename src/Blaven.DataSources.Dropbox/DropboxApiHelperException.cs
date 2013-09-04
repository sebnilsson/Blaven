using System;

namespace Blaven.DataSources.Dropbox
{
    public class DropboxApiHelperException : BlavenBlogException
    {
        public DropboxApiHelperException(BlavenBlogSetting bloggerSetting, Exception inner)
            : base(bloggerSetting.BlogKey, inner, "There was an error when retrieving data from Dropbox.")
        {
            this.BloggerSetting = bloggerSetting;
        }

        public BlavenBlogSetting BloggerSetting { get; private set; }
    }
}