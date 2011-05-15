using System;
using System.Concurrency;
using System.IO;
using System.Linq;
using System.Reactive.Testing.Mocks;
using System.Threading;
using System.Windows;
using CoverRetriever.Model;
using CoverRetriever.ViewModel;
using Moq;
using NUnit.Framework;

namespace CoverRetriever.Test.ViewModel
{
	[TestFixture]
	public class CoverPreviewViewModelTest
	{
		[Test]
		public void Ctr_should_create_instance_and_not_throw_exceptions()
		{
			RemoteCover remoteCover = GetRemoteCover(Observable.Empty<Stream>());

			var target = new CoverPreviewViewModel(remoteCover);
			Assert.That(target.CoverSize, Is.EqualTo(remoteCover.CoverSize));
			Assert.That(target.SaveCoverCommand, Is.Not.Null);
			Assert.That(target.CloseCommand, Is.Not.Null);
			Assert.That(target.SaveCover, Is.Not.Null);
			Assert.That(target.ErrorMessage, Is.Null);
		}

		[Test]
		public void Ctr_should_begin_load_an_image()
		{
			var coverStreamMock = new Mock<IObservable<Stream>>();
			coverStreamMock.Setup(x => x.Subscribe(It.IsAny<IObserver<Stream>>()));
			
			var remoteCover = GetRemoteCover(coverStreamMock.Object);
			var target = new CoverPreviewViewModel(remoteCover);
			target.SetBusy(true, "UnitTest");

			Assert.That(target.IsBusy, Is.True);
			Assert.That(target.OperationName, Is.Not.Null);
			Assert.That(target.ErrorMessage, Is.Null);
			coverStreamMock.VerifyAll();
		}

		[Test]
		public void SaveCoverCommand_shoult_push_save_command()
		{
			var remoteCover = GetRemoteCover(Observable.Empty<Stream>());
			
			var target = new CoverPreviewViewModel(remoteCover);
			
			var testScheduler = new TestScheduler();
			var mockObservable = new MockObserver<RemoteCover>(testScheduler);

			target.SaveCover.Subscribe(mockObservable);

			target.SaveCoverCommand.Execute();

			Assert.That(mockObservable[0].Value.Value, Is.EqualTo(remoteCover));
		}
		
		[Test]
		public void Should_set_error_message_of_unable_to_load_image()
		{
			var remoteCover = GetRemoteCover(Observable.Throw<Stream>(new Exception("UnitTest")));
			
			var target = new CoverPreviewViewModel(remoteCover);
			
			while (target.IsBusy)
			{
				Thread.Sleep(100);
			}

			Assert.That(target.ErrorMessage, Is.EqualTo("UnitTest"));	
		}

		private RemoteCover GetRemoteCover(IObservable<Stream> coverStream)
		{
			return new RemoteCover(
				"123-78asd",
				"cover.png",
				new Size(200,200),
				new Size(100, 100),
				new Uri("http://www.google.com/"), 
				coverStream,
				coverStream);
		}
	}
}