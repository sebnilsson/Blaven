using System.Net;

using Xunit;

namespace Blaven.Transformers.Tests
{
    public class PhraseTagsTransformerTest
    {
        [Fact]
        public void Transform_BlogPostWithContentCodeBlockContainingLink_ContainsLinks()
        {
            const string Content =
                "<code><a href=\"http://msdn.microsoft.com/en-us/library/system.web.httprequest.applicationpath.aspx\">Request.ApplicationPath</a>:</code>";

            // Arrange
            var transformer = new PhraseTagsTransformer();
            var blogPost = new BlogPost { Content = Content };

            // Act
            transformer.Transform(blogPost);

            // Assert
            string decodedContent = WebUtility.HtmlDecode(blogPost.Content);

            bool contentContainsAHrefTag = blogPost.Content.Contains("<a href");
            bool decodedContentContainsAHrefTag = decodedContent.Contains("<a href");

            Assert.False(contentContainsAHrefTag);
            Assert.True(decodedContentContainsAHrefTag);
        }

        [Fact]
        public void Transform_BlogPostWithPreCodeBlockWithHtml_ContainsUnencodedHtml()
        {
            const string Content = "<pre><code><head>\n<title>Title</title></code></pre>";
            const string Expected = "<pre><code>&lt;head&gt;\n&lt;title&gt;Title&lt;/title&gt;</code></pre>";

            // Arrange
            var transformer = new PhraseTagsTransformer();
            var blogPost = new BlogPost { Content = Content };

            // Act
            transformer.Transform(blogPost);

            // Assert
            Assert.Equal(Expected, blogPost.Content);
        }
    }
}