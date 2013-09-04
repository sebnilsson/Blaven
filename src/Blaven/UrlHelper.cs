using System;

namespace Blaven
{
    public static class UrlHelper
    {
        public static string GetAbsoluteUrl(string fullUrl, string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl) || !Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute))
            {
                return fullUrl;
            }

            string relative = GetRelativeUrl(fullUrl);
            var baseUri = new Uri(baseUrl);

            Uri result;
            if (Uri.TryCreate(baseUri, relative, out result))
            {
                return result.ToString();
            }

            return fullUrl;
        }

        public static string GetRelativeUrl(string fullUrl)
        {
            if (string.IsNullOrWhiteSpace(fullUrl) || !Uri.IsWellFormedUriString(fullUrl, UriKind.RelativeOrAbsolute))
            {
                return string.Empty;
            }
            var uri = new Uri(fullUrl);

            var relative = uri.LocalPath;
            var index = relative.LastIndexOf(".html", StringComparison.InvariantCultureIgnoreCase);
            if (index < 0)
            {
                index = relative.LastIndexOf(".asp", StringComparison.InvariantCultureIgnoreCase);
            }

            return relative.Substring(0, index);
        }
    }
}