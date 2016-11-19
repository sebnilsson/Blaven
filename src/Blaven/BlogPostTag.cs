namespace Blaven
{
    public class BlogPostTag
    {
        public BlogPostTag()
        {
        }

        public BlogPostTag(string text)
        {
            this.Text = text;
        }

        public long Id { get; set; }

        public BlogPost BlogPost { get; set; }

        public long BlogPostId { get; set; }

        public string Text { get; set; }
    }
}