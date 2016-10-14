using System.Threading.Tasks;

namespace Blaven.Transformers
{
    public interface IBlogPostTransformer
    {
        Task<BlogPost> Transform(BlogPost blogPost);
    }
}