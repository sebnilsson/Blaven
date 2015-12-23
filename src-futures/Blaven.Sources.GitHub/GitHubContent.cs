using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Newtonsoft.Json;

namespace Blaven.DataSources.GitHub
{
    [DebuggerDisplay("[{Type}] {Path}")]
    public class GitHubContent
    {
        public const string DirectoryType = "dir";

        public const string FileType = "file";

        public string Type { get; set; }

        public string Encoding { get; set; }

        public long Size { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Content { get; set; }

        public string Sha { get; set; }

        public string Url { get; set; }

        [JsonProperty(PropertyName = "git_url")]
        public string GitUrl { get; set; }

        [JsonProperty(PropertyName = "html_url")]
        public string HtmlUrl { get; set; }

        [JsonProperty(PropertyName = "_links")]
        public Dictionary<string, string> Links { get; set; }

        public static IEnumerable<GitHubContent> Parse(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return Enumerable.Empty<GitHubContent>();
            }

            IEnumerable<GitHubContent> result;
            try
            {
                result = JsonHelper.Deserialize<IEnumerable<GitHubContent>>(content);
            }
            catch (Exception)
            {
                result = new[] { JsonHelper.Deserialize<GitHubContent>(content) };
            }
            return result;
        }
    }
}