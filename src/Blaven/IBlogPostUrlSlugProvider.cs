namespace Blaven
{
    public interface IBlogPostUrlSlugProvider
    {
        string GetSlug(BlogPost blogPost);
    }
}