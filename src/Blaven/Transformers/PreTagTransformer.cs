using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Blaven.Transformers {
    public class PreTagTransformer : IBlogPostTransformer {
        public BlogPost Transform(BlogPost blogPost) {
            blogPost.Content = EncodePreTags(blogPost.Content);

            return blogPost;
        }

        private string EncodePreTags(string content) {
            if(!content.Contains("</pre>")) {
                return content;
            }

            var regex = new Regex(@"<pre.*?>(<code>)?(?<Content>.*?)(</code>)?</pre>", RegexOptions.Singleline);
            var matches = regex.Matches(content);
            var matchingCaptures = (from match in matches.OfType<Match>()
                                    from captures in match.Groups["Content"].Captures.OfType<Capture>()
                                    select captures).Reverse();
            
            string parsedContent = content;

            foreach(var capture in matchingCaptures) {
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
