using System;

using Google.GData.Client;

namespace Blaven.DataSources.Blogger
{
    internal class LocalGDataRequestFactory : IGDataRequestFactory
    {
        #region IGDataRequestFactory Members

        public bool UseSSL { get; set; }

        public bool UseGZip { get; set; }

        public IGDataRequest CreateRequest(GDataRequestType type, Uri uriTarget)
        {
            string localPath = uriTarget.LocalPath;
            int queryStringIndex = localPath.LastIndexOf('?');

            string cleanLocalPath = (queryStringIndex >= 0) ? localPath.Substring(0, queryStringIndex) : localPath;
            return new LocalGDataRequest(cleanLocalPath);
        }

        #endregion
    }
}