using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.Blogger;

namespace Blaven {
    /// <summary>
    /// A class for setting configurations on an instance of BlogService.
    /// </summary>
    public class BlogServiceConfig {
        public BlogServiceConfig()
            : this(BloggerSettingsService.ParseFile(AppSettingsService.BloggerSettingsPath)) {
        }

        /// <summary>
        /// Creates a new instance of BloggerSettingsConfig.
        /// </summary>
        /// <param name="documentStore">The DocumentStore to use.</param>
        /// <param name="bloggerSettingsFilePath">The full path to the Blogger-settings file.</param>
        public BlogServiceConfig(string bloggerSettingsFilePath)
            : this(BloggerSettingsService.ParseFile(bloggerSettingsFilePath)) {

        }

        /// <summary>
        /// Creates a new instance of BloggerSettingsConfig.
        /// </summary>
        /// <param name="settings">The Blogger-settings to use.</param>
        public BlogServiceConfig(IEnumerable<BloggerSetting> settings) {
            if(settings == null || !settings.Any()) {
                throw new ArgumentNullException("settings", "The provided Blogger-settings cannot be null or empty.");
            }

            this.BloggerSettings = settings;

            this.CacheTime = AppSettingsService.CacheTime;
            this.EnsureBlogsRefreshed = AppSettingsService.EnsureBlogsRefreshed;
            this.PageSize = AppSettingsService.PageSize;
            this.RefreshAsync = AppSettingsService.RefreshAsync;
        }

        /// <summary>
        /// Gets a list of Blogger-settings used in the BlogServiceConfig.
        /// </summary>
        public IEnumerable<BloggerSetting> BloggerSettings { get; private set; }


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