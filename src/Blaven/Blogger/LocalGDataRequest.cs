using System;
using System.IO;

using Google.GData.Client;

namespace Blaven.Blogger
{
    internal class LocalGDataRequest : IGDataRequest
    {
        private static object streamLock = new object();

        private string filePath;

        public LocalGDataRequest(string path)
        {
            filePath = path;
        }

        #region IGDataRequest Members

        public GDataCredentials Credentials { get; set; }

        public DateTime IfModifiedSince { get; set; }

        public bool UseGZip { get; set; }

        public void Execute()
        {
        }

        public Stream GetRequestStream()
        {
            var memoryStream = new MemoryStream();
            lock (streamLock)
            {
                using (var fileStream = File.Open(filePath, FileMode.Open))
                {
                    fileStream.CopyTo(memoryStream);
                }
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        public Stream GetResponseStream()
        {
            var memoryStream = new MemoryStream();
            lock (streamLock)
            {
                using (var fileStream = File.Open(filePath, FileMode.Open))
                {
                    fileStream.CopyTo(memoryStream);
                }
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        #endregion
    }
}