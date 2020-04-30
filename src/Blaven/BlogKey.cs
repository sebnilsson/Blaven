using System;
using System.Collections.Generic;

namespace Blaven
{
    public struct BlogKey : IEquatable<BlogKey>
    {
        public BlogKey(string value)
        {
            Value = (value ?? string.Empty).ToLowerInvariant();
        }

        public string Value { get; }

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
            return x.Value == y.Value && x.Value == y.Value;
        }
        public static bool operator !=(BlogKey x, BlogKey y)
        {
            return !(x == y);
        }
    }
}
