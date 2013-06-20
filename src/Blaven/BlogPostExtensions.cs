using Blaven.Transformers;

namespace Blaven
{
    public static class BlogPostExtensions
    {
        public static BlogPost ApplyTransformers(this BlogPost blogPost, BlogPostTransformersCollection transformers)
        {
            if (transformers != null)
            {
                transformers.ApplyTransformers(blogPost);
            }
            return blogPost;
        }
    }
}