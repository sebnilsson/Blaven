using System;
using System.Linq;
using System.Threading;

using Blaven.Tests;

namespace Blaven
{
    public static class TestUtility
    {
        public const int SingleKeyUserCount = 20;

        public const int MultipleKeysUserCount = 10;

        public static Action[] GetParallelActionsForSingleKey(Action body, int userCount = SingleKeyUserCount)
        {
            var actions = Enumerable.Range(0, userCount).Select(
                _ =>
                    {
                        Action action = (() =>
                            {
                                int randomSleep = GetRandomSleep();
                                Thread.Sleep(randomSleep);

                                body();
                            });
                        return action;
                    }).ToArray();
            return actions;
        }

        public static Action[] GetParallelActionsForMultipleKeys(
            Action<string> body,
            int userCount = MultipleKeysUserCount)
        {
            var actions = TestData.BlogKeys.SelectMany(
                blogKey => Enumerable.Range(0, userCount).Select(
                    _ =>
                        {
                            Action action = (() =>
                                {
                                    int randomSleep = GetRandomSleep();
                                    Thread.Sleep(randomSleep);

                                    body(blogKey);
                                });
                            return action;
                        })).ToArray();
            return actions;
        }

        public static int GetRandomSleep()
        {
            var random = new Random();

            int randomSleep = random.Next(0, 100);
            return randomSleep;
        }
    }
}