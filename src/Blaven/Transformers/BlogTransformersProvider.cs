using System;
using System.Collections.Generic;

namespace Blaven.Transformers
{
    public class BlogTransformersProvider
    {
        public BlogTransformersProvider()
        {
            Transformers = new List<IBlogPostTransformer>();
        }

        public List<IBlogPostTransformer> Transformers { get; }

        public static IEnumerable<IBlogPostTransformer> GetDefaultTransformers()
        {
            yield return new PhraseTagsTransformer();
        }

        public BlogPost ApplyTransformers(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            foreach (var transformer in Transformers)
                transformer.Transform(blogPost);

            return blogPost;
        }
    }
}