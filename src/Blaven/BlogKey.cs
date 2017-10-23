namespace Blaven
{
    public class BlogKey
    {
        public BlogKey(string value)
        {
            Value = value?.ToLowerInvariant();
        }

        public bool HasValue => Value != null;

        public string Value { get; }

        public static implicit operator BlogKey(string value)
        {
            var blogKey = new BlogKey(value);
            return blogKey;
        }

        public static implicit operator string(BlogKey blogKey)
        {
            var value = blogKey.Value;
            return value;
        }
    }
}