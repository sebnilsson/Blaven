using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace Blaven.BackgroundPoller {
    class Program {
        static void Main(string[] args) {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            string updateUrl = ConfigurationManager.AppSettings["Blaven.UpdateUrl"];

            int timeout;
            int.TryParse(ConfigurationManager.AppSettings["Blaven.AppTimout"], out timeout);

            if(string.IsNullOrWhiteSpace(updateUrl)) {
                throw new ConfigurationErrorsException("The configuration-value for 'Blaven.UpdateUrl' was empty.");
            }

            if(timeout <= 0) {
                throw new ConfigurationErrorsException("The configuration-value for 'Blaven.AppTimout' was empty.");
            }
            
            WebRequest request = System.Net.WebRequest.Create(Convert.ToString(updateUrl));
            request.GetResponse();
            
            stopwatch.Stop();

            int elapsed = (int)stopwatch.ElapsedMilliseconds;
            int timeOut = Math.Max(0, timeout * 60 * 1000 - elapsed);

            Thread.Sleep(timeOut);
        }
    }
}
