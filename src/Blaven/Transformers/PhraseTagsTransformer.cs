﻿using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Blaven.Transformers
{
    public class PhraseTagsTransformer : IBlogPostTransformer
    {
        public Task<BlogPost> Transform(BlogPost blogPost)
        {
            if (blogPost.Content != null)
                blogPost.Content = EncodePreTags(blogPost.Content);

            return Task.FromResult(blogPost);
        }

        private static string EncodePreTags(string content)
        {
            var regex = new Regex(
                @"<(pre|code|samp)+.*?>(<code>)?(?<Content>.*?)(</code>)?</(pre|code|samp)+>",
                RegexOptions.Singleline);
            var matches = regex.Matches(content).OfType<Match>().ToList();

            if (!matches.Any())
                return content;

            var matchingCaptures =
                (from match in matches
                 from captures in match.Groups["Content"].Captures.OfType<Capture>()
                 select captures).Reverse();

            var parsedContent = content;

            foreach (var capture in matchingCaptures)
            {
                var index = capture.Index;
                var length = capture.Length;

                var encodedContent = WebUtility.HtmlEncode(capture.Value);
                parsedContent = parsedContent.Remove(index, length);
                parsedContent = parsedContent.Insert(index, encodedContent);
            }

            return parsedContent;
        }
    }
}