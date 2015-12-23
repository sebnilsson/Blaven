using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

namespace Blaven
{
    public class BlogServiceConfig
    {
        private const string BlavenConfigPrefix = "Blaven";

        private const string CredentialsKeyword = "Credentials";

        private const string NameSuffix = "Name";

        private const string PasswordSuffix = "Password";

        private static readonly string CredentialsPrefix = string.Format(
            "{0}.{1}.",
            BlavenConfigPrefix,
            CredentialsKeyword);

        private static readonly Lazy<BlogServiceConfig> InstanceLazy = new Lazy<BlogServiceConfig>(
            () =>
                {
                    var configuration = GetEnumerableAppSettings(ConfigurationManager.AppSettings);
                    return new BlogServiceConfig(configuration);
                });

        public BlogServiceConfig(IEnumerable<KeyValuePair<string, string>> appSettings)
        {
            if (appSettings == null)
            {
                throw new ArgumentNullException("appSettings");
            }

            this.PopulateAppSettings(appSettings);

            this.CacheTime = this.GetInt("CacheTime", defaultValue: 5);
            this.PageSize = this.GetInt("PageSize", defaultValue: 5);
        }

        public static BlogServiceConfig Instance
        {
            get
            {
                return InstanceLazy.Value;
            }
        }

        public IReadOnlyDictionary<string, string> AppSettings { get; private set; }

        public IReadOnlyDictionary<string, KeyValuePair<string, string>> Credentials { get; private set; }

        public int CacheTime { get; private set; }

        public int PageSize { get; private set; }

        public string Get(string configKey, string defaultValue = "")
        {
            if (configKey == null)
            {
                throw new ArgumentNullException("configKey");
            }

            string formattedConfigKey = string.Format("{0}.{1}", BlavenConfigPrefix, configKey);

            string value = this.AppSettings.ContainsKey(formattedConfigKey)
                               ? this.AppSettings[formattedConfigKey]
                               : null;

            return !string.IsNullOrWhiteSpace(value) ? value : defaultValue;
        }

        public bool GetBool(string configKey, bool defaultValue = default(bool))
        {
            if (configKey == null)
            {
                throw new ArgumentNullException("configKey");
            }

            string value = this.Get(configKey);
            bool result;
            if (!bool.TryParse(value, out result))
            {
                result = defaultValue;
            }

            return result;
        }

        public int GetInt(string configKey, int defaultValue = default(int))
        {
            if (configKey == null)
            {
                throw new ArgumentNullException("configKey");
            }

            string value = this.Get(configKey);
            int result;
            if (!int.TryParse(value, out result))
            {
                result = defaultValue;
            }

            return result;
        }

        private void PopulateAppSettings(IEnumerable<KeyValuePair<string, string>> appSettings)
        {
            var items = appSettings.ToList();

            var credentials =
                items.Where(x => x.Key.StartsWith(CredentialsPrefix, StringComparison.InvariantCultureIgnoreCase))
                    .ToDictionary(x => x.Key, x => x.Value, StringComparer.InvariantCultureIgnoreCase);

            var appSettingsDictionary = items.Except(credentials)
                .ToDictionary(x => x.Key, x => x.Value, StringComparer.InvariantCultureIgnoreCase);

            var credentialsDictionary = GetCredentials(credentials);

            this.AppSettings = new ReadOnlyDictionary<string, string>(appSettingsDictionary);
            this.Credentials = new ReadOnlyDictionary<string, KeyValuePair<string, string>>(credentialsDictionary);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetEnumerableAppSettings(
            NameValueCollection appSettings)
        {
            if (appSettings == null)
            {
                throw new ArgumentNullException("appSettings");
            }

            var enumerableAppSettings =
                appSettings.Keys.OfType<string>().Select(x => new KeyValuePair<string, string>(x, appSettings[x]));
            return enumerableAppSettings;
        }

        private static IDictionary<string, KeyValuePair<string, string>> GetCredentials(
            IDictionary<string, string> credentials)
        {
            var result = new Dictionary<string, KeyValuePair<string, string>>();

            var credentialKeys = (from credential in credentials.Keys
                                  let item = credential.Substring(CredentialsPrefix.Length)
                                  let dotIndex = item.IndexOf('.')
                                  let key = item.Substring(0, dotIndex)
                                  select key).Distinct().ToList();

            foreach (var credentialKey in credentialKeys)
            {
                string nameKey = string.Format("{0}{1}.{2}", CredentialsPrefix, credentialKey, NameSuffix);
                string passwordKey = string.Format("{0}{1}.{2}", CredentialsPrefix, credentialKey, PasswordSuffix);

                string name = credentials.ContainsKey(nameKey) ? credentials[nameKey] : null;
                string password = credentials.ContainsKey(passwordKey) ? credentials[passwordKey] : null;

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var kvp = new KeyValuePair<string, string>(name, password);
                    result[credentialKey] = kvp;
                }
            }

            return result;
        }
    }
}