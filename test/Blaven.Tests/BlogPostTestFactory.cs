using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.Tests
{
    public static class BlogPostTestFactory
    {
        public static BlogPost Create(
            int index = 1,
            Action<BlogPost>? config = null)
        {
            var blogPost = new BlogPost
            {
                Author = new BlogAuthor
                {
                    Id = $"{100 * index}",
                    ImageUrl = $"TEST_IMAGE_URL_{index}",
                    Name = $"TEST_NAME_{index}",
                    SourceId = $"TEST_SOURCE_ID_{index}",
                    Url = $"TEST_URL_{index}"
                },
                BlogKey = new BlogKey($"TEST_BLOG_KEY_{index}"),
                Content = $"TEST_CONTENT_{index}",
                Hash = $"TEST_HASH_{index}",
                Id = $"{index}",
                ImageUrl = $"TEST_IMAGE_URL_{index}",
                PublishedAt =
                    new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero)
                        .AddDays(index),
                Slug = $"TEST_SLUG_{index}",
                SourceId = $"TEST_SOURCE_ID_{index}",
                SourceUrl = $"TEST_SOURCE_URL_{index}",
                Summary = $"TEST_SUMMARY_{index}",
                Tags = new[]
                {
                    $"TEST_TAG_{index}",
                    $"TEST_TAG_{index + 1}",
                    $"TEST_TAG_{index + 2}"
                },
                Title = $"TEST_TITLE_{index}",
                UpdatedAt =
                    new DateTimeOffset(2020, 2, 3, 4, 5, 6, TimeSpan.Zero)
                        .AddDays(index),
            };

            config?.Invoke(blogPost);

            return blogPost;
        }

        public static IReadOnlyList<BlogPost> CreateList(
            params int[] indexes)
        {
            return
                indexes
                    .Select(x => Create(x))
                    .ToList();
        }
    }
}
