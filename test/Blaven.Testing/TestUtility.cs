namespace Blaven.Tests
{
    public static class TestUtility
    {
        public static string GetTestString(string name, string blogKey, int index, bool isUpdate = false)
        {
            string prefix = isUpdate ? "Updated" : null;

            string testString = $"{prefix}Test{name}_{blogKey}_{index}";
            return testString;
        }
    }
}