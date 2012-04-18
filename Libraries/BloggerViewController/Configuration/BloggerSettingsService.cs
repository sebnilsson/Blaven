using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BloggerViewController.Configuration {
    public static class BloggerSettingsService {
        public static IEnumerable<BloggerSetting> ParseFile(string bloggerSettingsFilePath) {
            if(!File.Exists(bloggerSettingsFilePath)) {
                throw new FileNotFoundException(
                    string.Format("The Blogger-settings file couldn't be found at '{0}'.", bloggerSettingsFilePath), bloggerSettingsFilePath);
            }

            string fileContent = File.ReadAllText(bloggerSettingsFilePath);
            var settings = JsonSerializationHelper.GetDeserializedObject<IEnumerable<BloggerSetting>>(fileContent, Enumerable.Empty<BloggerSetting>());

            foreach(var setting in settings) {
                if(string.IsNullOrWhiteSpace(setting.BlogKey)) {
                    throw new System.Configuration.ConfigurationErrorsException("Blogger-settings cannot have a blank blog-key.");
                }
                setting.Password = AppSettingsService.GetConfigValue(setting.PasswordKey);
                setting.Username = AppSettingsService.GetConfigValue(setting.UsernameKey);
            }

            if(settings == null || !settings.Any()) {
                throw new System.Configuration.ConfigurationErrorsException(
                    string.Format("No Blogger-settings were defined in file at '[0}'.", bloggerSettingsFilePath));
            }

            return settings;
        }
    }
}
