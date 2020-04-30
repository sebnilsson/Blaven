using System.Threading.Tasks;

namespace Blaven.Transformers
{
    public interface IBlogPostTransformer
    {
        void Transform(BlogPost blogPost);
    }
}
