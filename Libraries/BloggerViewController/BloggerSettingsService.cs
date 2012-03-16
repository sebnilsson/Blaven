using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BloggerViewController {
    public static class BloggerSettingsService {
        private static string _filePath;
        private static bool _isInitialized = false;

        public static void Init(string fullFilePath) {
            if(!File.Exists(fullFilePath)) {
                throw new FileNotFoundException(string.Format("The Blogger-settings file couldn't be found at '{0}'.", fullFilePath), fullFilePath);
            }

            _filePath = fullFilePath;
            _isInitialized = true;
        }

        private static IEnumerable<BloggerSetting> _settings;
        public static IEnumerable<BloggerSetting> Settings {
            get {
                if(_settings == null) {
                    CheckIsInitialized();

                    string fileContent = File.ReadAllText(_filePath);
                    _settings = SerializationHelper.GetDeserializedObject<IEnumerable<BloggerSetting>>(fileContent, Enumerable.Empty<BloggerSetting>());
                    
                    foreach(var setting in _settings) {
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
