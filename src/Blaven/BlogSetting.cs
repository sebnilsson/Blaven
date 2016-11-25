using System;
using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, Id={Id}, Name={Name}")]
    public class BlogSetting : BlogKeyItemBase
    {
        public BlogSetting(BlogKey blogKey, string id = null, string name = null)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }
            if (!blogKey.HasValue)
            {
                throw new ArgumentOutOfRangeException(nameof(blogKey), $"{nameof(Blaven.BlogKey)} must have a value.");
            }

            this.BlogKey = blogKey.Value;
            this.Id = id;
            this.Name = name;
        }

        public string Id { get; }

        public string Name { get; }
    }
}