using System;
using System.Configuration;
using System.Net;
using System.Threading;

namespace BloggerViewController.BackgroundPoller {
    class Program {
        static void Main(string[] args) {
            string updateUrl = ConfigurationManager.AppSettings["BloggerViewController.UpdateUrl"];

            var callback = new System.Threading.WaitCallback((url) => {
                WebRequest request = System.Net.WebRequest.Create(Convert.ToString(url));
                request.GetResponse();
            });
            System.Threading.ThreadPool.QueueUserWorkItem(callback, updateUrl);

            int timeout;
            int.TryParse(ConfigurationManager.AppSettings["BloggerViewController..AppTimout"], out timeout);

            Thread.Sleep(timeout * 60 * 1000);
        }
    }
}
