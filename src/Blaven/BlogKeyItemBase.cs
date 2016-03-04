namespace Blaven
{
    public abstract class BlogKeyItemBase
    {
        private string blogKey;

        public string BlogKey
        {
            get
            {
                return this.blogKey;
            }
            set
            {
                this.blogKey = value?.ToLowerInvariant();
            }
        }
    }
}