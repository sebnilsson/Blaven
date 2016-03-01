using System.Net;

using Xunit;

namespace Blaven.Transformers.Tests
{
    public class PhraseTagsTransformerTest
    {
        [Fact]
        public void Transform_BlogPostWithContentCodeBlockContainingLink_()
        {
            // Arrange
            var transformer = new PhraseTagsTransformer();
            var blogPost = new BlogPost
                               {
                                   Content =
                                       "<code><a href=\"http://msdn.microsoft.com/en-us/library/system.web.httprequest.applicationpath.aspx\">Request.ApplicationPath</a>:</code>"
                               };

            // Act
            transformer.Transform(blogPost);

            // Assert
            string decodedContent = WebUtility.HtmlDecode(blogPost.Content);

            bool contentContainsAHrefTag = blogPost.Content.Contains("<a href");
            bool decodedContentContainsAHrefTag = decodedContent.Contains("<a href");

            Assert.False(contentContainsAHrefTag);
            Assert.True(decodedContentContainsAHrefTag);
        }
    }
}