using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace Blaven.BackgroundPoller {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Main functionality starting");

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

            Console.WriteLine("Main functionality done");

            int elapsed = (int)stopwatch.ElapsedMilliseconds;
            int timeoutMs = Math.Max(0, timeout * 60 * 1000 - elapsed);

            Console.WriteLine("Sleep of {0} minutes starting", timeout);
            
            Thread.Sleep(timeoutMs);
            
            Console.WriteLine("Sleep done");
        }
    }
}
