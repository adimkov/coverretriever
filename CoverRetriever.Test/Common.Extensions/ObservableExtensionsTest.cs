// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableExtensionsTest.cs" author="Anton Dimkov">
//   Copyright (c) Anton Dimkov 2011. All rights reserved.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CoverRetriever.Test.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Concurrency;
    using System.Linq;
    using System.Reactive.Testing.Mocks;


    using NUnit.Framework;

    [TestFixture]
    public class ObservableExtensionsTest
    {
        [Test]
        public void should_call_complete_action_on_observable_complete()
        {
            var isCalled = false;
            var scheduler = new TestScheduler();
            var observer = new MockObserver<Unit>(scheduler);
            var observable =  new MockObservable<Unit>(
                new Notification<Unit>.OnNext(new Unit()),
                new Notification<Unit>.OnNext(new Unit()),
                new Notification<Unit>.OnCompleted());

            observable.Completed(() => isCalled = true).Run(observer);

            Assert.IsTrue(isCalled);
            Assert.That(observer[0].Value.Kind, Is.EqualTo(NotificationKind.OnNext));
            Assert.That(observer[1].Value.Kind, Is.EqualTo(NotificationKind.OnNext));
            Assert.That(observer[2].Value.Kind, Is.EqualTo(NotificationKind.OnCompleted));
        }

        [Test]
        public void should_not_call_complete_if_observable_does_not_finished()
        {
            var isCalled = false;
            var scheduler = new TestScheduler();
            var observer = new MockObserver<Unit>(scheduler);
            var observable = new MockObservable<Unit>(
                new Notification<Unit>.OnNext(new Unit()),
                new Notification<Unit>.OnNext(new Unit()));

            observable.Completed(() => isCalled = true).Timeout(TimeSpan.FromSeconds(1)).Run(observer);

            Assert.IsFalse(isCalled);
            Assert.That(observer[0].Value.Kind, Is.EqualTo(NotificationKind.OnNext));
            Assert.That(observer[1].Value.Kind, Is.EqualTo(NotificationKind.OnNext));
        }

        [Test]
        public void should_not_call_complete_if_observable_return_an_error()
        {
            var isCalled = false;
            var scheduler = new TestScheduler();
            var observer = new MockObserver<Unit>(scheduler);
            var observable = new MockObservable<Unit>(
                new Notification<Unit>.OnNext(new Unit()),
                new Notification<Unit>.OnError(new Exception()));

            observable.Completed(() => isCalled = true).Run(observer);

            Assert.IsFalse(isCalled);
            Assert.That(observer[0].Value.Kind, Is.EqualTo(NotificationKind.OnNext));
            Assert.That(observer[1].Value.Kind, Is.EqualTo(NotificationKind.OnError));
        }

        [Test]
        public void should_call_complete_if_observable_is_empty()
        {
            var isCalled = false;
            var observable = Observable.Empty<Unit>();

            observable.Completed(() => isCalled = true).Run();

            Assert.IsTrue(isCalled);
        }
    }
}