using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Blaven
{
    public static class BlavenBlogSettingsParser
    {
        private static readonly JsonSerializerSettings JsonSettings;

        static BlavenBlogSettingsParser()
        {
            var resolver = new CamelCasePropertyNamesContractResolver();
            resolver.DefaultMembersSearchFlags |= System.Reflection.BindingFlags.NonPublic;
            JsonSettings = new JsonSerializerSettings { ContractResolver = resolver };
        }

        public static IEnumerable<BlavenBlogSetting> ParseFile()
        {
            return ParseFile(AppSettingsService.BloggerSettingsPath);
        }

        public static IEnumerable<BlavenBlogSetting> ParseFile(string blogSettingsFilePath)
        {
            if (!File.Exists(blogSettingsFilePath))
            {
                throw new FileNotFoundException(
                    string.Format("The blog-settings file couldn't be found at '{0}'.", blogSettingsFilePath),
                    blogSettingsFilePath);
            }

            string fileContent = File.ReadAllText(blogSettingsFilePath);
            return Parse(fileContent);
        }

        public static IEnumerable<BlavenBlogSetting> Parse(string settingsContent)
        {
            var settings =
                (JsonConvert.DeserializeObject<IEnumerable<BlavenBlogSetting>>(settingsContent, JsonSettings)
                 ?? Enumerable.Empty<BlavenBlogSetting>()).ToList();

            foreach (var setting in settings)
            {
                if (string.IsNullOrWhiteSpace(setting.BlogKey))
                {
                    throw new System.Configuration.ConfigurationErrorsException(
                        "Blog-settings cannot have a blank blog-key.");
                }
                if (string.IsNullOrWhiteSpace(setting.PasswordKey))
                {
                    throw new System.Configuration.ConfigurationErrorsException(
                        "Blog-settings must contain Password-key.");
                }
                setting.DataSource = AppSettingsService.GetConfigValue("Blaven.DataSource", throwException: false);
                //setting.Username = AppSettingsService.GetConfigValue(setting.UsernameKey, throwException: true);
                setting.Password = AppSettingsService.GetConfigValue(setting.PasswordKey, throwException: true);

                setting.SetBlogDataSource(setting.DataSource);
            }

            return settings;
        }
    }
}