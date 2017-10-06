using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Blaven.Testing;
using Xunit;

namespace Blaven.Tests
{
    public class KeyLockerTest
    {
        [Fact]
        public void RunWithLock_ParallelUsersAndSingleKey_ShouldNotHaveCollisions()
        {
            // Arrange
            const string Key = "TEST_KEY";
            var locker = new KeyLocker();

            bool isRunning = false;
            bool hasCollision = false;

            // Act
            ParallelUtility.RunParallelUsers(
                () =>
                    {
                        locker.RunWithLock(
                            Key,
                            () =>
                                {
                                    if (isRunning)
                                    {
                                        hasCollision = true;
                                    }

                                    isRunning = true;

                                    Thread.Sleep(100);

                                    isRunning = false;
                                });
                    });

            // Assert
            Assert.False(hasCollision);
        }

        [Fact]
        public void RunWithLock_ParallelUsersAndRandomKey_ShouldHaveCollisions()
        {
            // Arrange
            var locker = new KeyLocker();

            bool isRunning = false;
            bool hasCollision = false;

            // Act
            ParallelUtility.RunParallelUsers(
                () =>
                    {
                        var key = Convert.ToString(Guid.NewGuid());
                        locker.RunWithLock(
                            key,
                            () =>
                                {
                                    if (isRunning)
                                    {
                                        hasCollision = true;
                                    }

                                    isRunning = true;

                                    Thread.Sleep(100);

                                    isRunning = false;
                                });
                    });

            // Assert
            Assert.True(hasCollision);
        }

        [Fact]
        public void RunWithLock_ParallelUsersAndMultipleKeys_ShouldNotHaveCollisions()
        {
            // Arrange
            var locker = new KeyLocker();

            var isRunnings = new HashSet<string>();
            var hasCollisions = new Dictionary<string, int>();

            // Act
            ParallelUtility.RunParallelUsers(
                index =>
                    {
                        var key = $"TEST_KEY_{index}";

                        locker.RunWithLock(
                            key,
                            () =>
                                {
                                    bool isRunning;
                                    lock (isRunnings)
                                    {
                                        isRunning = isRunnings.Contains(key);
                                    }

                                    if (isRunning)
                                    {
                                        lock (hasCollisions)
                                        {

                                            if (!hasCollisions.ContainsKey(key))
                                            {
                                                hasCollisions[key] = 0;
                                            }

                                            hasCollisions[key] = (hasCollisions[key] + 1);
                                        }
                                    }

                                    lock (isRunnings)
                                    {
                                        isRunnings.Add(key);
                                    }

                                    Thread.Sleep(100);

                                    lock (isRunnings)
                                    {

                                        if (isRunnings.Contains(key))
                                        {
                                            isRunnings.Remove(key);
                                        }
                                    }
                                });
                    });

            // Assert
            var hasCollision = hasCollisions.Any(x => x.Value > 0);

            Assert.False(hasCollision);
        }
    }
}