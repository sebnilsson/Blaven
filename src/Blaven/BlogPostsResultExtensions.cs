using Blaven.Transformers;

namespace Blaven
{
    public static class BlogPostsResultExtensions
    {
        public static BlogPostCollection ApplyTransformers(
            this BlogPostCollection posts, BlogPostTransformersCollection transformers)
        {
            if (transformers != null)
            {
                transformers.ApplyTransformers(posts);
            }
            return posts;
        }
    }
}