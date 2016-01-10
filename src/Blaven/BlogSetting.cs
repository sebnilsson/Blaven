using System;
using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, Id={Id}, Name={Name}")]
    public class BlogSetting
    {
        public BlogSetting(string blogKey, string id, string name)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            this.BlogKey = blogKey;
            this.Id = id;
            this.Name = name;
        }

        public string BlogKey { get; }

        public string Id { get; }

        public string Name { get; }
    }
}