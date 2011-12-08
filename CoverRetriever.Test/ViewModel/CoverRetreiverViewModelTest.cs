using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Reactive.Testing.Mocks;
using CoverRetriever.AudioInfo;
using CoverRetriever.Model;
using CoverRetriever.ViewModel;

using Moq;

using NUnit.Framework;

namespace CoverRetriever.Test.ViewModel
{
    using System.Linq;
    using System.Threading;

    using CoverRetriever.AudioInfo.Tagger;
    using CoverRetriever.Service;

    [TestFixture]
    public class CoverRetrieverViewModelTest : ViewModelMock
    {
        private TestScheduler _testScheduler;

        [SetUp]
        public void Given()
        {
            _testScheduler = new TestScheduler();
        }

        [Test]
        public void Ctr_should_not_throws_exception()
        {
            var fileSystemService = GetFileSystemServiceMock();
            var coverRetrieverService = GetCoverRetrieverServiceMock();
            var fileConductorViewModelMock = GetFileConductorViewModelMock();
            
            Assert.DoesNotThrow(() => new CoverRetrieverViewModel(
                fileSystemService.Object, 
                coverRetrieverService.Object, 
                GetRootFolderViewModelMock().Object,
                fileConductorViewModelMock.Object));
        }

        [Test]
        public void Ctr_should_instance_all_commands()
        {
            var target = GetCoverRetrieverViewModel();

            Assert.That(target.LoadedCommand, Is.Not.Null);
            Assert.That(target.FileSystemSelectedItemChangedCommand, Is.Not.Null);
            Assert.That(target.PreviewCoverCommand, Is.Not.Null);
            Assert.That(target.SaveCoverCommand, Is.Not.Null);
        }

        [Test]
        public void LoadedCommand_should_request_for_select_folder()
        {
            bool isRequestedForSelectFolder = false;

            var target = GetCoverRetrieverViewModel();
            target.SelectRootFolderRequest.Raised += (sender, args) => isRequestedForSelectFolder = true;

            target.LoadedCommand.Execute(null);
            Assert.IsTrue(isRequestedForSelectFolder);
        }

        [Test]
        public void FileSystemSelectedItemChangedCommand_should_set_FileDetails()
        {
            var target = GetCoverRetrieverViewModel();
            
            var mettaProvider = new Mock<IMetaProvider>();
            var root = GetAudioMockedFile(mettaProvider.Object);

            target.FileSystemSelectedItemChangedCommand.Execute(root);
            Assert.IsNotNull(target.SelectedFileSystemItem);
        }

        [Test]
        public void FileSystemSelectedItemChangedCommand_should_set_sugested_cover()
        {
            var eventCounter = 0;
            var mettaProvider = new Mock<IMetaProvider>();
            var root = GetAudioMockedFile(mettaProvider.Object);
            Mock<IFileSystemService> fileSystemServiceMock;
            Mock<FileConductorViewModel> fileConductorViewModelMock;
            Mock<ICoverRetrieverService> coverRetrieverServiceMock;

            var target = GetCoverRetrieverViewModel(
                out fileSystemServiceMock,
                out fileConductorViewModelMock,
                out coverRetrieverServiceMock);

            target.PropertyChanged += (sender, args) => eventCounter++;

            target.FileSystemSelectedItemChangedCommand.Execute(root);

            coverRetrieverServiceMock.Verify(x => x.GetCoverFor(It.IsAny<string>(), It.IsAny<string>(), 6), Times.Once());
            Assert.That(eventCounter, Is.EqualTo(4));
        }

        [Test]
        public void SavingCoverResult_should_push_begin_on_starting_saving_image()
        {
            var mockObserver = new MockObserver<ProcessResult>(_testScheduler);
            Mock<IFileSystemService> fileSystemServiceMock;
            Mock<FileConductorViewModel> fileConductorViewModelMock;
            Mock<ICoverRetrieverService> coverRetrieverServiceMock;

            var target = GetCoverRetrieverViewModel(
                out fileSystemServiceMock,
                out fileConductorViewModelMock,
                out coverRetrieverServiceMock);

            fileConductorViewModelMock.SetupGet(x => x.SelectedAudio).Returns(GetAudioFileForSaveIntoDirectoryStub());
            fileConductorViewModelMock.Setup(x => x.SaveCover(It.IsAny<RemoteCover>())).Returns(Observable.Empty<Unit>());

            target.SavingCoverResult.Subscribe(mockObserver);

            target.SaveCoverCommand.Execute(GetRemoteCoverStub());

            Assert.That(mockObserver[0].Value.Value, Is.EqualTo(ProcessResult.Begin));
        }

        [Test]
        public void SavingCoverResult_should_push_Done_on_image_has_saved()
        {
            var mockObserver = new MockObserver<ProcessResult>(_testScheduler);
            Mock<IFileSystemService> fileSystemServiceMock;
            Mock<FileConductorViewModel> fileConductorViewModelMock;
            Mock<ICoverRetrieverService> coverRetrieverServiceMock;

            var target = GetCoverRetrieverViewModel(
                out fileSystemServiceMock,
                out fileConductorViewModelMock,
                out coverRetrieverServiceMock);

            fileConductorViewModelMock.SetupGet(x => x.SelectedAudio).Returns(GetAudioFileForSaveIntoDirectoryStub());
            fileConductorViewModelMock.Setup(x => x.SaveCover(It.IsAny<RemoteCover>())).Returns(Observable.Empty<Unit>());

            target.SavingCoverResult.Subscribe(mockObserver);

            target.SaveCoverCommand.Execute(GetRemoteCoverStub());

            Assert.That(mockObserver[1].Value.Value, Is.EqualTo(ProcessResult.Done));
        }

        [Test]
        public void SavingCoverResult_should_push_Done_and_error_message_on_image_did_not_saved()
        {
            var mockObserver = new MockObserver<ProcessResult>(_testScheduler);
            Mock<IFileSystemService> fileSystemServiceMock;
            Mock<FileConductorViewModel> fileConductorViewModelMock;
            Mock<ICoverRetrieverService> coverRetrieverServiceMock;

            var target = GetCoverRetrieverViewModel(
                out fileSystemServiceMock,
                out fileConductorViewModelMock,
                out coverRetrieverServiceMock);

            fileConductorViewModelMock.SetupGet(x => x.SelectedAudio).Returns(GetAudioFileForSaveIntoDirectoryStub());
            fileConductorViewModelMock.Setup(x => x.SaveCover(It.IsAny<RemoteCover>())).Returns(Observable.Throw<Unit>(new Exception()));

            target.SavingCoverResult.Subscribe(mockObserver);

            target.SaveCoverCommand.Execute(GetRemoteCoverStub());

            Assert.That(mockObserver[1].Value.Value, Is.EqualTo(ProcessResult.Done));
            Assert.IsNotNullOrEmpty(target.CoverRetrieverErrorMessage);
        }

        [Test]
        public void Should_enable_GrabTagCommand_for_executing()
        {
            var mettaProvider = new Mock<IMetaProvider>();
            var root = GetAudioMockedFile(mettaProvider.Object);

            var target = GetCoverRetrieverViewModel();

            target.FileSystemSelectedItemChangedCommand.Execute(root);

            Assert.IsTrue(target.GrabTagsCommand.CanExecute(null));
        }

        [Test]
        public void Should_disable_GrabTagCommand_for_executing()
        {
            var root = new FileSystemItem("Root");

            var target = GetCoverRetrieverViewModel();

            target.FileSystemSelectedItemChangedCommand.Execute(root);

            Assert.IsFalse(target.GrabTagsCommand.CanExecute(null));
        }

        [Test]
        public void Should_assign_tagger_to_audio_file()
        {
            var mettaProvider = new Mock<IMetaProvider>();
            var root = GetAudioMockedFile(mettaProvider.Object);
            var taggerMock = new Mock<ITagger>();
            taggerMock.Setup(x => x.LoadTagsForAudioFile(It.Is<string>(s => s == root.GetFileSystemItemFullPath())))
                .Returns(Observable.Return(new Unit()));

            var target = GetCoverRetrieverViewModel();
            target.Tagger = new Lazy<ITagger>(() => taggerMock.Object);
            
            target.FileSystemSelectedItemChangedCommand.Execute(root);
            target.GrabTagsCommand.Execute(null);
            _testScheduler.Run();
            
            Thread.Sleep(100);

            Assert.That(root.MetaProvider, Is.EqualTo(taggerMock.Object));
        }

        [Test]
        public void Should_set_error_message_if_retrieving_tag_failed()
        {
            var mettaProvider = new Mock<IMetaProvider>();
            var root = GetAudioMockedFile(mettaProvider.Object);
            var taggerMock = new Mock<ITagger>();
            taggerMock.Setup(x => x.LoadTagsForAudioFile(It.IsAny<string>()))
                .Returns(Observable.Throw<Unit>(new InvalidOperationException()));

            var target = GetCoverRetrieverViewModel();
            target.Tagger = new Lazy<ITagger>(() => taggerMock.Object);

            target.FileSystemSelectedItemChangedCommand.Execute(root);
            target.GrabTagsCommand.Execute(null);
            _testScheduler.Run();

            Assert.That(target.CoverRetrieverErrorMessage, Is.Not.Empty);
        }

        [Test]
        public void Should_switch_view_model_to_save_tag_mode_in_true_if_success_tag_retrieving()
        {
            var mettaProvider = new Mock<IMetaProvider>();
            var root = GetAudioMockedFile(mettaProvider.Object);
            var taggerMock = new Mock<ITagger>();
            taggerMock.Setup(x => x.LoadTagsForAudioFile(It.Is<string>(s => s == root.GetFileSystemItemFullPath())))
                .Returns(Observable.Return(new Unit()));

            var target = GetCoverRetrieverViewModel();
            target.Tagger = new Lazy<ITagger>(() => taggerMock.Object);

            target.FileSystemSelectedItemChangedCommand.Execute(root);
            target.GrabTagsCommand.Execute(null);
            _testScheduler.Run();

            Thread.Sleep(100);

            Assert.That(target.SaveTagMode, Is.True);
        }

        [Test]
        public void Should_switch_view_model_to_save_tag_mode_in_false_if_tag_retrieving_failed()
        {
            var mettaProvider = new Mock<IMetaProvider>();
            var root = GetAudioMockedFile(mettaProvider.Object);
            var taggerMock = new Mock<ITagger>();
            taggerMock.Setup(x => x.LoadTagsForAudioFile(It.Is<string>(s => s == root.GetFileSystemItemFullPath())))
                .Returns(Observable.Throw<Unit>(new InvalidOperationException()));

            var target = GetCoverRetrieverViewModel();
            target.Tagger = new Lazy<ITagger>(() => taggerMock.Object);

            target.FileSystemSelectedItemChangedCommand.Execute(root);
            target.GrabTagsCommand.Execute(null);
            _testScheduler.Run();

            Thread.Sleep(100);

            Assert.That(target.SaveTagMode, Is.False);
        }

        [Test]
        public void Should_reset_tag_and_set_save_tag_mode_with_false()
        {
            var mettaProvider = new Mock<IMetaProvider>();
            var root = GetAudioMockedFile(mettaProvider.Object);
            var taggerMock = new Mock<ITagger>();
            taggerMock.Setup(x => x.LoadTagsForAudioFile(It.Is<string>(s => s == root.GetFileSystemItemFullPath())))
               .Returns(Observable.Return(new Unit()));

            root.AssignTagger(taggerMock.Object);
            var target = GetCoverRetrieverViewModel();
            target.SaveTagMode = true;

            target.FileSystemSelectedItemChangedCommand.Execute(root);
            target.RejectSuggestedTagCommand.Execute(null);

            Assert.That(target.SaveTagMode, Is.False);
            Assert.That(root.MetaProvider, Is.EqualTo(mettaProvider.Object));
        }


        private CoverRetrieverViewModel GetCoverRetrieverViewModel(
            out Mock<IFileSystemService> fileSystemServiceMock,
            out Mock<FileConductorViewModel> fileConductorViewModelMock,
            out Mock<ICoverRetrieverService> coverRetrieverServiceMock)
        {
            fileSystemServiceMock = GetFileSystemServiceMock();
            fileConductorViewModelMock = GetFileConductorViewModelMock();
            fileConductorViewModelMock.SetupAllProperties();

            coverRetrieverServiceMock = GetCoverRetrieverServiceMock();
            coverRetrieverServiceMock.Setup(x => x.GetCoverFor(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Observable.Empty<IEnumerable<RemoteCover>>);

            var versionControlMock = new Mock<IVersionControlService>();
            versionControlMock.Setup(x => x.GetLatestVersion()).Returns(Observable.Empty<RevisionVersion>);


            
            var target = new CoverRetrieverViewModel(
                fileSystemServiceMock.Object,
                coverRetrieverServiceMock.Object,
                GetRootFolderViewModelMock().Object,
                fileConductorViewModelMock.Object);
            target.ObservableScheduler = _testScheduler;
            target.SubscribeScheduler = _testScheduler;
            target.VersionControl = new Lazy<IVersionControlService>(() => versionControlMock.Object);
            
            return target;
        }

        private CoverRetrieverViewModel GetCoverRetrieverViewModel()
        {
            Mock<IFileSystemService> fileSystemServiceMock;
            Mock<FileConductorViewModel> fileConductorViewModelMock;
            Mock<ICoverRetrieverService> coverRetrieverServiceMock;

            return GetCoverRetrieverViewModel(
                out fileSystemServiceMock, 
                out fileConductorViewModelMock, 
                out coverRetrieverServiceMock);
        }
    }
}