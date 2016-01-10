using System;
using System.Collections.Generic;
using System.Configuration;

namespace Blaven.Sources.Blogger.Tests.Integrations
{
    public static class TestAppSettingsHelper
    {
        private static readonly Lazy<string> ApiKeyLazy =
            new Lazy<string>(() => ConfigurationManager.AppSettings["ApiKey"]);

        public static string ApiKey
        {
            get
            {
                string apiKey = ApiKeyLazy.Value;

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    throw new KeyNotFoundException("AppSettings does not contain 'ApiKey'.");
                }

                return apiKey;
            }
        }
    }
}