using System;
using System.Concurrency;
using System.Reactive.Testing.Mocks;
using System.Windows;

using CoverRetriever.Model;
using CoverRetriever.ViewModel;

using NUnit.Framework;

namespace CoverRetriever.Test.ViewModel
{
	[TestFixture]
	public class CoverPreviewViewModelTest
	{
		private const string CoverUri = "/CoverRetriever.Test; component/Input.CoverPreviewViewModelTest/Penguins.jpg";

		[Test]
		public void Ctr_should_create_instance_and_not_throw_exceptions()
		{
			CoverPreviewViewModel target = null;
			RemoteCover remoteCover = GetRemoteCover();

			Assert.DoesNotThrow(() => target = new CoverPreviewViewModel(remoteCover));
			Assert.That(target.CoverSize, Is.EqualTo(remoteCover.CoverSize));
			Assert.That(target.CoverImage, Is.Not.Null);
			Assert.That(target.SaveCoverCommand, Is.Not.Null);
			Assert.That(target.CloseCommand, Is.Not.Null);
			Assert.That(target.SaveCover, Is.Not.Null);
		}

		

		[Test]
		public void Ctr_should_begin_load_an_image()
		{
			var remoteCover = GetRemoteCover();
			
			var target = new CoverPreviewViewModel(remoteCover);

			Assert.That(target.IsBusy, Is.True);
			Assert.That(target.OperationName, Is.Not.Null);
		}

		[Test]
		public void SaveCoverCommand_shoult_push_save_command()
		{
			var remoteCover = GetRemoteCover();

			var target = new CoverPreviewViewModel(remoteCover);
			
			var testScheduler = new TestScheduler();
			var mockObservable = new MockObserver<RemoteCover>(testScheduler);

			target.SaveCover.Subscribe(mockObservable);

			target.SaveCoverCommand.Execute();

			Assert.That(mockObservable[0].Value.Value, Is.EqualTo(remoteCover));
		}

		private RemoteCover GetRemoteCover()
		{
			return new RemoteCover(
				"UnitTestImage",
				new Uri(CoverUri, UriKind.Relative),
				new Size(500, 500));
		}
	}
}