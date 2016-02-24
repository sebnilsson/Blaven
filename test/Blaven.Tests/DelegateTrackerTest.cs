using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace Blaven.Tests
{
    public class DelegateTrackerTest
    {
        private const int TestThreadId1 = 100;

        private const int TestThreadId2 = 200;

        [Fact]
        public void KeyCollisionCount_Collisions_ShouldReturnCollisionCount()
        {
            // Arrange
            var action = GetTestAction();
            var tracking = GetTestDelegateTrackingWithEvents(action);

            // Act
            action(TestData.BlogKey1);

            // Assert
            int collisionCount = tracking.KeyCollisionCount[TestData.BlogKey1];

            Assert.Equal(2, collisionCount);
        }

        [Fact]
        public void KeyRunCount_Events_ShouldReturnCount()
        {
            // Arrange
            var action = GetTestAction();
            var tracking = GetTestDelegateTrackingWithEvents(action);

            // Act
            action(TestData.BlogKey1);

            // Assert
            int runCount = tracking.KeyRunCount[TestData.BlogKey1];

            Assert.Equal(3, runCount);
        }

        [Fact]
        public void KeyRunOtherCount_Events_ShouldReturnCount()
        {
            // Arrange
            var action = GetTestAction();
            var tracking = GetTestDelegateTrackingWithEvents(action);

            // Act
            action(TestData.BlogKey1);

            // Assert
            int runOtherCount = tracking.KeyRunOtherCount[TestData.BlogKey1];

            Assert.Equal(2, runOtherCount);
        }

        [Fact]
        public void RunCount_Events_ShouldReturnCount()
        {
            // Arrange
            var action = GetTestAction();
            var tracking = GetTestDelegateTrackingWithEvents(action);

            // Act
            action(TestData.BlogKey1);

            // Assert
            Assert.Equal(tracking.Events.Count, tracking.RunCount);
        }

        private static Action<string> GetTestAction()
        {
            Action<string> action = _ => { };
            return action;
        }

        private static DelegateTracker<string> GetTestDelegateTrackingWithEvents(Action<string> action)
        {
            var events =
                new[]
                    {
                        new DelegateTrackerEvent<string>(
                            TestData.BlogKey1,
                            TestThreadId1,
                            startedAt: GetTestDateTime(10, 0, 0),
                            endedAt: GetTestDateTime(10, 0, 10)),
                        new DelegateTrackerEvent<string>(
                            TestData.BlogKey1,
                            TestThreadId2,
                            startedAt: GetTestDateTime(10, 0, 1),
                            endedAt: GetTestDateTime(10, 0, 8)),
                        new DelegateTrackerEvent<string>(
                            TestData.BlogKey1,
                            TestThreadId1,
                            startedAt: GetTestDateTime(10, 0, 2),
                            endedAt: GetTestDateTime(10, 0, 7)),
                        new DelegateTrackerEvent<string>(
                            TestData.BlogKey2,
                            TestThreadId2,
                            startedAt: GetTestDateTime(10, 0, 0),
                            endedAt: GetTestDateTime(10, 0, 10)),
                        new DelegateTrackerEvent<string>(
                            TestData.BlogKey2,
                            TestThreadId1,
                            startedAt: GetTestDateTime(10, 0, 3),
                            endedAt: GetTestDateTime(10, 0, 6)),
                        new DelegateTrackerEvent<string>(
                            TestData.BlogKey2,
                            TestThreadId2,
                            startedAt: GetTestDateTime(11, 0, 0),
                            endedAt: GetTestDateTime(11, 0, 10)),
                        new DelegateTrackerEvent<string>(
                            TestData.BlogKey2,
                            TestThreadId1,
                            startedAt: GetTestDateTime(12, 0, 3),
                            endedAt: GetTestDateTime(12, 0, 6))
                    }.ToList();

            var delegateTracker = GetTestDelegateTracking(events);

            action.WithTracking(delegateTracker);

            return delegateTracker;
        }

        private static DateTime GetTestDateTime(int hours, int minutes, int seconds)
        {
            var dateTime = new DateTime(2015, 1, 1, hours, minutes, seconds);
            return dateTime;
        }

        private static DelegateTracker<string> GetTestDelegateTracking(IEnumerable<DelegateTrackerEvent<string>> events)
        {
            var eventList = new List<DelegateTrackerEvent<string>>(events);

            var delegateTracker = new DelegateTracker<string>(eventList);
            return delegateTracker;
        }
    }
}