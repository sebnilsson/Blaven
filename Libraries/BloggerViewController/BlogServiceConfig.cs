using System;
using System.Collections.Generic;
using System.Linq;

using BloggerViewController.Configuration;
using BloggerViewController.Data;
using Raven.Client;
using Raven.Client.Document;

namespace BloggerViewController {
    /// <summary>
    /// A class for setting configurations on an instance of BlogService.
    /// </summary>
    public class BlogServiceConfig {
        /// <summary>
        /// Creates a new instance of BloggerSettingsConfig.
        /// </summary>
        /// <param name="bloggerSettingsFilePath">The full path to the Blogger-settings file.</param>
        /// <param name="documentStore">The IDocument to use. Defaults to a DocumentStore using the default store-URL in the AppSettings.</param>
        /// <param name="bloggerHelperUri">The BloggerHelper to use. Defaults to new default instance.</param>
        public BlogServiceConfig(string bloggerSettingsFilePath, IDocumentStore documentStore = null, string bloggerHelperUri = null)
            : this(BloggerSettingsService.ParseFile(bloggerSettingsFilePath), documentStore, bloggerHelperUri) {

        }

        /// <summary>
        /// Creates a new instance of BloggerSettingsConfig.
        /// </summary>
        /// <param name="settings">The Blogger-settings to use.</param>
        /// <param name="documentStore">The IDocument to use. Defaults to a DocumentStore using the default store-URL in the AppSettings.</param>
        /// <param name="bloggerHelperUri">The BloggerHelper to use. Defaults to new default instance.</param>
        public BlogServiceConfig(IEnumerable<BloggerSetting> settings, IDocumentStore documentStore = null, string bloggerHelperUri = null) {
            if(settings == null || !settings.Any()) {
                throw new ArgumentNullException("settings", "The provided Blogger-settings cannot be null or empty.");
            }

            this.BloggerSettings = settings;
            this.BlogStore = new RavenDbBlogStore(documentStore ?? new DocumentStore { Url = AppSettingsService.RavenDbStoreUrlKey });
            this.DocumentStore = this.BlogStore.DocumentStore;
            this.BloggerHelper = new BloggerHelper(bloggerHelperUri);
            this.PageSize = AppSettingsService.PageSize;
        }

        /// <summary>
        /// Gets a list of Blogger-settings used in the BlogServiceConfig.
        /// </summary>
        public IEnumerable<BloggerSetting> BloggerSettings { get; private set; }

        internal RavenDbBlogStore BlogStore { get; private set; }

        /// <summary>
        /// Gets the page-size used in the BlogServiceConfig. Defaults to AppSettings default.
        /// </summary>
        public int PageSize { get; set; }

        internal BloggerHelper BloggerHelper { get; private set; }

        public IDocumentStore DocumentStore { get; private set; }
    }
}
