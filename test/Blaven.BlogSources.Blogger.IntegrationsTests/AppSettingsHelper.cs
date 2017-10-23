using System;
using System.Collections.Generic;
using System.Configuration;

namespace Blaven.BlogSources.Blogger.IntegrationsTests
{
    internal static class AppSettingsHelper
    {
        private const string ApiKeyAppSettingKey = "Blaven.BlogSources.Blogger.ApiKey";
        private const string BloggerExpectedAfterLastUpdatedAtAppSettingKey =
            "Blaven.BlogSources.Blogger.ExpectedAfterLastUpdatedAt";
        private const string BloggerExpectedAfterLastUpdatedAtPostsCountAppSettingKey =
            "Blaven.BlogSources.Blogger.ExpectedAfterLastUpdatedAtPostsCount";
        private const string BloggerExpectedPostsCountAppSettingKey = "Blaven.BlogSources.Blogger.ExpectedPostsCount";
        private const string BlogIdAppSettingKey = "Blaven.Blogs.TestBlog.Id";
        public static readonly int ExpectedPostsCount = GetExpectedPostsCount();
        public static readonly int ExpectedAfterLastUpdatedAtPostsCount = GetExpectedAfterLastUpdatedAtPostsCount();
        public static readonly DateTime ExpectedAfterLastUpdatedAt = GetExpectedAfterLastUpdatedAt();

        public static string GetApiKey()
        {
            var apiKey = GetAppSetting(ApiKeyAppSettingKey);
            return apiKey;
        }

        public static string GetBlogId()
        {
            var blogId = GetAppSetting(BlogIdAppSettingKey);
            return blogId;
        }

        private static string GetAppSetting(string key)
        {
            var appSetting = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(appSetting))
                throw new KeyNotFoundException($"No AppSetting-value found for key '{key}'.");

            return appSetting;
        }

        private static DateTime GetAppSettingDateTime(string key)
        {
            var appSetting = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(appSetting))
                throw new KeyNotFoundException($"No AppSetting-value found for key '{key}'.");

            DateTime result;
            if (!DateTime.TryParse(appSetting, out result))
            {
                throw new KeyNotFoundException(
                    $"Could not parse AppSetting-value as {result.GetType().Name} for key '{key}'.");
            }

            return result;
        }

        private static int GetAppSettingInt(string key)
        {
            var appSetting = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(appSetting))
                throw new KeyNotFoundException($"No AppSetting-value found for key '{key}'.");

            int result;
            if (!int.TryParse(appSetting, out result))
            {
                throw new KeyNotFoundException(
                    $"Could not parse AppSetting-value as {result.GetType().Name} for key '{key}'.");
            }

            return result;
        }

        private static DateTime GetExpectedAfterLastUpdatedAt()
        {
            var expectedAfterLastUpdatedAt = GetAppSettingDateTime(BloggerExpectedAfterLastUpdatedAtAppSettingKey);
            return expectedAfterLastUpdatedAt;
        }

        private static int GetExpectedAfterLastUpdatedAtPostsCount()
        {
            var expectedPostsCount = GetAppSettingInt(BloggerExpectedAfterLastUpdatedAtPostsCountAppSettingKey);
            return expectedPostsCount;
        }

        private static int GetExpectedPostsCount()
        {
            var expectedPostsCount = GetAppSettingInt(BloggerExpectedPostsCountAppSettingKey);
            return expectedPostsCount;
        }
    }
}