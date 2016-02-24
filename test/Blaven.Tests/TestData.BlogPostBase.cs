using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.Synchronization;

namespace Blaven.Tests
{
    public static partial class TestData
    {
        public static IEnumerable<BlogPostBase> GetBlogPostBases(
            string blogKey,
            int blogPostCount,
            int blogPostStart = 0,
            bool isUpdate = false)
        {
            var blogPostBases =
                Enumerable.Range(blogPostStart, blogPostCount)
                    .Select(i => GetBlogPostBase(blogKey, i, isUpdate))
                    .ToList();
            return blogPostBases;
        }

        public static BlogPostBase GetBlogPostBase(string blogKey, int index = 0, bool isUpdate = false)
        {
            var blogPostBase = new BlogPostBase
                                   {
                                       Hash = HashUtility.GetBase64(index),
                                       SourceId =
                                           GetTestString(
                                               nameof(BlogPostBase.SourceId),
                                               blogKey,
                                               index,
                                               isUpdate: isUpdate)
                                   };

            blogPostBase.BlavenId = BlavenBlogPostBlavenIdProvider.GetId(blogPostBase.SourceId);

            return blogPostBase;
        }
    }
}