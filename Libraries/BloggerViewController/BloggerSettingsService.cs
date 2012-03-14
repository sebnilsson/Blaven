using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BloggerViewController {
    public class BloggerSettingsService {
        private string _filePath;

        public BloggerSettingsService(string fullFilePath) {
            _filePath = fullFilePath;
        }

        public BloggerSetting DefaultSetting {
            get {
                return GetSettings().FirstOrDefault();
            }
        }

        public IEnumerable<BloggerSetting> GetSettings() {
            if(!File.Exists(_filePath)) {
                throw new FileNotFoundException(string.Format("The Blogger-settings file couldn't be found at '{0}'.", _filePath), _filePath);
            }

            string fileContent = File.ReadAllText(_filePath);
            var deserialized = SerializationHelper.GetDeserializedObject<IEnumerable<BloggerSetting>>(fileContent, Enumerable.Empty<BloggerSetting>());
            return deserialized;
        }
    }
}
