using System;
using System.Collections.Generic;
using System.Reactive;
namespace Microsoft.Reactive.Testing
{
    internal class MockObserver<T> : ITestableObserver<T>
    {
        private TestScheduler scheduler;
        private List<Recorded<Notification<T>>> messages;
        public IList<Recorded<Notification<T>>> Messages
        {
            get
            {
                return messages;
            }
        }
        public MockObserver(TestScheduler scheduler)
        {
            if (scheduler == null)
            {
                throw new ArgumentNullException("scheduler");
            }
            this.scheduler = scheduler;
            messages = new List<Recorded<Notification<T>>>();
        }
        public void OnNext(T value)
        {
            messages.Add(new Recorded<Notification<T>>(scheduler.Clock, Notification.CreateOnNext<T>(value)));
        }
        public void OnError(Exception exception)
        {
            messages.Add(new Recorded<Notification<T>>(scheduler.Clock, Notification.CreateOnError<T>(exception)));
        }
        public void OnCompleted()
        {
            messages.Add(new Recorded<Notification<T>>(scheduler.Clock, Notification.CreateOnCompleted<T>()));
        }
    }
}
