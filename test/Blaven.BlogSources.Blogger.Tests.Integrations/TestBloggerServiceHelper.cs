using Google.Apis.Blogger.v3;
using Google.Apis.Services;

namespace Blaven.Sources.Blogger.Tests.Integrations
{
    public static class TestBloggerServiceHelper
    {
        public static BloggerService Get()
        {
            string apiKey = TestAppSettingsHelper.ApiKey;

            var initializer = new BaseClientService.Initializer { ApiKey = apiKey, ApplicationName = "BlavenTests" };

            var service = new BloggerService(initializer);
            return service;
        }
    }
}