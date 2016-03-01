namespace Blaven
{
    public interface IBlogPostUrlSlugProvider
    {
        string GetUrlSlug(BlogPost blogPost);
    }
}