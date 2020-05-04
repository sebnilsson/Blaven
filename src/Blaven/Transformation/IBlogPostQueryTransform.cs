namespace Blaven.Transformation
{
    public interface IBlogPostQueryTransform
    {
        void Transform(BlogPost post);

        void Transform(BlogPostHeader postHeader);
    }
}
