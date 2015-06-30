using System;
using System.Linq;

using Blaven.DataSources;
using Newtonsoft.Json;

namespace Blaven
{
    public class BlavenBlogSetting<TBlogDataSource> : BlavenBlogSetting
        where TBlogDataSource : IDataSource
    {
        public BlavenBlogSetting(string blogKey)
            : base(blogKey, Activator.CreateInstance<TBlogDataSource>())
        {
        }
    }

    /// <summary>
    /// Represents a Blog-setting.
    /// </summary>
    public class BlavenBlogSetting
    {
        public BlavenBlogSetting(string blogKey, IDataSource blogDataSource)
        {
            this.BlogKey = blogKey;
            this.BlogDataSource = blogDataSource;
        }

        internal BlavenBlogSetting()
        {
            this.SetBlogDataSource(null);
        }

        internal string DataSource { get; set; }

        internal string PasswordKey { get; set; }

        internal string UsernameKey { get; set; }

        /// <summary>
        /// The base-URL for the blog. Leave empty to use default value from the data-source.
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The unique identifier for the blog.
        /// </summary>
        public string BlogKey { get; set; }

        /// <summary>
        /// The data-source for the blog.
        /// </summary>
        [JsonIgnore]
        public IDataSource BlogDataSource { get; set; }

        /// <summary>
        /// The ID of the blog at the data-source.
        /// </summary>
        public string DataSourceId { get; set; }

        /// <summary>
        /// The URI to the data-source. Will be resolved to correct URI from BlogId, if left empty.
        /// </summary>
        public string DataSourceUri { get; set; }

        /// <summary>
        /// The password for the account at the data-source.
        /// </summary>
        public string Password { get; internal set; }

        /// <summary>
        /// The username for the account at the data-source.
        /// </summary>
        //public string Username { get; internal set; }

        public void SetBlogDataSource(string dataSourceName)
        {
            string name = (dataSourceName ?? string.Empty).ToLowerInvariant();
            switch (name)
            {
                case "dropbox":
                    break;
                case "googledrive":
                    break;
                default:
                    this.BlogDataSource = GetBlogDataSourceReflected(dataSourceName);
                    break;
            }
        }

        private static IDataSource GetBlogDataSourceReflected(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return null;
            }

            var types = AppDomainAssemblyTypeScanner.GetTypesOf<IDataSource>();
            var type = types.FirstOrDefault(x => x.Name.Equals(typeName, StringComparison.InvariantCultureIgnoreCase));

            if (type == null)
            {
                throw new BlavenException(string.Format("No IBlogDataSource found for type-name '{0}'.", typeName));
            }

            var instance = Activator.CreateInstance(type) as IDataSource;

            if (instance == null)
            {
                throw new BlavenException(
                    string.Format("No IBlogDataSource could be instantiated for type-name '{0}'.", type.FullName));
            }
            return instance;
        }
    }
}