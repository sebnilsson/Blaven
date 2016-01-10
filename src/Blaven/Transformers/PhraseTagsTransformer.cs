﻿using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Blaven.Transformers
{
    public class PhraseTagsTransformer : IBlogPostTransformer
    {
        public BlogPost Transform(BlogPost blogPost)
        {
            if (blogPost.Content != null)
            {
                blogPost.Content = EncodePreTags(blogPost.Content);
            }

            return blogPost;
        }

        private static string EncodePreTags(string content)
        {
            var regex = new Regex(@"<(pre|code|samp)+.*?>(<code>)?(?<Content>.*?)(</code>)?</(pre|code|samp)+>", RegexOptions.Singleline);
            var matches = regex.Matches(content).OfType<Match>().ToList();

            if (!matches.Any())
            {
                return content;
            }

            var matchingCaptures = (from match in matches
                                    from captures in match.Groups["Content"].Captures.OfType<Capture>()
                                    select captures).Reverse();

            string parsedContent = content;

            foreach (var capture in matchingCaptures)
            {
                int index = capture.Index;
                int length = capture.Length;

                string encodedContent = HttpUtility.HtmlEncode(capture.Value);
                parsedContent = parsedContent.Remove(index, length);
                parsedContent = parsedContent.Insert(index, encodedContent);
            }

            return parsedContent;
        }
    }
}