using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using DropNet;

using RestSharp;

namespace Blaven.DataSources.Dropbox
{
    public class DropboxApiHelper
    {
        private const string ApiBaseUrl = "https://api.dropbox.com";

        private readonly RestClient restClient;

        private readonly BlavenBlogSetting setting;

        private readonly string contentsApiPath;

        public DropboxApiHelper(BlavenBlogSetting setting)
        {
            var client = new DropNetClient(setting.Username, setting.Password);


            this.setting = setting;

            var authenticator = new HttpBasicAuthenticator(setting.Username, setting.Password);
            this.restClient = new RestClient(ApiBaseUrl) { UserAgent = "Blaven", Authenticator = authenticator };

            this.contentsApiPath = this.GetContentsApiPath();
        }

        public IEnumerable<DropboxContent> GetFileList(params string[] acceptableExtensions)
        {
            var contents = this.GetContents(this.setting.DataSourceUri);

            //var remainingHeader = response.Headers.FirstOrDefault(x => x.Name == "X-RateLimit-Remaining");
            //var remaining = (remainingHeader != null) ? remainingHeader.Value : null;

            return from item in contents
                   let extension = Path.GetExtension(item.Name)
                   where item.Type == DropboxContent.FileType && acceptableExtensions.Contains(extension)
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

        private IEnumerable<DropboxContent> GetContents(string fullPath)
        {
            var request = new RestRequest(this.contentsApiPath) { RequestFormat = DataFormat.Json };
            request.AddParameter("path", fullPath);

            var response = this.restClient.Execute(request);
            if (response.ErrorException != null)
            {
                throw new DropboxApiHelperException(this.setting, response.ErrorException);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var result = JsonHelper.Deserialize<dynamic>(response.Content);
                string message = Convert.ToString(result.message);

                var innerException = new Exception(message);
                throw new DropboxApiHelperException(this.setting, innerException);
            }

            return DropboxContent.Parse(response.Content) ?? Enumerable.Empty<DropboxContent>();
        }

        private string GetContentsApiPath()
        {
            var args =
                new[] { this.setting.Username, this.setting.DataSourceId }.Select(x => x.Trim('/')).ToArray<object>();
            return string.Format("/repos/{0}/{1}/contents", args);
        }
    }
}