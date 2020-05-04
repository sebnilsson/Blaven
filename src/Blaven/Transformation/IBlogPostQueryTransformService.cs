namespace Blaven.Transformation
{
    public interface IBlogPostQueryTransformService
    {
        void TransformPost(BlogPost post);

        void TransformPostHeader(BlogPostHeader postHeader);
    }
}
