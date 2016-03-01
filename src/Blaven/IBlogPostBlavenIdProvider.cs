namespace Blaven
{
    public interface IBlogPostBlavenIdProvider
    {
        string GetBlavenId(BlogPostHead blogPost);
    }
}