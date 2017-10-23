namespace Blaven
{
    public abstract class BlogKeyItemBase
    {
        private string _blogKey;

        public string BlogKey
        {
            get => _blogKey;
            set => _blogKey = value?.ToLowerInvariant();
        }
    }
}