using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CoverRetriever.Controls;
using Moq;
using NUnit.Framework;

namespace CoverRetriever.Test.Controls
{
	[TestFixture]
	public class ImageAsyncSourceTest
	{
		[Test]
		[STAThread]
		public void Should_call_subscribe_on_property_setted()
		{
			var asyncSource = new Mock<IObservable<Stream>>();
			asyncSource.Setup(x => x.Subscribe(It.IsAny<IObserver<Stream>>())).Verifiable();
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

			var image = new Image();
			image.SetValue(ImageAsyncSource.AsyncSourceProperty, asyncSource);
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