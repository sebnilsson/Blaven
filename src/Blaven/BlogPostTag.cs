namespace Blaven
{
    public class BlogPostTag
    {
        public BlogPostTag()
        {
        }

        public BlogPostTag(string text)
        {
            Text = text;
        }

        public BlogPost BlogPost { get; set; }

        public long BlogPostId { get; set; }

        public long Id { get; set; }

        public string Text { get; set; }
    }
}