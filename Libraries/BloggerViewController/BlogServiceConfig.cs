using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BloggerViewController.Configuration;
using BloggerViewController.Data;

namespace BloggerViewController {
    /// <summary>
    /// A class for setting configurations on an instance of BlogService.
    /// </summary>
    public class BlogServiceConfig {
        /// <summary>
        /// Initalizes a new instance of BloggerSettingsConfig.
        /// </summary>
        /// <param name="blogStore">The BlogStore to use.</param>
        /// <param name="bloggerSettingsFilePath">The full path to the Blogger-settings file.</param>
        /// <param name="settings">The bloggerHelper to use. Defaults to new default instance.</param>
        public BlogServiceConfig(IBlogStore blogStore, string bloggerSettingsFilePath, string bloggerHelperUri = null)
            : this(blogStore, BloggerSettingsService.ParseFile(bloggerSettingsFilePath), bloggerHelperUri) {

        }

        /// <summary>
        /// Initalizes a new instance of BloggerSettingsConfig.
        /// </summary>
        /// <param name="blogStore">The BlogStore to use.</param>
        /// <param name="settings">The Blogger-settings to use.</param>
        /// <param name="settings">The bloggerHelper to use. Defaults to new default instance.</param>
        public BlogServiceConfig(IBlogStore blogStore, IEnumerable<BloggerSetting> settings, string bloggerHelperUri = null) {
            if(blogStore == null) {
                throw new ArgumentNullException("blogStore");
            }

            if(settings == null || !settings.Any()) {
                throw new ArgumentNullException("settings", "The provided Blogger-settings cannot be null or empty.");
            }

            this.BlogStore = blogStore;
            this.BloggerSettings = settings;
            this.BloggerHelper = new BloggerHelper(bloggerHelperUri);
            this.PageSize = AppSettingsService.PageSize;
        }

        /// <summary>
        /// Gets the BlogStore used in the BlogServiceConfig.
        /// </summary>
        public IBlogStore BlogStore { get; private set; }

        /// <summary>
        /// Gets a list of Blogger-settings used in the BlogServiceConfig.
        /// </summary>
        public IEnumerable<BloggerSetting> BloggerSettings { get; private set; }

        /// <summary>
        /// Gets the page-size used in the BlogServiceConfig. Defaults to AppSettings default.
        /// </summary>
        public int PageSize { get; set; }

        internal BloggerHelper BloggerHelper { get; private set; }
    }
}
