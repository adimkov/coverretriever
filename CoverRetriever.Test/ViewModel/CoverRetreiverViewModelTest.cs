using System;
using System.Linq;
using System.Windows.Threading;

using CoverRetriever.AudioInfo;
using CoverRetriever.Model;
using CoverRetriever.Service;
using CoverRetriever.ViewModel;

using Moq;

using NUnit.Framework;

namespace CoverRetriever.Test.ViewModel
{
	[TestFixture]
	public class CoverRetreiverViewModelTest
	{
		[Test]
		public void Ctr_should_not_throws_exception()
		{
			var fileSystemService = GetFileSystemServiceMock();
			var coverRetrieverService = GetCoverRetrieverServiceMock();

			Assert.DoesNotThrow(() => new CoverRetreiverViewModel(fileSystemService.Object, coverRetrieverService.Object));
		}

		[Test]
		public void Ctr_should_instance_all_commands()
		{
			var fileSystemService = GetFileSystemServiceMock();
			var coverRetrieverService = GetCoverRetrieverServiceMock();

			var target = new CoverRetreiverViewModel(fileSystemService.Object, coverRetrieverService.Object);

			Assert.That(target.LoadedCommand, Is.Not.Null);
			Assert.That(target.FileSystemSelectedItemChangedCommand, Is.Not.Null);
			Assert.That(target.PreviewCoverCommand, Is.Not.Null);
			Assert.That(target.SaveCoverCommand, Is.Not.Null);
			
		}

		[Test]
		public void LoadedCommand_should_set_initialize_file_system_collection()
		{
			var fileSystemService = GetFileSystemServiceMock();
			fileSystemService.Setup(x => x.FillRootFolderAsync(It.IsAny<RootFolder>(), It.IsAny<Dispatcher>(), null))
				.AtMostOnce();
			var coverRetrieverService = GetCoverRetrieverServiceMock();

			var target = new CoverRetreiverViewModel(fileSystemService.Object, coverRetrieverService.Object);

			target.LoadedCommand.Execute();
			
			fileSystemService.VerifyAll();
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
			
			var target = new CoverRetreiverViewModel(fileSystemService.Object, coverRetrieverService.Object);
			target.PropertyChanged += (sender, args) => eventCounter++;

			target.FileSystemSelectedItemChangedCommand.Execute(root);

			coverRetrieverService.VerifyAll();
			Assert.That(target.FileDetails, Is.EqualTo(root.Children[0]));
			Assert.That(target.SuggestedCovers.Count, Is.EqualTo(1));
			Assert.That(eventCounter, Is.EqualTo(5));
		}

		private Mock<IFileSystemService> GetFileSystemServiceMock()
		{
			return new Mock<IFileSystemService>();
		}

		private Mock<ICoverRetrieverService> GetCoverRetrieverServiceMock()
		{
			return new Mock<ICoverRetrieverService>();
		}
	}
}