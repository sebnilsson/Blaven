namespace Blaven.Testing
{
    public static class TestUtility
    {
        public static string GetTestString(string name, string blogKey, int index, bool isUpdate = false)
        {
            var prefix = isUpdate ? "Updated" : null;

            var testString = $"{prefix}Test{name}_{blogKey}_{index}";
            return testString;
        }
    }
}