using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CoverRetriever.Controls;
using Moq;
using NUnit.Framework;

namespace CoverRetriever.Test.Controls
{
    using System.Reactive.Linq;

    using Microsoft.Reactive.Testing;

    [TestFixture]
    public class ImageAsyncSourceTest
    {
        [Test]
        [STAThread]
        public void Should_call_subscribe_on_property_setted()
        {
            var asyncSource = new Mock<IObservable<Stream>>();
            var stream = new Mock<Stream>();
            asyncSource.Setup(x => x.Subscribe(It.IsAny<IObserver<Stream>>()))
                .Returns(stream.Object)
                .Verifiable();
            var image = new Image();
            image.SetValue(ImageAsyncSource.AsyncSourceProperty, asyncSource.Object);

            asyncSource.Verify();
        }

        [Test]
        [STAThread]
        public void Should_set_source_for_image_from_IObservable()
        {
            var stream = new Mock<Stream>();
            var asyncSource = Observable.Return(stream.Object);
            var scheduler = new TestScheduler();

            var image = new Image();
            image.SetValue(ImageAsyncSource.DispatcherProperty, scheduler);
            image.SetValue(ImageAsyncSource.AsyncSourceProperty, asyncSource);
            scheduler.Start();
            var imageSource = image.Source as BitmapImage;

            Assert.That(imageSource, Is.Not.Null);
            Assert.That(imageSource.StreamSource, Is.EqualTo(stream.Object));
        }

        [Test]
        [STAThread]
        public void Should_throttle_error_if_image_cannot_download()
        {
            var asyncSource = Observable.Throw<Stream>(new Exception("Unit test exception"));
            var image = new Image();
            image.SetValue(ImageAsyncSource.AsyncSourceProperty, asyncSource);
        }
    }
}