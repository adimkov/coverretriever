using System;
using System.Concurrency;
using System.Linq;
using System.Reactive.Testing.Mocks;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using CoverRetriever.Model;
using CoverRetriever.Service;
using CoverRetriever.ViewModel;
using Moq;
using NUnit.Framework;

namespace CoverRetriever.Test.ViewModel
{
	[TestFixture]
	public class CoverPreviewViewModelTest
	{
		private const string CoverUri = "/CoverRetriever.Test; component/Input.CoverPreviewViewModelTest/Penguins.jpg";
		private const string InvalidCoverUri = "/CoverRetriever.Test; component/Input.CoverPreviewViewModelTest/PenguinsNotValid.jpg";

		[Test]
		public void Ctr_should_create_instance_and_not_throw_exceptions()
		{
			CoverPreviewViewModel target = null;
			Mock<IImageDownloader> imageDownloaderMock = SetupImageDownloaderMock();

			RemoteCover remoteCover = GetRemoteCover(CoverUri);

			Assert.DoesNotThrow(() => target = new CoverPreviewViewModel(imageDownloaderMock.Object, remoteCover));
			Assert.That(target.CoverSize, Is.EqualTo(remoteCover.CoverSize));
			Assert.That(target.CoverImage, Is.Not.Null);
			Assert.That(target.SaveCoverCommand, Is.Not.Null);
			Assert.That(target.CloseCommand, Is.Not.Null);
			Assert.That(target.SaveCover, Is.Not.Null);
			Assert.That(target.ErrorMessage, Is.Null);
		}

		[Test]
		public void Ctr_should_begin_load_an_image()
		{
			var remoteCover = GetRemoteCover(CoverUri);
			Mock<IImageDownloader> imageDownloaderMock = SetupImageDownloaderMock();
			
			var target = new CoverPreviewViewModel(imageDownloaderMock.Object, remoteCover);
			target.SetBusy(true, "UnitTest");

			Assert.That(target.IsBusy, Is.True);
			Assert.That(target.OperationName, Is.Not.Null);
			Assert.That(target.ErrorMessage, Is.Null);
		}

		[Test]
		public void SaveCoverCommand_shoult_push_save_command()
		{
			var remoteCover = GetRemoteCover(CoverUri);
			Mock<IImageDownloader> imageDownloaderMock = SetupImageDownloaderMock();
			
			var target = new CoverPreviewViewModel(imageDownloaderMock.Object, remoteCover);
			
			var testScheduler = new TestScheduler();
			var mockObservable = new MockObserver<RemoteCover>(testScheduler);

			target.SaveCover.Subscribe(mockObservable);

			target.SaveCoverCommand.Execute();

			Assert.That(mockObservable[0].Value.Value, Is.EqualTo(remoteCover));
		}

		[Test]
		public void Should_set_error_message_of_unable_to_load_image()
		{
			var remoteCover = GetRemoteCover(InvalidCoverUri);
			var imageDownloaderMock = new Mock<IImageDownloader>();
			imageDownloaderMock.Setup(x => x.DownloadImage(It.IsAny<Uri>()))
				.Returns(Observable.Throw<double>(new Exception("UnitTest")));
			
			var target = new CoverPreviewViewModel(imageDownloaderMock.Object, remoteCover);
			
			while (target.IsBusy)
			{
				Thread.Sleep(100);
			}

			Assert.That(target.ErrorMessage, Is.EqualTo("UnitTest"));	
		}

		private RemoteCover GetRemoteCover(string coverUri)
		{
			return new RemoteCover(
				"UnitTestImage",
				new Uri(coverUri, UriKind.Relative),
				new Size(500, 500));
		}
		
		private Mock<IImageDownloader> SetupImageDownloaderMock()
		{
			var imageDownloaderMock = new Mock<IImageDownloader>();
			imageDownloaderMock.Setup(x => x.DownloadImage(It.IsAny<Uri>()))
				.Returns(Observable.Empty<double>);
			imageDownloaderMock.Setup(x => x.BitmapImage)
				.Returns(new BitmapImage());
			return imageDownloaderMock;
		}
	}
}