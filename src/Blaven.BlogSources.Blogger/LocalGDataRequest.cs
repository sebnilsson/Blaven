using System;
using System.IO;

using Google.GData.Client;

namespace Blaven.Sources.Blogger
{
    internal class LocalGDataRequest : IGDataRequest
    {
        private static readonly object StreamLock = new object();

        private readonly string filePath;

        public LocalGDataRequest(string path)
        {
            this.filePath = path;
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
            lock (StreamLock)
            {
                using (var fileStream = File.Open(this.filePath, FileMode.Open))
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
            lock (StreamLock)
            {
                using (var fileStream = File.Open(this.filePath, FileMode.Open))
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