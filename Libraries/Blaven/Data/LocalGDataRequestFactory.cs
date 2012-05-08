using System;
using Google.GData.Client;

namespace Blaven.Data {
    internal class LocalGDataRequestFactory : IGDataRequestFactory {
        #region IGDataRequestFactory Members

        public IGDataRequest CreateRequest(GDataRequestType type, Uri uriTarget) {
            string localPath = uriTarget.LocalPath;
            int queryStringIndex = localPath.LastIndexOf('?');

            string cleanLocalPath = localPath.Substring(0, queryStringIndex);
            return new LocalGDataRequest(cleanLocalPath);
        }

        public bool UseSSL { get; set; }

        public bool UseGZip { get; set; }

        #endregion
    }
}
