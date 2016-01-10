using System.Collections.ObjectModel;
using System.Linq;

namespace Blaven.Tests
{
    public static partial class TestData
    {
        public const string BlogKey = "Test_Blog-Key";

        public const string BlogKey1 = "Test Blog-Key 1";

        public const string BlogKey2 = "Test-Blog Key TWO";

        public const string BlogKey3 = "Blog-Test Key Trees";

        public static readonly string[] BlogKeys =
            new ReadOnlyCollection<string>(new[] { BlogKey1, BlogKey2, BlogKey3 }).ToArray();
    }
}