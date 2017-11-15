using System.Collections.Generic;
using Blaven.BlogSources.Testing;

namespace Blaven.Testing
{
    public static class AppSettingTestData
    {
        public const string AppSettingsTestPassword = "TestPassword";
        public const string AppSettingsTestUsername = "TestUsername";

        public static IEnumerable<KeyValuePair<string, string>> CreateDictionary()
        {
            yield return new KeyValuePair<string, string>($"Blaven.BlogSources.{nameof(FakeBlogSource)}.Username",
                AppSettingsTestUsername);

            yield return new KeyValuePair<string, string>($"Blaven.BlogSources.{nameof(FakeBlogSource)}.Password",
                AppSettingsTestPassword);

            yield return new KeyValuePair<string, string>("Blaven.Blogs.BlogKey1.Id", "BlogKey1Id");

            yield return new KeyValuePair<string, string>("Blaven.Blogs.BlogKey1.Name", "BlogKey1Name");

            yield return new KeyValuePair<string, string>("Blaven.Blogs.BlogKey2.Id", "BlogKey2Id");

            yield return new KeyValuePair<string, string>("Blaven.Blogs.BlogKey2.Name", "BlogKey2Name");

            yield return new KeyValuePair<string, string>("Blaven.Blogs.BlogKey3", string.Empty);

            yield return new KeyValuePair<string, string>("Blaven.BlogSources.Blogger.ApiKey", "TestApiKey");
        }
    }
}