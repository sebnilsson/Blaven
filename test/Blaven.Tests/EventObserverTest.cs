using System;
using System.Threading;

using Moq;

using Xunit;

namespace Blaven.Tests
{
    public class EventObserverTest
    {
        [Fact]
        public void RunCount_NoParallelAndSingleKey_ShouldReturnOneRunCount()
        {
            // Arrange
            var obj = new FakeMethodObject(() => { Thread.Sleep(200); });

            var observer = new EventObserver();
            obj.OnMethodRan += observer.Handler;

            // Act
            obj.RunMethod("TEST_KEY");

            // Assert
            Assert.Equal(1, observer.RunCount);
        }

        [Fact]
        public void RunCount_ParallelUsersAndSingleKey_ShouldReturnOneRunCountPerUser()
        {
            // Arrange
            var obj = new FakeMethodObject(() => { Thread.Sleep(200); });

            var observer = new EventObserver();
            obj.OnMethodRan += observer.Handler;

            // Act
            ParallelUtility.RunParallelUsers(() => obj.RunMethod("TEST_KEY"));

            // Assert
            Assert.Equal(ParallelUtility.DefaultParallelUsersCount, observer.RunCount);
        }

        [Fact]
        public void CollisionCount_NoParallelAndSingleKey_ShouldReturnZero()
        {
            // Arrange
            var obj = new FakeMethodObject(() => { Thread.Sleep(200); });

            var observer = new EventObserver();
            obj.OnMethodRan += observer.Handler;

            // Act
            obj.RunMethod("TEST_KEY");

            // Assert
            Assert.Equal(0, observer.CollisionCount);
        }

        // TODO: Fix - Sometimes true
        [Fact]
        public void CollisionCount_ParallelUserAndSingleKey_ShouldReturnCollision()
        {
            // Arrange
            var obj = new FakeMethodObject(() => { Thread.Sleep(400); });
            
            var observer = new EventObserver();
            obj.OnMethodRan += observer.Handler;

            // Act
            ParallelUtility.RunParallelUsers(() => obj.RunMethod("TEST_KEY"));

            // Assert
            Assert.NotEqual(0, observer.CollisionCount);
        }

        [Fact]
        public void CollisionCount_ParallelUserAndSingleKeyAndKeyLocker_ShouldReturnNoCollision()
        {
            // Arrange
            var keyLocker = new KeyLocker();
            var obj = new FakeMethodObject(
                          key => keyLocker.RunWithLock(
                              key,
                              () =>
                                  {
                                      Thread.Sleep(200);
                                  }));

            var observer = new EventObserver();
            obj.OnMethodRan += observer.Handler;

            // Act
            ParallelUtility.RunParallelUsers(() => obj.RunMethod("TEST_KEY"));

            // Assert
            Assert.Equal(0, observer.CollisionCount);
        }

        [Fact]
        public void CollisionCount_ParallelUserAndDifferentKeys_ShouldReturnNoCollision()
        {
            // Arrange
            var obj = new FakeMethodObject(() => { Thread.Sleep(200); });

            var observer = new EventObserver();
            obj.OnMethodRan += observer.Handler;

            // Act
            ParallelUtility.RunParallelUsers(index => obj.RunMethod($"TEST_KEY_{index}"));

            // Assert
            Assert.Equal(0, observer.CollisionCount);
        }

        public class FakeMethodObject
        {
            private readonly Action<string> methodAction;

            public FakeMethodObject()
            {
            }

            public FakeMethodObject(Action methodAction)
            {
                this.methodAction = (_ => methodAction());
            }

            public FakeMethodObject(Action<string> methodAction)
            {
                this.methodAction = methodAction;
            }

            public event EventHandler<string> OnMethodRan;

            public virtual void RunMethod(string key)
            {
                this.methodAction?.Invoke(key);

                this.OnMethodRan?.Invoke(this, key);
            }
        }
    }
}