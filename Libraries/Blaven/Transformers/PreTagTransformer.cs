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

            string parsedContent = content;

            int openTagStartIndex = parsedContent.IndexOf("<pre");
            int openTagEndIndex = (openTagStartIndex >= 0) ? parsedContent.IndexOf(">", openTagStartIndex) : -1;
            int closeTagStartIndex = (openTagStartIndex >= 0) ? parsedContent.IndexOf("</pre>", openTagEndIndex) : -1;

            while(openTagStartIndex >= 0 && closeTagStartIndex >= 0) {
                if(parsedContent.Substring(openTagEndIndex + 1, 5) == "<code") {
                    openTagStartIndex = openTagEndIndex + 1;
                    openTagEndIndex = parsedContent.IndexOf(">", openTagStartIndex);
                    closeTagStartIndex = parsedContent.IndexOf("</code>", openTagEndIndex);
                }

                string preContent = content.Substring(openTagEndIndex + 1, closeTagStartIndex - openTagEndIndex - 1);

                string encodedPre = HttpUtility.HtmlEncode(preContent);

                parsedContent = parsedContent.Remove(openTagEndIndex + 1, closeTagStartIndex - openTagEndIndex - 1);
                parsedContent = parsedContent.Insert(openTagEndIndex + 1, encodedPre);

                openTagStartIndex = content.IndexOf("<pre", closeTagStartIndex);
                if(openTagStartIndex < 0) {
                    break;
                }
                openTagEndIndex = content.IndexOf(">", openTagStartIndex);
                closeTagStartIndex = content.IndexOf("</pre>", openTagEndIndex);
            }

            return parsedContent;
        }
    }
}
