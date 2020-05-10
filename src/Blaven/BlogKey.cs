using System;
using System.Collections.Generic;

namespace Blaven
{
    public readonly struct BlogKey : IEquatable<BlogKey>
    {
        private readonly string _value;

        public BlogKey(string value)
        {
            _value = (value ?? string.Empty).ToLowerInvariant();
        }

        public readonly bool HasValue => !string.IsNullOrWhiteSpace(Value);

        public readonly string Value => _value ?? string.Empty;

        public bool Equals(BlogKey other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is BlogKey && Equals((BlogKey)obj);
        }

        public override int GetHashCode()
        {
            return
                -1937169414
                + EqualityComparer<string>.Default.GetHashCode(Value);
        }

        public override string ToString()
        {
            return Value;
        }

        public static bool operator ==(BlogKey x, BlogKey y)
        {
            return x.Value == y.Value;
        }

        public static bool operator !=(BlogKey x, BlogKey y)
        {
            return !(x == y);
        }

        public static implicit operator BlogKey(string str) => new BlogKey(str);

        public static BlogKey Empty => new BlogKey();
    }
}
