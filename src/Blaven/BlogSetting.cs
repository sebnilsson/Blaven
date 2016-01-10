using System;

namespace Blaven
{
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

        public string BlogKey { get; private set; }

        public string Id { get; private set; }

        public string Name { get; private set; }
    }
}