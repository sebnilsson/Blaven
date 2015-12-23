namespace Blaven
{
    public interface IBlogPostBlavenIdProvider
    {
        string GetId(BlogPost blogPost);
    }
}