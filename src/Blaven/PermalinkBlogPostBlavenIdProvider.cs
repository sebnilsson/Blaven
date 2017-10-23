using System;

namespace Blaven
{
    public class PermalinkBlogPostBlavenIdProvider : IBlogPostBlavenIdProvider
    {
        private readonly bool _includePublishedDay;
        private readonly bool _includePublishedYearAndMonth;

        public PermalinkBlogPostBlavenIdProvider(bool includePublishedYearAndMonth, bool includePublishedDay)
        {
            _includePublishedYearAndMonth = includePublishedYearAndMonth;
            _includePublishedDay = includePublishedDay;
        }

        public static string GetBlogPostBlavenId(
            BlogPostHead blogPost,
            bool includePublishedYearAndMonth,
            bool includePublishedDay)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));
            if (blogPost.PublishedAt == null && (includePublishedYearAndMonth || includePublishedDay))
            {
                var message = $"{nameof(blogPost.PublishedAt)} cannot be null.";
                throw new ArgumentOutOfRangeException(nameof(blogPost), message);
            }
            if (string.IsNullOrWhiteSpace(blogPost.UrlSlug))
            {
                var message = $"{nameof(blogPost.UrlSlug)} cannot be null or empty.";
                throw new ArgumentOutOfRangeException(nameof(blogPost), message);
            }

            var format = GetFormat(includePublishedYearAndMonth, includePublishedDay);

            var blavenId = blogPost.PublishedAt != null
                               ? string.Format(
                                   format,
                                   blogPost.PublishedAt.Value.Year,
                                   blogPost.PublishedAt.Value.Month,
                                   blogPost.PublishedAt.Value.Day,
                                   blogPost.UrlSlug)
                               : blogPost.UrlSlug;
            return blavenId;
        }

        public string GetBlavenId(BlogPostHead blogPost)
        {
            var blavenId = GetBlogPostBlavenId(blogPost, _includePublishedYearAndMonth, _includePublishedDay);
            return blavenId;
        }

        private static string GetFormat(bool includePublishedYearAndMonth, bool includePublishedDay)
        {
            if (includePublishedYearAndMonth)
            {
                if (includePublishedDay)
                    return "{0:0000}/{1:00}/{2:00}/{3}";

                return "{0:0000}/{1:00}/{3}";
            }

            return "{3}";
        }
    }
}