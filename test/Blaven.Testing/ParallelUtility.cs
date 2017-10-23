using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blaven.Testing
{
    public static class ParallelUtility
    {
        public const int DefaultParallelUsersCount = 5;

        public static int GetRandomSleep()
        {
            var random = new Random();

            var randomSleep = random.Next(0, 100);
            return randomSleep;
        }

        public static void RunParallelUsers(Action action, int userCount = DefaultParallelUsersCount)
        {
            RunParallelUsers(_ => action(), userCount);
        }

        public static void RunParallelUsers(Action<int> action, int userCount = DefaultParallelUsersCount)
        {
            Parallel.For(
                0,
                userCount,
                i =>
                {
                    var randomSleep = GetRandomSleep();
                    Thread.Sleep(randomSleep);

                    action(i);
                });
        }
    }
}