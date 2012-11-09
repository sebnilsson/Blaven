using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;

namespace Blaven.Blogger
{
    public static class BloggerSettingsService
    {
        public static IEnumerable<BloggerSetting> ParseFile(string bloggerSettingsFilePath)
        {
            if (!File.Exists(bloggerSettingsFilePath))
            {
                throw new FileNotFoundException(
                    string.Format("The Blogger-settings file couldn't be found at '{0}'.", bloggerSettingsFilePath),
                    bloggerSettingsFilePath);
            }

            string fileContent = File.ReadAllText(bloggerSettingsFilePath);
            var settings = GetDeserializedObject(fileContent, Enumerable.Empty<BloggerSetting>()).ToList();

            foreach (var setting in settings)
            {
                if (string.IsNullOrWhiteSpace(setting.BlogKey))
                {
                    throw new System.Configuration.ConfigurationErrorsException(
                        "Blogger-settings cannot have a blank blog-key.");
                }
                setting.Password = AppSettingsService.GetConfigValue(setting.PasswordKey, throwException: true);
                setting.Username = AppSettingsService.GetConfigValue(setting.UsernameKey, throwException: true);
            }

            if (settings == null || !settings.Any())
            {
                throw new System.Configuration.ConfigurationErrorsException(
                    string.Format("No Blogger-settings were defined in file at '{0}'.", bloggerSettingsFilePath));
            }

            return settings;
        }

        private static TDeserialized GetDeserializedObject<TDeserialized>(
            string serializedData, TDeserialized defaultValue = default(TDeserialized)) where TDeserialized : class
        {
            if (string.IsNullOrWhiteSpace(serializedData))
            {
                return defaultValue;
            }

            var serializer = new DataContractJsonSerializer(typeof(TDeserialized));
            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(serializedData)))
            {
                var deserialized = serializer.ReadObject(stream) as TDeserialized;
                return deserialized ?? defaultValue;
            }
        }
    }
}