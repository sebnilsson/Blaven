using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.Transformers
{
    public class BlogPostTransformersCollection
    {
        private static readonly Lazy<List<IBlogPostTransformer>> DefaultTransformers =
            new Lazy<List<IBlogPostTransformer>>(
                () =>
                    {
                        var defaultTransformers = new List<IBlogPostTransformer> { new PhraseTagsTransformer() };

                        var excludedTransformers = AppSettingsService.ExcludeTransformers.ToList();
                        if (excludedTransformers.Any())
                        {
                            RemoveTransformers(defaultTransformers, excludedTransformers.ToArray());
                        }

                        return defaultTransformers;
                    });

        public BlogPostTransformersCollection(IEnumerable<IBlogPostTransformer> transformers = null)
        {
            this.Transformers = (transformers != null)
                                    ? new List<IBlogPostTransformer>(transformers)
                                    : new List<IBlogPostTransformer>();
        }

        private List<IBlogPostTransformer> Transformers { get; set; }

        public static BlogPostTransformersCollection Default
        {
            get
            {
                return new BlogPostTransformersCollection(DefaultTransformers.Value);
            }
        }

        public BlogPost ApplyTransformers(BlogPost blogPost)
        {
            return blogPost != null
                       ? this.Transformers.Aggregate(blogPost, (post, transformer) => transformer.Transform(post))
                       : null;
        }

        public BlogPostCollection ApplyTransformers(BlogPostCollection blogPostsResult)
        {
            if (blogPostsResult == null)
            {
                return null;
            }

            ApplyTransformers(blogPostsResult.Posts);
            return blogPostsResult;
        }

        public IEnumerable<BlogPost> ApplyTransformers(IEnumerable<BlogPost> blogPosts)
        {
            return blogPosts.Select(this.ApplyTransformers).ToList();
        }

        public void RegisterTransformer(Func<IBlogPostTransformer> transformerConstructor)
        {
            var transformer = transformerConstructor();
            Transformers.Add(transformer);
        }

        public bool RemoveTransformer<TTransformer>() where TTransformer : IBlogPostTransformer
        {
            var transformer = this.Transformers.FirstOrDefault(x => x is TTransformer);
            return this.Transformers.Remove(transformer);
        }

        public bool RemoveTransformer(string transformerName)
        {
            return RemoveTransformers(this.Transformers, new[] { transformerName });
        }

        private static bool RemoveTransformers(
            ICollection<IBlogPostTransformer> list, IEnumerable<string> transformerNames)
        {
            var names = (transformerNames ?? Enumerable.Empty<string>()).ToList();

            var removeTransformers = (from transformer in list
                                      let type = transformer.GetType()
                                      let typeFullName = type.FullName.ToLowerInvariant()
                                      where
                                          type.Assembly.FullName.StartsWith("Blaven, ")
                                          && (names.Contains(typeFullName) || names.Any(typeFullName.EndsWith))
                                      select transformer).ToList();

            removeTransformers.ForEach(x => list.Remove(x));
            return removeTransformers.Any();
        }
    }
}