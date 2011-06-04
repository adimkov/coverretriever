using System;
using System.Concurrency;
using System.IO;
using System.Linq;
using System.Reactive.Testing.Mocks;
using System.Windows;
using CoverRetriever.AudioInfo;
using CoverRetriever.Model;
using CoverRetriever.Service;
using CoverRetriever.ViewModel;

using Moq;

using NUnit.Framework;

namespace CoverRetriever.Test.ViewModel
{
	[TestFixture]
	public class CoverRetrieverViewModelTest
	{
		[Test]
		public void Ctr_should_not_throws_exception()
		{
			var fileSystemService = GetFileSystemServiceMock();
			var coverRetrieverService = GetCoverRetrieverServiceMock();
			
			Assert.DoesNotThrow(() => new CoverRetrieverViewModel(fileSystemService.Object, coverRetrieverService.Object, GetRootFolderViewModelMock().Object));
		}

		[Test]
		public void Ctr_should_instance_all_commands()
		{
			var fileSystemService = GetFileSystemServiceMock();
			var coverRetrieverService = GetCoverRetrieverServiceMock();

			var target = new CoverRetrieverViewModel(fileSystemService.Object, coverRetrieverService.Object, GetRootFolderViewModelMock().Object);

			Assert.That(target.LoadedCommand, Is.Not.Null);
			Assert.That(target.FileSystemSelectedItemChangedCommand, Is.Not.Null);
			Assert.That(target.PreviewCoverCommand, Is.Not.Null);
			Assert.That(target.SaveCoverCommand, Is.Not.Null);
			
		}

		[Test]
		public void LoadedCommand_should_request_for_select_folder()
		{
			bool isRequestedForSelectFolder = false;
			var fileSystemService = GetFileSystemServiceMock();
			var coverRetrieverService = GetCoverRetrieverServiceMock();

			var target = new CoverRetrieverViewModel(fileSystemService.Object, coverRetrieverService.Object, GetRootFolderViewModelMock().Object);
			target.SelectRootFolderRequest.Raised += (sender, args) => isRequestedForSelectFolder = true;

			target.LoadedCommand.Execute();
			Assert.IsTrue(isRequestedForSelectFolder);
		}

		[Test]
		public void FileSystemSelectedItemChangedCommand_should_set_FileDetails()
		{
			var eventCounter = 0;
			var fileSystemService = GetFileSystemServiceMock();
			var coverRetrieverService = GetCoverRetrieverServiceMock();
			coverRetrieverService.Setup(x => x.GetCoverFor(It.IsAny<string>(), It.IsAny<string>(), 5))
				.Returns(Observable.Return(new System.Collections.Generic.List<RemoteCover>()
				{
					new RemoteCover()
				}))
				.AtMostOnce();
			
			var mettaProvider = new Mock<IMetaProvider>();
			var root = new RootFolder("Root");
			root.Children.Add(new AudioFile("name", root, new Lazy<IMetaProvider>(() => mettaProvider.Object)));

			var target = new CoverRetrieverViewModel(fileSystemService.Object, coverRetrieverService.Object, GetRootFolderViewModelMock().Object);
			
			target.PropertyChanged += (sender, args) => eventCounter++;

			target.FileSystemSelectedItemChangedCommand.Execute(root);

			coverRetrieverService.VerifyAll();
			Assert.That(target.FileDetails, Is.EqualTo(root.Children[0]));
			Assert.That(target.SuggestedCovers.Count, Is.EqualTo(1));
			Assert.That(eventCounter, Is.EqualTo(7));
		}

		[Test]
		public void FileSystemSelectedItemChangedCommand_should_set_sugested_cover()
		{
			var eventCounter = 0;
			var fileSystemService = GetFileSystemServiceMock();
			var coverRetrieverService = GetCoverRetrieverServiceMock();
			coverRetrieverService.Setup(x => x.GetCoverFor(It.IsAny<string>(), It.IsAny<string>(), 5))
				.Returns(Observable.Return(new System.Collections.Generic.List<RemoteCover>
				{
					new RemoteCover()
				}))
				.AtMostOnce();

			var mettaProvider = new Mock<IMetaProvider>();
			var root = new RootFolder("Root");
			root.Children.Add(new AudioFile("name", root, new Lazy<IMetaProvider>(() => mettaProvider.Object)));

			var target = new CoverRetrieverViewModel(fileSystemService.Object, coverRetrieverService.Object, GetRootFolderViewModelMock().Object);

			target.PropertyChanged += (sender, args) => eventCounter++;

			target.FileSystemSelectedItemChangedCommand.Execute(root);

			coverRetrieverService.VerifyAll();
			Assert.That(target.SuggestedCovers.Count, Is.EqualTo(1));
			Assert.That(target.SelectedSuggestedCover, Is.Not.Null);
			Assert.That(eventCounter, Is.EqualTo(7));
		}

		[Test]
		public void SavingCoverResult_should_push_begin_on_starting_saving_image()
		{
			var testScheduler = new TestScheduler();
			var mockObserver = new MockObserver<ProcessResult>(testScheduler);
			var fileSystemService = GetFileSystemServiceMock();
			var coverRetrieverService = GetCoverRetrieverServiceMock();

			var target = new Mock<CoverRetrieverViewModel>(fileSystemService.Object, coverRetrieverService.Object, GetRootFolderViewModelMock().Object);
			target.SetupGet(x => x.FileDetails).Returns(GetSuccessAudioFileStub());

			target.Object.SavingCoverResult.Subscribe(mockObserver);

			target.Object.SaveCoverCommand.Execute(GetMockRemoteCover());

			coverRetrieverService.VerifyAll();

			Assert.That(mockObserver[0].Value.Value, Is.EqualTo(ProcessResult.Begin));
		}

		[Test]
		public void SavingCoverResult_should_push_Done_on_image_has_saved()
		{
			var testScheduler = new TestScheduler();
			var mockObserver = new MockObserver<ProcessResult>(testScheduler);
			var fileSystemService = GetFileSystemServiceMock();
			var coverRetrieverService = GetCoverRetrieverServiceMock();

			var target = new Mock<CoverRetrieverViewModel>(fileSystemService.Object, coverRetrieverService.Object, GetRootFolderViewModelMock().Object);
			target.SetupGet(x => x.FileDetails).Returns(GetSuccessAudioFileStub());

			target.Object.SavingCoverResult.Subscribe(mockObserver);

			target.Object.SaveCoverCommand.Execute(GetMockRemoteCover());

			coverRetrieverService.VerifyAll();

			Assert.That(mockObserver[1].Value.Value, Is.EqualTo(ProcessResult.Done));
		}

		[Test]
		public void SavingCoverResult_should_push_Done_and_error_message_on_image_did_not_saved()
		{
			var testScheduler = new TestScheduler();
			var mockObserver = new MockObserver<ProcessResult>(testScheduler);
			var fileSystemService = GetFileSystemServiceMock();
			var coverRetrieverService = GetCoverRetrieverServiceMock();

			var target = new Mock<CoverRetrieverViewModel>(fileSystemService.Object, coverRetrieverService.Object, GetRootFolderViewModelMock().Object);
			target.SetupGet(x => x.FileDetails).Returns(GetFailureAudioFileStub());

			target.Object.SavingCoverResult.Subscribe(mockObserver);

			target.Object.SaveCoverCommand.Execute(GetMockRemoteCover());

			coverRetrieverService.VerifyAll();

			Assert.That(mockObserver[0].Value.Value, Is.EqualTo(ProcessResult.Begin));
			Assert.That(mockObserver[1].Value.Value, Is.EqualTo(ProcessResult.Done));
			Assert.That(target.Object.CoverRetrieveErrorMessage, Is.EqualTo("FailureSaveFile"));
		}

		[Test]
		public void Should_save_cover_into_directory()
		{
			var fileSystemService = GetFileSystemServiceMock();
			var coverRetrieverService = GetCoverRetrieverServiceMock();
			var directoryCoverOrganizerMock = new Mock<DirectoryCoverOrganizer>().As<ICoverOrganizer>();
			directoryCoverOrganizerMock.Setup(x => x.SaveCover(It.IsAny<Cover>())).Returns(Observable.Empty<Unit>());
			var metaProviderMock = new Mock<IMetaProvider>();
			var coverOrganizer = metaProviderMock.As<ICoverOrganizer>();
			coverOrganizer.Setup(x => x.SaveCover(It.IsAny<Cover>())).Returns(Observable.Empty<Unit>());
			
			var lazyMetaProvider = new Lazy<IMetaProvider>(() => metaProviderMock.Object);
			var audioFile = new AudioFile(
				"foo",
				new FileSystemItem("Parent"), 
				lazyMetaProvider, 
				(DirectoryCoverOrganizer) directoryCoverOrganizerMock.Object);

			var target = new Mock<CoverRetrieverViewModel>(fileSystemService.Object, coverRetrieverService.Object, GetRootFolderViewModelMock().Object);
			target.SetupGet(x => x.FileDetails).Returns(audioFile);
			target.Object.Recipient = CoverRecipient.Folder;
			target.Object.SaveCoverCommand.Execute(GetMockRemoteCover());
			
			directoryCoverOrganizerMock.Verify(x => x.SaveCover(It.IsAny<Cover>()), Times.Once());
			coverOrganizer.Verify(x => x.SaveCover(It.IsAny<Cover>()), Times.Never());
		}

		[Test]
		public void Should_save_cover_into_audio_frame()
		{
			var fileSystemService = GetFileSystemServiceMock();
			var coverRetrieverService = GetCoverRetrieverServiceMock();
			var directoryCoverOrganizerMock = new Mock<DirectoryCoverOrganizer>().As<ICoverOrganizer>();
			directoryCoverOrganizerMock.Setup(x => x.SaveCover(It.IsAny<Cover>())).Returns(Observable.Empty<Unit>());
			var metaProviderMock = new Mock<IMetaProvider>();
			var coverOrganizer = metaProviderMock.As<ICoverOrganizer>();
			coverOrganizer.Setup(x => x.SaveCover(It.IsAny<Cover>())).Returns(Observable.Empty<Unit>());

			var lazyMetaProvider = new Lazy<IMetaProvider>(() => metaProviderMock.Object);
			var audioFile = new AudioFile(
				"foo",
				new FileSystemItem("Parent"),
				lazyMetaProvider,
				(DirectoryCoverOrganizer)directoryCoverOrganizerMock.Object);

			var target = new Mock<CoverRetrieverViewModel>(fileSystemService.Object, coverRetrieverService.Object, GetRootFolderViewModelMock().Object);
			target.SetupGet(x => x.FileDetails).Returns(audioFile);
			target.Object.Recipient = CoverRecipient.Frame;
			target.Object.SaveCoverCommand.Execute(GetMockRemoteCover());

			directoryCoverOrganizerMock.Verify(x => x.SaveCover(It.IsAny<Cover>()), Times.Never());
			coverOrganizer.Verify(x => x.SaveCover(It.IsAny<Cover>()), Times.Once());
		}

		private Mock<IFileSystemService> GetFileSystemServiceMock()
		{
			return new Mock<IFileSystemService>();
		}

		private Mock<ICoverRetrieverService> GetCoverRetrieverServiceMock()
		{
			return new Mock<ICoverRetrieverService>();
		}

		private Mock<OpenFolderViewModel> GetRootFolderViewModelMock()
		{
			return new Mock<OpenFolderViewModel>(new Mock<IFileSystemService>().Object);
		}

		public RemoteCover GetMockRemoteCover()
		{
			return new RemoteCover(
				"123-78asd",
				"cover.png",
				new Size(200,200),
				new Size(100, 100),
				new Uri("http://www.google.com/"), 
				Observable.Empty<Stream>(),
				Observable.Empty<Stream>());
		}

		public AudioFile GetSuccessAudioFileStub()
		{
			var directoryCoverOrganizerMock = new Mock<DirectoryCoverOrganizer>().As<ICoverOrganizer>();
			directoryCoverOrganizerMock.Setup(x => x.SaveCover(It.IsAny<Cover>()))
				.Returns(Observable.Empty<Unit>());

			return new AudioFile(
				"UnitTest", 
				new FileSystemItem("Parent"), 
				new Lazy<IMetaProvider>(() => new Mock<IMetaProvider>().Object),
				(DirectoryCoverOrganizer)directoryCoverOrganizerMock.Object);
		}

		public AudioFile GetFailureAudioFileStub()
		{
			var directoryCoverOrganizerMock = new Mock<DirectoryCoverOrganizer>().As<ICoverOrganizer>();
			directoryCoverOrganizerMock.Setup(x => x.SaveCover(It.IsAny<Cover>()))
				.Returns(Observable.Throw<Unit>(new Exception("FailureSaveFile")));

			return new AudioFile(
				"UnitTest",
				new FileSystemItem("Parent"),
				new Lazy<IMetaProvider>(() => new Mock<IMetaProvider>().Object),
				(DirectoryCoverOrganizer)directoryCoverOrganizerMock.Object);
		}
	}
}