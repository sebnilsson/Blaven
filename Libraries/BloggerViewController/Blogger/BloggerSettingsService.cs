using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BloggerViewController.Blogger {
    /// <summary>
    /// A static service-class to handle the application's Blogger-settings.
    /// </summary>
    public static class BloggerSettingsService {
        private static string _filePath;
        private static bool _isInitialized = false;

        /// <summary>
        /// Initializes the BloggerSettingsService-class.
        /// </summary>
        /// <param name="fullFilePath">The full path to the Blogger-settings-file.</param>
        public static void Init(string fullFilePath) {
            if(!File.Exists(fullFilePath)) {
                throw new FileNotFoundException(string.Format("The Blogger-settings file couldn't be found at '{0}'.", fullFilePath), fullFilePath);
            }

            _filePath = fullFilePath;
            _isInitialized = true;
        }

        private static IEnumerable<BloggerSetting> _settings;
        /// <summary>
        /// Gets a list of Blogger-settings configured in the Blogger-settings-file.
        /// </summary>
        public static IEnumerable<BloggerSetting> Settings {
            get {
                if(_settings == null) {
                    CheckIsInitialized();

                    string fileContent = File.ReadAllText(_filePath);
                    _settings = JsonSerializationHelper.GetDeserializedObject<IEnumerable<BloggerSetting>>(fileContent, Enumerable.Empty<BloggerSetting>());
                    
                    foreach(var setting in _settings) {
                        if(_settings.Count() == 1 && string.IsNullOrWhiteSpace(setting.BlogKey)) {
                            setting.BlogKey = BlogService.DefaultBlogKey;
                        }

                        if(string.IsNullOrWhiteSpace(setting.BlogKey)) {
                            throw new System.Configuration.ConfigurationErrorsException("Blogger-settings cannot have a blank blog-key.");
                        }
                        setting.Password = ConfigurationService.GetConfigValue(setting.PasswordKey);
                        setting.Username = ConfigurationService.GetConfigValue(setting.UsernameKey);
                    }

                    if(_settings == null || !_settings.Any()) {
                        throw new System.Configuration.ConfigurationErrorsException(
                            string.Format("No BloggerSettings were defined in file at '[0}'.", _filePath));
                    }
                }
                return _settings;
            }
        }

        private static void CheckIsInitialized() {
            if(!_isInitialized) {
                throw new ApplicationException("BloggerSettingService has not been initialized. Please call the method 'Init' before using class.");
            }
        }
    }
}
