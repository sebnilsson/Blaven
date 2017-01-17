using System;
using System.Collections.Generic;

namespace Blaven.Transformers
{
    public class BlogTransformersProvider
    {
        public BlogTransformersProvider()
        {
            this.Transformers = new List<IBlogPostTransformer>();
        }

        public List<IBlogPostTransformer> Transformers { get; }

        public BlogPost ApplyTransformers(BlogPost blogPost)
        {
            if (blogPost == null)
            {
                throw new ArgumentNullException(nameof(blogPost));
            }

            foreach (var transformer in this.Transformers)
            {
                transformer.Transform(blogPost);
            }

            return blogPost;
        }

        public static IEnumerable<IBlogPostTransformer> GetDefaultTransformers()
        {
            yield return new PhraseTagsTransformer();
        }
    }
}