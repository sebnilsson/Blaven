using System;
using System.Collections.Generic;
using System.Configuration;

namespace Blaven.BlogSources.Blogger
{
    internal static class AppSettingsHelper
    {
        private const string ApiKeyAppSettingKey = "Blaven.BlogSources.Blogger.ApiKey";

        private const string BlogIdAppSettingKey = "Blaven.Blogs.TestBlog.Id";

        private const string BloggerExpectedAfterLastUpdatedAtAppSettingKey =
            "Blaven.BlogSources.Blogger.ExpectedAfterLastUpdatedAt";

        private const string BloggerExpectedAfterLastUpdatedAtPostsCountAppSettingKey =
            "Blaven.BlogSources.Blogger.ExpectedAfterLastUpdatedAtPostsCount";

        private const string BloggerExpectedPostsCountAppSettingKey = "Blaven.BlogSources.Blogger.ExpectedPostsCount";

        public static readonly int ExpectedPostsCount = GetExpectedPostsCount();

        public static readonly int ExpectedAfterLastUpdatedAtPostsCount = GetExpectedAfterLastUpdatedAtPostsCount();

        public static readonly DateTime ExpectedAfterLastUpdatedAt = GetExpectedAfterLastUpdatedAt();

        public static string GetApiKey()
        {
            string apiKey = GetAppSetting(ApiKeyAppSettingKey);
            return apiKey;
        }

        public static string GetBlogId()
        {
            string blogId = GetAppSetting(BlogIdAppSettingKey);
            return blogId;
        }

        private static int GetExpectedAfterLastUpdatedAtPostsCount()
        {
            int expectedPostsCount = GetAppSettingInt(BloggerExpectedAfterLastUpdatedAtPostsCountAppSettingKey);
            return expectedPostsCount;
        }

        private static DateTime GetExpectedAfterLastUpdatedAt()
        {
            var expectedAfterLastUpdatedAt = GetAppSettingDateTime(BloggerExpectedAfterLastUpdatedAtAppSettingKey);
            return expectedAfterLastUpdatedAt;
        }

        private static int GetExpectedPostsCount()
        {
            int expectedPostsCount = GetAppSettingInt(BloggerExpectedPostsCountAppSettingKey);
            return expectedPostsCount;
        }

        private static string GetAppSetting(string key)
        {
            string appSetting = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(appSetting))
            {
                throw new KeyNotFoundException($"No AppSetting-value found for key '{key}'.");
            }

            return appSetting;
        }

        private static int GetAppSettingInt(string key)
        {
            string appSetting = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(appSetting))
            {
                throw new KeyNotFoundException($"No AppSetting-value found for key '{key}'.");
            }

            int result;
            if (!int.TryParse(appSetting, out result))
            {
                throw new KeyNotFoundException(
                    $"Could not parse AppSetting-value as {result.GetType().Name} for key '{key}'.");
            }

            return result;
        }

        private static DateTime GetAppSettingDateTime(string key)
        {
            string appSetting = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(appSetting))
            {
                throw new KeyNotFoundException($"No AppSetting-value found for key '{key}'.");
            }

            DateTime result;
            if (!DateTime.TryParse(appSetting, out result))
            {
                throw new KeyNotFoundException(
                    $"Could not parse AppSetting-value as {result.GetType().Name} for key '{key}'.");
            }

            return result;
        }
    }
}