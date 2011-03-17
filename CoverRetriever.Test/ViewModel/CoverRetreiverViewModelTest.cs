using System;
using System.Linq;

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
			Assert.That(eventCounter, Is.EqualTo(6));
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
			Assert.That(eventCounter, Is.EqualTo(6));
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
	}
}