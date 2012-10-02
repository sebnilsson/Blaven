using System;
using System.IO;

using Google.GData.Client;

namespace Blaven.Blogger {
    internal class LocalGDataRequest : IGDataRequest {
        private static object _streamLock = new object();

        private string _filePath;

        public LocalGDataRequest(string filePath) {
            _filePath = filePath;
        }

        #region IGDataRequest Members

        public GDataCredentials Credentials { get; set; }

        public void Execute() {
            
        }

        public System.IO.Stream GetRequestStream() {
            var memoryStream = new MemoryStream();
            lock(_streamLock) {
                using(var fileStream = File.Open(_filePath, FileMode.Open)) {
                    fileStream.CopyTo(memoryStream);
                }
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        public System.IO.Stream GetResponseStream() {
            var memoryStream = new MemoryStream();
            lock(_streamLock) {
                using(var fileStream = File.Open(_filePath, FileMode.Open)) {
                    fileStream.CopyTo(memoryStream);
                }
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        public DateTime IfModifiedSince { get; set; }

        public bool UseGZip { get; set; }

        #endregion
    }
}
