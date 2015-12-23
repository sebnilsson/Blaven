using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using RestSharp;

namespace Blaven.DataSources.GitHub
{
    public class GitHubApiHelper
    {
        private const string ApiBaseUrl = "https://api.github.com";

        private readonly RestClient restClient;

        private readonly BlavenBlogSetting setting;

        private readonly string contentsApiPath;

        public GitHubApiHelper(BlavenBlogSetting setting)
        {
            this.setting = setting;

            var authenticator = new HttpBasicAuthenticator(setting.Username, setting.Password);
            this.restClient = new RestClient(ApiBaseUrl) { UserAgent = "Blaven", Authenticator = authenticator };

            this.contentsApiPath = this.GetContentsApiPath();
        }

        public IEnumerable<GitHubContent> GetFileList(params string[] acceptableExtensions)
        {
            var contents = this.GetContents(this.setting.DataSourceUri);

            //var remainingHeader = response.Headers.FirstOrDefault(x => x.Name == "X-RateLimit-Remaining");
            //var remaining = (remainingHeader != null) ? remainingHeader.Value : null;

            return from item in contents
                   let extension = Path.GetExtension(item.Name)
                   where item.Type == GitHubContent.FileType && acceptableExtensions.Contains(extension)
                   select item;
        }

        public string GetFileContent(string fullPath)
        {
            var content = this.GetContents(fullPath).FirstOrDefault();
            if (content == null)
            {
                return null;
            }

            var base64Content = Convert.FromBase64String(content.Content);

            var encoding = Encoding.UTF8;
            var bom = encoding.GetPreamble();

            if (bom.Length > 0 && base64Content.Take(bom.Length).SequenceEqual(bom))
            {
                base64Content = base64Content.Skip(bom.Length).ToArray();
            }

            return encoding.GetString(base64Content);
        }

        private IEnumerable<GitHubContent> GetContents(string fullPath)
        {
            var request = new RestRequest(this.contentsApiPath) { RequestFormat = DataFormat.Json };
            request.AddParameter("path", fullPath);

            var response = this.restClient.Execute(request);
            if (response.ErrorException != null)
            {
                throw new GitHubApiHelperException(this.setting, response.ErrorException);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var result = JsonHelper.Deserialize<dynamic>(response.Content);
                string message = Convert.ToString(result.message);

                var innerException = new Exception(message);
                throw new GitHubApiHelperException(this.setting, innerException);
            }

            return GitHubContent.Parse(response.Content) ?? Enumerable.Empty<GitHubContent>();
        }

        private string GetContentsApiPath()
        {
            var args =
                new[] { this.setting.Username, this.setting.DataSourceId }.Select(x => x.Trim('/')).ToArray<object>();
            return string.Format("/repos/{0}/{1}/contents", args);
        }
    }
}