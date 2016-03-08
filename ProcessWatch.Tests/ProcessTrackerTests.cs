using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProcessWatch.Interfaces;
using System;
using System.Collections.Generic;

namespace ProcessWatch.Tests
{
    [TestClass]
    public class ProcessTrackerTests
    {
        private ProcessProviderMock provider;
        private ControllableProcessNotifier notifier;
        private ControllableWindowNotifier windowNotifier;
        private ProcessTrackerSource<ControllableProcessNotifier, ControllableWindowNotifier> source;

        public string Path = "Tests";

        [TestInitialize]
        public void Init()
        {
            provider = new ProcessProviderMock();
            notifier = new ControllableProcessNotifier();
            windowNotifier = new ControllableWindowNotifier();
            source = new ProcessTrackerSource<ControllableProcessNotifier, ControllableWindowNotifier>
                (provider, notifier, windowNotifier);
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
            var tracker = source.CreateTracker();
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
            var tracker = source.CreateTracker();
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
            var tracker = source.CreateTracker();
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

        [TestMethod]
        public void NotifyActiveProcessChanged()
        {
            //Arrange
            var tracker = source.CreateTracker();
            var raiseCount = 0;
            var action = new Action(() => { raiseCount++; });
            windowNotifier.ActiveWindowId = 3;

            int callOrder = 0;
            var prog1 = new Mock<ITrackableProgram>();
            var prog2 = new Mock<ITrackableProgram>();
            prog1.Setup(p => p.Path).Returns(nameof(prog1));
            prog2.Setup(p => p.Path).Returns(nameof(prog2));
            prog1.Setup(p => p.Start(It.IsAny<DateTime>())).Callback(() => Assert.AreEqual(callOrder++, 0));
            prog2.Setup(p => p.Start(It.IsAny<DateTime>())).Callback(() => Assert.AreEqual(callOrder++, 1));
            prog1.Setup(p => p.Activate()).Callback(() => Assert.AreEqual(callOrder++, 2));
            prog2.Setup(p => p.Activate()).Callback(() => Assert.AreEqual(callOrder++, 3));

            tracker.AddProgram(prog1.Object);
            tracker.AddProgram(prog2.Object);

            var dt = default(DateTime);

            //Act
            notifier.InvokeProcessStarted(new ProcessStartEventArgs(nameof(prog1), 1, dt));
            notifier.InvokeProcessStarted(new ProcessStartEventArgs(nameof(prog2), 2, dt));
            windowNotifier.InvokeActiveWindowChanged(1);
            windowNotifier.InvokeActiveWindowChanged(2);
        }
    }
}