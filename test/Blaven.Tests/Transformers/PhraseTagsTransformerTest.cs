using System.Net;
using Xunit;

namespace Blaven.Transformers.Tests
{
    public class PhraseTagsTransformerTest
    {
        [Fact]
        public void Transform_BlogPostWithContentCodeBlockContainingLink_ContainsLinks()
        {
            const string content =
                "<code><a href=\"http://msdn.microsoft.com/en-us/library/system.web.httprequest.applicationpath.aspx\">Request.ApplicationPath</a>:</code>";

            // Arrange
            var transformer = new PhraseTagsTransformer();
            var blogPost = new BlogPost
                           {
                               Content = content
                           };

            // Act
            transformer.Transform(blogPost);

            // Assert
            var decodedContent = WebUtility.HtmlDecode(blogPost.Content);

            var contentContainsAHrefTag = blogPost.Content.Contains("<a href");
            var decodedContentContainsAHrefTag = decodedContent.Contains("<a href");

            Assert.False(contentContainsAHrefTag);
            Assert.True(decodedContentContainsAHrefTag);
        }

        [Fact]
        public void Transform_BlogPostWithPreCodeBlockWithHtml_ContainsUnencodedHtml()
        {
            const string content = "<pre><code><head>\n<title>Title</title></code></pre>";
            const string expected = "<pre><code>&lt;head&gt;\n&lt;title&gt;Title&lt;/title&gt;</code></pre>";

            // Arrange
            var transformer = new PhraseTagsTransformer();
            var blogPost = new BlogPost
                           {
                               Content = content
                           };

            // Act
            transformer.Transform(blogPost);

            // Assert
            Assert.Equal(expected, blogPost.Content);
        }
    }
}