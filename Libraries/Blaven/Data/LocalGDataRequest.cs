using System;
using System.IO;

using Google.GData.Client;

namespace Blaven.Data {
    internal class LocalGDataRequest : IGDataRequest {
        private string _filePath;

        public LocalGDataRequest(string filePath) {
            _filePath = filePath;
        }

        #region IGDataRequest Members

        public GDataCredentials Credentials { get; set; }

        public void Execute() {
            
        }

        public System.IO.Stream GetRequestStream() {
            var stream = File.Open(_filePath, FileMode.Open);
            return stream;
        }

        public System.IO.Stream GetResponseStream() {
            var stream = File.Open(_filePath, FileMode.Open);
            return stream;
        }

        public DateTime IfModifiedSince { get; set; }

        public bool UseGZip { get; set; }

        #endregion
    }
}
