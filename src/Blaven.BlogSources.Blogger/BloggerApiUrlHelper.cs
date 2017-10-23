using System;
using System.Net;

namespace Blaven.BlogSources.Blogger
{
    internal class BloggerApiUrlHelper
    {
        private const string BaseUrl = "https://www.googleapis.com/blogger/v3/blogs";
        private readonly string _apiKey;

        public BloggerApiUrlHelper(string apiKey)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public string GetBlogUrl(string blogId)
        {
            if (blogId == null)
                throw new ArgumentNullException(nameof(blogId));

            var url = AppendApiKey($"{BaseUrl}/{blogId}");
            return url;
        }

        public string GetPostsUrl(
            string blogId,
            DateTime? lastUpdateAt,
            string nextPageToken,
            int postsRequestMaxResults)
        {
            if (blogId == null)
                throw new ArgumentNullException(nameof(blogId));

            var url = AppendApiKey($"{BaseUrl}/{blogId}/posts/");

            var startDate = lastUpdateAt != null && lastUpdateAt > DateTime.MinValue
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

        private static string AppendParam(string url, string paramKey, string paramValue)
        {
            if (string.IsNullOrWhiteSpace(paramValue))
                return url;

            var separator = url.Contains("?") ? "&" : "?";

            var encodedKey = WebUtility.UrlEncode(paramKey);
            var encodedValue = WebUtility.UrlEncode(paramValue);

            var appendedUrl = $"{url}{separator}{encodedKey}={encodedValue}";
            return appendedUrl;
        }

        private string AppendApiKey(string url)
        {
            var apiKeyUrl = AppendParam(url, "key", _apiKey);
            return apiKeyUrl;
        }
    }
}