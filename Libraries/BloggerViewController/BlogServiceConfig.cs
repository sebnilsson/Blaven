﻿using System;
using System.Collections.Generic;
using System.Linq;

using BloggerViewController.Blogger;
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
            this.BloggerHelper = new BloggerHelper();
            this.CacheTime = AppSettingsService.CacheTime;
            this.PageSize = AppSettingsService.PageSize;
        }

        /// <summary>
        /// Gets a list of Blogger-settings used in the BlogServiceConfig.
        /// </summary>
        public IEnumerable<BloggerSetting> BloggerSettings { get; private set; }

        private RavenDbBlogStore _blogStore;
        internal RavenDbBlogStore BlogStore {
            get {
                if(_blogStore == null) {
                    _blogStore = new RavenDbBlogStore(this.DocumentStore);
                }
                return _blogStore;
            }
            set {
                _blogStore = value;
            }
        }

        public int CacheTime { get; set; }

        /// <summary>
        /// Gets or sets the page-size used in the BlogServiceConfig. Defaults to AppSettings default.
        /// </summary>
        public int PageSize { get; set; }

        internal BloggerHelper BloggerHelper { get; private set; }

        private IDocumentStore _documentStore;
        public IDocumentStore DocumentStore {
            get {
                if(_documentStore == null) {
                    _documentStore = new DocumentStore { Url = AppSettingsService.RavenDbStoreUrl };
                    _documentStore.Initialize();
                }
                return _documentStore;
            }
            set {
                _documentStore = value;
            }
        }
    }
}
