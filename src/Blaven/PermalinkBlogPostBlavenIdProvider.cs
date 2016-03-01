using System;

namespace Blaven
{
    public class PermalinkBlogPostBlavenIdProvider : IBlogPostBlavenIdProvider
    {
        private readonly bool includePublishedYearAndMonth;

        private readonly bool includePublishedDay;

        public PermalinkBlogPostBlavenIdProvider(bool includePublishedYearAndMonth, bool includePublishedDay)
        {
            this.includePublishedYearAndMonth = includePublishedYearAndMonth;
            this.includePublishedDay = includePublishedDay;
        }

        public string GetBlavenId(BlogPostHead blogPost)
        {
            string blavenId = GetBlogPostBlavenId(blogPost, this.includePublishedYearAndMonth, this.includePublishedDay);
            return blavenId;
        }

        public static string GetBlogPostBlavenId(
            BlogPostHead blogPost,
            bool includePublishedYearAndMonth,
            bool includePublishedDay)
        {
            if (blogPost == null)
            {
                throw new ArgumentNullException(nameof(blogPost));
            }
            if (blogPost.PublishedAt == null && (includePublishedYearAndMonth || includePublishedDay))
            {
                string message = $"{nameof(blogPost.PublishedAt)} cannot be null.";
                throw new ArgumentOutOfRangeException(nameof(blogPost), message);
            }
            if (string.IsNullOrWhiteSpace(blogPost.UrlSlug))
            {
                string message = $"{nameof(blogPost.UrlSlug)} cannot be null or empty.";
                throw new ArgumentOutOfRangeException(nameof(blogPost), message);
            }

            string format = GetFormat(includePublishedYearAndMonth, includePublishedDay);

            string blavenId = (blogPost.PublishedAt != null)
                                  ? string.Format(
                                      format,
                                      blogPost.PublishedAt.Value.Year,
                                      blogPost.PublishedAt.Value.Month,
                                      blogPost.PublishedAt.Value.Day,
                                      blogPost.UrlSlug)
                                  : blogPost.UrlSlug;
            return blavenId;
        }

        private static string GetFormat(bool includePublishedYearAndMonth, bool includePublishedDay)
        {
            if (includePublishedYearAndMonth)
            {
                if (includePublishedDay)
                {
                    return "{0:0000}/{1:00}/{2:00}/{3}";
                }

                return "{0:0000}/{1:00}/{3}";
            }

            return "{3}";
        }
    }
}