namespace Blaven
{
    public struct BlogKey
    {
        public BlogKey(string value)
        {
            Value = (value ?? string.Empty).ToLowerInvariant();
        }

        public string Value { get; }

        public override string ToString()
        {
            return Value;
        }
    }
}
