using System.Linq;

namespace Blaven.Tests
{
    public static partial class TestData
    {
        public const string BlogKey = "test_blog-key"; //"Test_Blog-Key";

        public const string BlogKey1 = "test blog-key 1"; //"Test Blog-Key 1";

        public const string BlogKey2 = "test-blog key two"; //"Test-Blog Key TWO";

        public const string BlogKey3 = "blog-test key trees"; //"Blog-Test Key Trees";

        public static string[] BlogKeys => new[] { BlogKey1, BlogKey2, BlogKey3 }.ToArray();

        public static string GetTestString(string name, string blogKey, int index, bool isUpdate = false)
        {
            string prefix = isUpdate ? "Updated" : null;

            string testString = $"{prefix}Test{name}_{blogKey}_{index}";
            return testString;
        }
    }
}