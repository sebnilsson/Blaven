using System;
using System.Collections.Generic;

namespace Blaven.Tests
{
    public static partial class TestData
    {
        public const string AppSettingsTestPassword = "TestPassword";

        public const string AppSettingsTestUsername = "TestUsername";

        public static IDictionary<string, string> GetAppSettings()
        {
            var appSettings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
                                  {
                                      {
                                          "Blaven.BlogSources.MockBlogSource.Username",
                                          AppSettingsTestUsername
                                      },
                                      {
                                          "Blaven.BlogSources.MockBlogSource.Password",
                                          AppSettingsTestPassword
                                      },
                                      {
                                          "Blaven.Blogs.BlogKey1.Id",
                                          "BlogKey1Id"
                                      },
                                      {
                                          "Blaven.Blogs.BlogKey1.Name",
                                          "BlogKey1Name"
                                      },
                                      {
                                          "Blaven.Blogs.BlogKey2.Id",
                                          "BlogKey2Id"
                                      },
                                      {
                                          "Blaven.Blogs.BlogKey2.Name",
                                          "BlogKey2Name"
                                      },
                                      {
                                          "Blaven.Blogs.BlogKey3",
                                          string
                                          .Empty
                                      }
                                  };

            return appSettings;
        }
    }
}