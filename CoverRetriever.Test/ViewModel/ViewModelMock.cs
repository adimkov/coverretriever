using System;
using System.IO;
using System.Windows;
using CoverRetriever.AudioInfo;
using CoverRetriever.Model;
using CoverRetriever.Service;
using CoverRetriever.ViewModel;
using Moq;

namespace CoverRetriever.Test.ViewModel
{
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;

    using CoverRetriever.AudioInfo.Tagger;

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

        /// <summary>
        /// Gets mocked folder with 4 audio files mocked as well
        /// </summary>
        /// <param name="metaProvider"></param>
        /// <param name="directoryCoverOrganizer"></param>
        /// <returns></returns>
        protected AudioFile GetMockedFolderWithAudioFile(IMetaProvider metaProvider, DirectoryCoverOrganizer directoryCoverOrganizer)
        {
            var parent = new RootFolder("root");
            parent.Children.Add(new AudioFile("AudioFile1", parent, new Lazy<IMetaProvider>(() => metaProvider), directoryCoverOrganizer));
            parent.Children.Add(new AudioFile("AudioFile2", parent, new Lazy<IMetaProvider>(() => metaProvider), directoryCoverOrganizer));
            parent.Children.Add(new AudioFile("AudioFile3", parent, new Lazy<IMetaProvider>(() => metaProvider), directoryCoverOrganizer));
            parent.Children.Add(new AudioFile("AudioFile4", parent, new Lazy<IMetaProvider>(() => metaProvider), directoryCoverOrganizer));

            return parent.Children.OfType<AudioFile>().First();
        }

        protected IMetaProvider GetMockedSuggestedTags()
        {
            return new SuggestTag()
                {
                    Album = "test_album",
                    Artist = "test_artist",
                    Year = "2009",
                    TrackName = "test_trackName"
                };
        }
    }
}