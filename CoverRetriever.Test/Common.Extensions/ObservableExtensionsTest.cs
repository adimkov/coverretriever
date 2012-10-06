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
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    [TestFixture]
    public class ObservableExtensionsTest
    {
        [Test]
        public void should_call_complete_action_on_observable_complete()
        {
            var isCalled = false;
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<Unit>();
            var observable =
                scheduler.CreateColdObservable(
                    new Recorded<Notification<Unit>>(0, Notification.CreateOnNext(new Unit())),
                    new Recorded<Notification<Unit>>(1, Notification.CreateOnNext(new Unit())),
                    new Recorded<Notification<Unit>>(2, Notification.CreateOnCompleted<Unit>()));


            observable.Completed(() => isCalled = true).Subscribe(observer);
            scheduler.Start();
            
            Assert.IsTrue(isCalled);
            Assert.That(observer.Messages[0].Value.Kind, Is.EqualTo(NotificationKind.OnNext));
            Assert.That(observer.Messages[1].Value.Kind, Is.EqualTo(NotificationKind.OnNext));
            Assert.That(observer.Messages[2].Value.Kind, Is.EqualTo(NotificationKind.OnCompleted));
        }

        [Test]
        public void should_not_call_complete_if_observable_does_not_finished()
        {
            var isCalled = false;
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<Unit>();

            var observable =
                 scheduler.CreateColdObservable(
                     new Recorded<Notification<Unit>>(0, Notification.CreateOnNext(new Unit())),
                     new Recorded<Notification<Unit>>(1, Notification.CreateOnNext(new Unit())));

            observable.Completed(() => isCalled = true).Subscribe(observer);
            scheduler.Start();

            Assert.IsFalse(isCalled);
            Assert.That(observer.Messages[0].Value.Kind, Is.EqualTo(NotificationKind.OnNext));
            Assert.That(observer.Messages[1].Value.Kind, Is.EqualTo(NotificationKind.OnNext));
        }

        [Test]
        public void should_not_call_complete_if_observable_return_an_error()
        {
            var isCalled = false;
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<Unit>();

            var observable =
                 scheduler.CreateColdObservable(
                     new Recorded<Notification<Unit>>(0, Notification.CreateOnNext(new Unit())),
                     new Recorded<Notification<Unit>>(1, Notification.CreateOnError<Unit>(new Exception())));

            observable.Completed(() => isCalled = true).Subscribe(observer);
            scheduler.Start();

            Assert.IsFalse(isCalled);
            Assert.That(observer.Messages[0].Value.Kind, Is.EqualTo(NotificationKind.OnNext));
            Assert.That(observer.Messages[1].Value.Kind, Is.EqualTo(NotificationKind.OnError));
        }

        [Test]
        public void should_call_complete_if_observable_is_empty()
        {
            var isCalled = false;
            var observable = Observable.Empty<Unit>();

            observable.Completed(() => isCalled = true).FirstOrDefault();

            Assert.IsTrue(isCalled);
        }
    }
}