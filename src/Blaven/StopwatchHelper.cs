using System;
using System.Diagnostics;

namespace Blaven
{
    public static class StopwatchHelper
    {
        public static TimeSpan PerformMeasuredAction(Action action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            action();

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }
}