namespace Blaven
{
    /// <summary>
    /// A class for setting configurations on an instance of BlogService.
    /// </summary>
    public class BlogServiceConfig
    {
        /// <summary>
        /// Creates a new instance of BloggerSettingsConfig.
        /// </summary>
        public BlogServiceConfig()
        {
            this.CacheTime = AppSettingsService.CacheTime;
            this.EnsureBlogsRefreshed = AppSettingsService.EnsureBlogsRefreshed;
            this.PageSize = AppSettingsService.PageSize;
            this.RefreshAsync = AppSettingsService.RefreshAsync;
        }

        /// <summary>
        /// Gets the default cache-time, in minutes. Defaults to AppSettings default.
        /// </summary>
        public int CacheTime { get; set; }

        /// <summary>
        /// Gets or sets if the BlogService should automatically ensure that blogs are refresh upon instantiation. Defaults to AppSettings default.
        /// </summary>
        public bool EnsureBlogsRefreshed { get; set; }

        /// <summary>
        /// Gets or sets the page-size used in the BlogServiceConfig. Defaults to AppSettings default.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets which mode should be used for refreshing data from blogs.
        /// </summary>
        public bool RefreshAsync { get; set; }
    }
}