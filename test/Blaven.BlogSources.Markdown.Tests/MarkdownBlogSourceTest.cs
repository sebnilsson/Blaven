using System;
using System.Linq;
using System.Threading.Tasks;
using Blaven.BlogSources.FileProviders;
using Moq;
using Xunit;

namespace Blaven.BlogSources.Markdown.Tests
{
    public class MarkdownBlogSourceTest
    {
        private const string TestFileName2 = "TEST_FILE_NAME2";

        private static readonly BlogKey s_blogKey1 = new BlogKey("BLOG_KEY_1");
        private static readonly BlogKey s_blogKey2 = new BlogKey("BLOG_KEY_2");

        [Fact]
        public async Task GetMeta_FullJsonData_ReturnFullData()
        {
            // Arrange
            var markdownBlogSource = GetMarkdownBlogSource();

            // Act
            var data = await markdownBlogSource.GetData(s_blogKey1);
            var meta = data.Meta;

            // Assert
            Assert.NotNull(meta);
            Assert.Equal(s_blogKey1, meta?.BlogKey);
            Assert.Equal("TEST_DESCRIPTION", meta?.Description);
            Assert.Equal("TEST_ID", meta?.Id);
            Assert.Equal("TEST_NAME", meta?.Name);
            Assert.Equal(new DateTime(2020, 1, 2), meta?.PublishedAt);
            Assert.Equal("TEST_SOURCE_ID", meta?.SourceId);
            Assert.Equal(new DateTime(2020, 2, 1), meta?.UpdatedAt);
            Assert.Equal("TEST_URL", meta?.Url);
        }

        [Fact]
        public async Task GetMeta_BlogKeyFromFolderName_ReturnDataFromFileData()
        {
            // Arrange
            var markdownBlogSource = GetMarkdownBlogSource();

            // Act
            var data = await markdownBlogSource.GetData(s_blogKey2);
            var meta = data.Meta;

            // Assert
            Assert.NotNull(meta);
            Assert.Equal(s_blogKey2, meta?.BlogKey);
            Assert.Equal(new DateTime(2020, 3, 3), meta?.PublishedAt);
        }

        [Fact]
        public async Task GetPosts_FullYamlData_ReturnFullData()
        {
            // Arrange
            var markdownBlogSource = GetMarkdownBlogSource();

            // Act
            var data = await markdownBlogSource.GetData("test_blog_key_1");
            var posts = data.Posts;
            var post = posts.FirstOrDefault(x => x.Id == "TEST_ID_1");

            // Assert
            var expectTag =
                Enumerable.Range(1, 3).Select(x => $"TEST_TAG_{x}").ToList();
            var expectedContent =
                @"<p><strong>Bold</strong> <em>Italic</em> </p>
<div>TEST_DIV</div>".Replace(Environment.NewLine, "\n");

            Assert.NotNull(post);
            Assert.Equal("test_blog_key_1", post.BlogKey);
            Assert.Equal("TEST_AUTHOR_ID_1", post.Author.Id);
            Assert.Equal("TEST_AUTHOR_IMAGE_URL_1", post.Author.ImageUrl);
            Assert.Equal("TEST_AUTHOR_NAME_1", post.Author.Name);
            Assert.Equal("TEST_AUTHOR_SOURCE_ID_1", post.Author.SourceId);
            Assert.Equal("TEST_AUTHOR_URL_1", post.Author.Url);
            Assert.Equal("TEST_HASH_1", post.Hash);
            Assert.Equal("TEST_ID_1", post.Id);
            Assert.Equal("TEST_IMAGE_URL_1", post.ImageUrl);
            Assert.Equal(new DateTime(2020, 1, 2), post.PublishedAt);
            Assert.Equal("TEST_SLUG_1", post.Slug);
            Assert.Equal("TEST_SOURCE_ID_1", post.SourceId);
            Assert.Equal("TEST_SOURCE_URL_1", post.SourceUrl);
            Assert.Equal("TEST_SUMMARY_1", post.Summary);
            Assert.Equal("TEST_TITLE_1", post.Title);
            Assert.Equal(new DateTime(2020, 2, 4, 4, 5, 6), post.UpdatedAt);

            Assert.Equal(expectedContent, post.Content.Trim());

            var tagsSequenceEquals = expectTag.SequenceEqual(post.Tags);
            Assert.True(tagsSequenceEquals);
        }

        [Fact]
        public async Task GetPosts_BlogKeyFromFolderName_ReturnDataFromFileData()
        {
            // Arrange
            var markdownBlogSource = GetMarkdownBlogSource();

            // Act
            var data = await markdownBlogSource.GetData(s_blogKey2);
            var posts = data.Posts;
            var post = posts.FirstOrDefault();

            // Assert
            Assert.NotNull(post);
            Assert.Equal(s_blogKey2, post.BlogKey);
            Assert.Equal(TestFileName2, post.Id);
            Assert.Equal(TestFileName2, post.Slug);
            Assert.Equal(new DateTime(2020, 3, 5), post.PublishedAt);
        }

        private IBlogSource GetMarkdownBlogSource()
        {
            var metaJsonFiles = new[]
            {
                new FileData(
                    Resources.BlogMeta1,
                    fileName: "TEST_FILE_NAME1",
                    relativeFolderPath: "FOLDER_NAME1",
                    createdAt: new DateTime(2020, 3, 2)),
                new FileData(
                    Resources.BlogMeta2,
                    fileName: "TEST_FILE_NAME2",
                    relativeFolderPath: "/TEST/.BLOG_KEY_2/meta/",
                    createdAt: new DateTime(2020, 3, 3))
            };
            var postMarkdownFiles = new[]
            {
                new FileData(
                    Resources.BlogPost1,
                    fileName: "TEST_FILE_NAME1",
                    relativeFolderPath: "FOLDER_NAME1",
                    createdAt: new DateTime(2020, 3, 4)),
                new FileData(
                    Resources.BlogPost2,
                    fileName: TestFileName2,
                    relativeFolderPath: "/TEST/.BLOG_KEY_2/posts/",
                    createdAt: new DateTime(2020, 3, 5))
            };

            var mockFileDataProvider = new Mock<IFileDataProvider>();

            mockFileDataProvider
                .Setup(x => x.GetFileData())
                .Returns(() =>
                {
                    var result = new FileDataResult(
                    metas: metaJsonFiles,
                    posts: postMarkdownFiles);

                    return Task.FromResult(result);
                });

            return new MarkdownBlogSource(mockFileDataProvider.Object);
        }
    }
}
