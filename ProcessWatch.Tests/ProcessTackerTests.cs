using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace ProcessWatch.Tests
{
    [TestClass]
    public class ProcessTackerTests
    {
        private ProcessProviderMock provider;
        private ControllableProcessNotifier notifier;
        private ProcessTrackerFactory factory;

        [TestInitialize]
        public void Init()
        {
            provider = new ProcessProviderMock();
            notifier = new ControllableProcessNotifier();
            factory = new ProcessTrackerFactory(provider, notifier);
        }

        [TestCleanup]
        public void Cleanup()
        {
            notifier.Dispose();
        }

        private ITrackableProgram GetMockedProgram(Action start)
        {
            var trackedProgram = new Mock<ITrackableProgram>();
            trackedProgram.SetupGet(x => x.Path).Returns("test");
            trackedProgram.Setup(x => x.Start(It.IsAny<DateTime>()))
                            .Callback(start);

            return trackedProgram.Object;
        }

        private ITrackableProgram GetSimpleProgram(Action start)
        {
            return new SimpleTrackableProgram("test", start, () => { });
        }

        [TestMethod]
        public void NotifyAboutNewlyCreatedProcesses()
        {
            //Arrange
            var tracker = factory.CreateTracker();
            var startRaised = false;
            var action = new Action(() => { startRaised = true; });
            var mock = GetSimpleProgram(action);
            tracker.AddProgram(mock);

            var startEventArgs = new ProcessStartEventArgs("test", 1, default(DateTime));

            //Act
            notifier.InvokeProcessStarted(startEventArgs);

            //Assert
            Assert.IsTrue(startRaised);
        }

        [TestMethod]
        public void NotifyAboutAlreadyStartedProcesses()
        {
            var exDate = new DateTime(1);
            provider.Processes.Add(new TrackedProcessInfo(1, "test", exDate));
            var tracker = factory.CreateTracker();
            var startRaised = false;
            var action = new Action(() => { startRaised = true; });
            var program = this.GetSimpleProgram(action);

            tracker.AddProgram(program);

            Assert.IsTrue(startRaised);
        }

        [TestMethod]
        public void NotifyMultipleSubscribers()
        {
            //Arrange
            var tracker = factory.CreateTracker();
            var raiseCount = 0;
            var action = new Action(() => { raiseCount++; });
            tracker.AddProgram(GetSimpleProgram(action));
            tracker.AddProgram(GetSimpleProgram(action));

            var startEventArgs = new ProcessStartEventArgs("test", 1, default(DateTime));

            //Act
            notifier.InvokeProcessStarted(startEventArgs);

            //Assert
            Assert.AreEqual(raiseCount, 2);
        }
    }
}