using System;
using System.IO;
using System.Linq;
using System.Windows;
using CoverRetriever.AudioInfo;
using CoverRetriever.Model;
using CoverRetriever.Service;
using CoverRetriever.ViewModel;
using Moq;

namespace CoverRetriever.Test.ViewModel
{
	public class ViewModelMock
	{
		protected Mock<IFileSystemService> GetFileSystemServiceMock()
		{
			return new Mock<IFileSystemService>();
		}

		protected Mock<ICoverRetrieverService> GetCoverRetrieverServiceMock()
		{
			return new Mock<ICoverRetrieverService>();
		}

		protected Mock<OpenFolderViewModel> GetRootFolderViewModelMock()
		{
			return new Mock<OpenFolderViewModel>(new Mock<IFileSystemService>().Object);
		}

		protected Mock<FileConductorViewModel> GetFileConductorViewModelMock()
		{
			return new Mock<FileConductorViewModel>();
		}

		protected RemoteCover GetRemoteCoverStub(IObservable<Stream> coverStream)
		{
			return new RemoteCover(
				"123-78asd",
				"cover.png",
				new Size(200, 200),
				new Size(100, 100),
				new Uri("http://www.google.com/"),
				coverStream,
				coverStream);
		}

		protected RemoteCover GetRemoteCoverStub()
		{
			var emptyStream = Observable.Empty<Stream>();
			return new RemoteCover(
				"123-78asd",
				"cover.png",
				new Size(200, 200),
				new Size(100, 100),
				new Uri("http://www.google.com/"),
				emptyStream,
				emptyStream);
		}

		protected AudioFile GetAudioFileForSaveIntoDirectoryStub()
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

		protected AudioFile GetAudioMockedFile(IMetaProvider metaProvider, DirectoryCoverOrganizer directoryCoverOrganizer)
		{
			return new AudioFile(
				"UnitTest",
				new FileSystemItem("Parent"),
				new Lazy<IMetaProvider>(() => metaProvider),
				directoryCoverOrganizer);
		}

		protected AudioFile GetAudioMockedFile(IMetaProvider metaProvider)
		{
			return new AudioFile(
				"UnitTest",
				new FileSystemItem("Parent"),
				new Lazy<IMetaProvider>(() => metaProvider));
		}
	}
}