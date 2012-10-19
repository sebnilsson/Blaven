using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.Transformers {
    public class TransformersService {
        static TransformersService() {
            SetDefaultTransformers();

            var excludedTransformers = AppSettingsService.ExcludeTransformers;
            if(!excludedTransformers.Any()) {
                return;
            }

            RemoveTransformers(excludedTransformers.ToArray());
        }

        public IEnumerable<IBlogPostTransformer> RegisteredBlogPostTransformers {
            get {
                return TransformersService.Instance.BlogPostTransformers.AsEnumerable();
            }
        }

        public void RegisterTransformer(Func<IBlogPostTransformer> constructorFunc) {
            var transformer = constructorFunc();
            BlogPostTransformers.Add(transformer);
        }

        public void RemoveTransformer(string transformerName) {
            RemoveTransformers(transformerName);
        }

        private List<IBlogPostTransformer> BlogPostTransformers { get; set; }

        private static void SetDefaultTransformers() {
            TransformersService.Instance.BlogPostTransformers = new List<IBlogPostTransformer> {
                new PreTagTransformer(),
            };
        }

        private static void RemoveTransformers(params string[] transformerNames) {
            transformerNames.ToList().ForEach(x => x = x.ToLowerInvariant());

            var foundTransformers = from transformer in TransformersService.Instance.BlogPostTransformers
                                    let type = transformer.GetType()
                                    let typeFullName = type.FullName.ToLowerInvariant()
                                    where type.Assembly.FullName.StartsWith("Blaven, ")
                                    && (transformerNames.Contains(typeFullName) || transformerNames.Any(x => typeFullName.EndsWith(x)))
                                    select transformer;

            foundTransformers.ToList().ForEach(x => TransformersService.Instance.BlogPostTransformers.Remove(x));
        }

        public static TransformersService Instance {
            get {
                return SingletonClass._instance;
            }
        }

        private class SingletonClass {
            static SingletonClass() { }
            internal static readonly TransformersService _instance = new TransformersService();
        }
    }
}
