﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blaven.Tests
{
    public static class TestUtility
    {
        public const int ParallelUsersCount = 5;

        public static void RunParallelUsers(Action action, int userCount = ParallelUsersCount)
        {
            Parallel.For(
                0,
                userCount,
                _ =>
                    {
                        int randomSleep = GetRandomSleep();
                        Thread.Sleep(randomSleep);

                        action();
                    });
        }

        public static int GetRandomSleep()
        {
            var random = new Random();

            int randomSleep = random.Next(0, 100);
            return randomSleep;
        }
    }
}