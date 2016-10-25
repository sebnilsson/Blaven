using System;
using System.Net;

namespace Blaven.BlogSources.Blogger
{
    internal class BloggerApiUrlHelper
    {
        private const string BaseUrl = "https://www.googleapis.com/blogger/v3/blogs";

        private readonly string apiKey;

        public BloggerApiUrlHelper(string apiKey)
        {
            if (apiKey == null)
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            this.apiKey = apiKey;
        }

        public string GetBlogUrl(string blogId)
        {
            if (blogId == null)
            {
                throw new ArgumentNullException(nameof(blogId));
            }

            string url = this.AppendApiKey($"{BaseUrl}/{blogId}");
            return url;
        }

        public string GetPostsUrl(
            string blogId,
            DateTime? lastUpdateAt,
            string nextPageToken,
            int postsRequestMaxResults)
        {
            if (blogId == null)
            {
                throw new ArgumentNullException(nameof(blogId));
            }

            string url = this.AppendApiKey($"{BaseUrl}/{blogId}/posts/");

            string startDate = (lastUpdateAt != null && lastUpdateAt > DateTime.MinValue)
                                   ? lastUpdateAt.Value.ToRfc3339String()
                                   : null;

            url = AppendParam(url, "startDate", startDate);

            //url = AppendParam(url, "orderBy", "updated");

            url = AppendParam(url, "maxResults", Convert.ToString(postsRequestMaxResults));

            url = AppendParam(url, "fetchImages", "false");

            url = AppendParam(url, "prettyPrint", "false");

            url = AppendParam(
                url,
                "fields",
                "nextPageToken,items(id,etag,published,updated,url,title,content,author,labels)");

            url = AppendParam(url, "pageToken", nextPageToken);

            return url;
        }

        private string AppendApiKey(string url)
        {
            string apiKeyUrl = AppendParam(url, "key", this.apiKey);
            return apiKeyUrl;
        }

        private static string AppendParam(string url, string paramKey, string paramValue)
        {
            if (string.IsNullOrWhiteSpace(paramValue))
            {
                return url;
            }

            string separator = url.Contains("?") ? "&" : "?";

            string encodedKey = WebUtility.UrlEncode(paramKey);
            string encodedValue = WebUtility.UrlEncode(paramValue);

            string appendedUrl = $"{url}{separator}{encodedKey}={encodedValue}";
            return appendedUrl;
        }
    }
}