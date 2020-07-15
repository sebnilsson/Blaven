using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Blaven.Storage.Queries
{
    internal static class SearchHelper
    {
        public static bool HasMatch(BlogPost blogPost, string searchText)
        {
            if (blogPost is null)
                throw new ArgumentNullException(nameof(blogPost));
            if (searchText is null)
                throw new ArgumentNullException(nameof(searchText));

            var regex = GetSearchRegex(searchText);

            return
                regex.IsMatch(blogPost.Content)
                || regex.IsMatch(blogPost.Summary)
                || regex.IsMatch(blogPost.Title)
                || blogPost.Tags.Any(x => regex.IsMatch(x));
        }

        private static Regex GetSearchRegex(string searchText)
        {
            var escapedSearchText = Regex.Escape(searchText);

            string pattern = $"(?<!\\w){escapedSearchText}";

            return new Regex(pattern, RegexOptions.IgnoreCase);
        }
    }
}
