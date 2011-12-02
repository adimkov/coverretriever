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

    [TestFixture]
    public class CoverRetrieverViewModelTest : ViewModelMock
    {
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
            var fileSystemService = GetFileSystemServiceMock();
            var coverRetrieverService = GetCoverRetrieverServiceMock();
            var fileConductorViewModelMock = GetFileConductorViewModelMock();

            var target = new CoverRetrieverViewModel(
                fileSystemService.Object,
                coverRetrieverService.Object,
                GetRootFolderViewModelMock().Object,
                fileConductorViewModelMock.Object);

            Assert.That(target.LoadedCommand, Is.Not.Null);
            Assert.That(target.FileSystemSelectedItemChangedCommand, Is.Not.Null);
            Assert.That(target.PreviewCoverCommand, Is.Not.Null);
            Assert.That(target.SaveCoverCommand, Is.Not.Null);
            
        }

        [Test, Ignore]
        public void LoadedCommand_should_request_for_select_folder()
        {
            bool isRequestedForSelectFolder = false;
            var fileSystemService = GetFileSystemServiceMock();
            var coverRetrieverService = GetCoverRetrieverServiceMock();
            var fileConductorViewModelMock = GetFileConductorViewModelMock();

            var target = new CoverRetrieverViewModel(
                fileSystemService.Object,
                coverRetrieverService.Object,
                GetRootFolderViewModelMock().Object,
                fileConductorViewModelMock.Object); 
            
            target.SelectRootFolderRequest.Raised += (sender, args) => isRequestedForSelectFolder = true;

            target.LoadedCommand.Execute();
            Assert.IsTrue(isRequestedForSelectFolder);
        }

        [Test]
        public void FileSystemSelectedItemChangedCommand_should_set_FileDetails()
        {
            var fileSystemService = GetFileSystemServiceMock();
            var fileConductorViewModelMock = GetFileConductorViewModelMock();
            fileConductorViewModelMock.VerifySet(x => x.SelectedAudio, Times.AtMost(2));
            var coverRetrieverService = GetCoverRetrieverServiceMock();

            var mettaProvider = new Mock<IMetaProvider>();
            var root = GetAudioMockedFile(mettaProvider.Object).Parent;

            var target = new CoverRetrieverViewModel(
                fileSystemService.Object,
                coverRetrieverService.Object,
                GetRootFolderViewModelMock().Object,
                fileConductorViewModelMock.Object);			

            target.FileSystemSelectedItemChangedCommand.Execute(root);
        }

        [Test, Ignore]
        public void FileSystemSelectedItemChangedCommand_should_set_sugested_cover()
        {
            var eventCounter = 0;
            var fileSystemService = GetFileSystemServiceMock();
            var fileConductorViewModelMock = GetFileConductorViewModelMock();
            fileConductorViewModelMock.SetupAllProperties();
            
            var coverRetrieverService = GetCoverRetrieverServiceMock();
            coverRetrieverService.Setup(x => x.GetCoverFor(It.IsAny<string>(), It.IsAny<string>(), 5))
                .Returns(Observable.Empty<IEnumerable<RemoteCover>>);

            var mettaProvider = new Mock<IMetaProvider>();
            var root = GetAudioMockedFile(mettaProvider.Object);

            var target = new CoverRetrieverViewModel(
                fileSystemService.Object,
                coverRetrieverService.Object,
                GetRootFolderViewModelMock().Object,
                fileConductorViewModelMock.Object); 

            target.PropertyChanged += (sender, args) => eventCounter++;

            target.FileSystemSelectedItemChangedCommand.Execute(root);

            coverRetrieverService.Verify(x => x.GetCoverFor(It.IsAny<string>(), It.IsAny<string>(), 5), Times.Once());
            Assert.That(eventCounter, Is.EqualTo(3));
        }

        [Test]
        public void SavingCoverResult_should_push_begin_on_starting_saving_image()
        {
            var testScheduler = new TestScheduler();
            var mockObserver = new MockObserver<ProcessResult>(testScheduler);
            var fileSystemService = GetFileSystemServiceMock();
            var coverRetrieverService = GetCoverRetrieverServiceMock();
            var fileConductorViewModelMock = GetFileConductorViewModelMock();

            fileConductorViewModelMock.SetupGet(x => x.SelectedAudio).Returns(GetAudioFileForSaveIntoDirectoryStub());
            fileConductorViewModelMock.Setup(x => x.SaveCover(It.IsAny<RemoteCover>())).Returns(Observable.Empty<Unit>());

            var target = new Mock<CoverRetrieverViewModel>(
                fileSystemService.Object, 
                coverRetrieverService.Object, 
                GetRootFolderViewModelMock().Object, 
                fileConductorViewModelMock.Object);

            target.Object.SavingCoverResult.Subscribe(mockObserver);

            target.Object.SaveCoverCommand.Execute(GetRemoteCoverStub());

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
            var fileConductorViewModelMock = GetFileConductorViewModelMock();

            fileConductorViewModelMock.SetupGet(x => x.SelectedAudio).Returns(GetAudioFileForSaveIntoDirectoryStub());
            fileConductorViewModelMock.Setup(x => x.SaveCover(It.IsAny<RemoteCover>())).Returns(Observable.Empty<Unit>());

            var target = new Mock<CoverRetrieverViewModel>(
                fileSystemService.Object,
                coverRetrieverService.Object,
                GetRootFolderViewModelMock().Object,
                fileConductorViewModelMock.Object);

            target.Object.SavingCoverResult.Subscribe(mockObserver);

            target.Object.SaveCoverCommand.Execute(GetRemoteCoverStub());

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
            var fileConductorViewModelMock = GetFileConductorViewModelMock();

            fileConductorViewModelMock.SetupGet(x => x.SelectedAudio).Returns(GetAudioFileForSaveIntoDirectoryStub());
            fileConductorViewModelMock.Setup(x => x.SaveCover(It.IsAny<RemoteCover>())).Returns(Observable.Throw<Unit>(new Exception()));

            var target = new Mock<CoverRetrieverViewModel>(
                fileSystemService.Object,
                coverRetrieverService.Object,
                GetRootFolderViewModelMock().Object,
                fileConductorViewModelMock.Object);

            target.Object.SavingCoverResult.Subscribe(mockObserver);

            target.Object.SaveCoverCommand.Execute(GetRemoteCoverStub());

            coverRetrieverService.VerifyAll();

            Assert.That(mockObserver[1].Value.Value, Is.EqualTo(ProcessResult.Done));
            Assert.IsNotNullOrEmpty(target.Object.CoverRetrieveErrorMessage);
        }
    }
}