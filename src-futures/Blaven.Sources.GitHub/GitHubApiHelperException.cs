using System;

namespace Blaven.DataSources.GitHub
{
    public class GitHubApiHelperException : BlavenBlogException
    {
        public GitHubApiHelperException(BlavenBlogSetting bloggerSetting, Exception inner)
            : base(bloggerSetting.BlogKey, inner, "There was an error when retrieving data from GitHub.")
        {
            this.BloggerSetting = bloggerSetting;
        }

        public BlavenBlogSetting BloggerSetting { get; private set; }
    }
}