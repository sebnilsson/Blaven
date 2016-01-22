namespace Blaven
{
    public interface IBlogPostBlavenIdProvider
    {
        string GetId(BlogPostBase blogPost);
    }
}