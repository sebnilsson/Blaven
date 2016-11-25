namespace Blaven
{
    public class BlogKey
    {
        public BlogKey(string value)
        {
            this.Value = value?.ToLowerInvariant();
        }

        public bool HasValue => (this.Value != null);

        public string Value { get; }
        
        public static implicit operator BlogKey(string value)
        {
            var blogKey = new BlogKey(value);
            return blogKey;
        }

        public static implicit operator string(BlogKey blogKey)
        {
            string value = blogKey.Value;
            return value;
        }
    }
}