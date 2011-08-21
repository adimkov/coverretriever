using System;
using System.Linq;
using CoverRetriever.AudioInfo;
using CoverRetriever.Model;
using CoverRetriever.Service;
using CoverRetriever.ViewModel;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CoverRetriever.Test.ViewModel
{
    [TestFixture]
    public class FileConductorViewModelTest : ViewModelMock
    {
        [Test]
        public void Should_save_cover_into_directory()
        {
            var target = new FileConductorViewModel();
            var directoryCoverOrganizerMock = GetDirectoryCoverOrganizerMock();
            directoryCoverOrganizerMock.Setup(x => x.IsCoverExists()).Returns(true);
            var frameCoverOrganizer = GetFrameCoverOrganizerMock();
            frameCoverOrganizer.Setup(x => x.IsCoverExists()).Returns(true);
            target.Recipient = CoverRecipient.Directory;
            target.SelectedAudio = GetAudioMockedFile(frameCoverOrganizer.As<IMetaProvider>().Object,
                                            (DirectoryCoverOrganizer)directoryCoverOrganizerMock.Object);

            target.SaveCover(GetRemoteCoverStub()).Run();

            directoryCoverOrganizerMock.Verify(x => x.SaveCover(It.IsAny<Cover>()), Times.Once());
            frameCoverOrganizer.Verify(x => x.SaveCover(It.IsAny<Cover>()), Times.Never());

        }

        [Test]
        public void Should_save_cover_into_audio_frame()
        {
            var target = new FileConductorViewModel();
            var directoryCoverOrganizerMock = GetDirectoryCoverOrganizerMock();
            directoryCoverOrganizerMock.Setup(x => x.IsCoverExists()).Returns(true);
            var frameCoverOrganizer = GetFrameCoverOrganizerMock();
            frameCoverOrganizer.Setup(x => x.IsCoverExists()).Returns(true);
            target.Recipient = CoverRecipient.Frame;
            target.SelectedAudio = GetAudioMockedFile(
                frameCoverOrganizer.As<IMetaProvider>().Object,
                (DirectoryCoverOrganizer)directoryCoverOrganizerMock.Object);

            target.SaveCover(GetRemoteCoverStub()).Run();

            directoryCoverOrganizerMock.Verify(x => x.SaveCover(It.IsAny<Cover>()), Times.Never());
            frameCoverOrganizer.Verify(x => x.SaveCover(It.IsAny<Cover>()), Times.Once());
        }

        [Test]
        public void Should_save_cover_in_all_frames_in_the_current_directory()
        {
            var target = new FileConductorViewModel();
            var directoryCoverOrganizerMock = GetDirectoryCoverOrganizerMock();
            directoryCoverOrganizerMock.Setup(x => x.IsCoverExists()).Returns(true);
            var frameCoverOrganizer = GetFrameCoverOrganizerMock();
            frameCoverOrganizer.Setup(x => x.IsCoverExists()).Returns(true);
            target.Recipient = CoverRecipient.Frame;
            target.ApplyToAllFiles = true;

            target.SelectedAudio = GetMockedFolderWithAudioFile(frameCoverOrganizer.As<IMetaProvider>().Object,
                                            (DirectoryCoverOrganizer)directoryCoverOrganizerMock.Object);

            target.SaveCover(GetRemoteCoverStub()).Run();

            directoryCoverOrganizerMock.Verify(x => x.SaveCover(It.IsAny<Cover>()), Times.Never());
            frameCoverOrganizer.Verify(x => x.SaveCover(It.IsAny<Cover>()), Times.Exactly(4));
        }

        [Test]
        public void Should_set_SelectedAudioCover_regarding_recipient_as_Directory()
        {
            var cover = new Cover();
            var target = new FileConductorViewModel();
            var directoryCoverOrganizerMock = GetDirectoryCoverOrganizerMock();
            directoryCoverOrganizerMock.Setup(x => x.IsCoverExists()).Returns(true);
            directoryCoverOrganizerMock.Setup(x => x.GetCover()).Returns(cover);
            var frameCoverOrganizer = GetFrameCoverOrganizerMock();
            frameCoverOrganizer.Setup(x => x.IsCoverExists()).Returns(true);
            
            target.SelectedAudio = GetAudioMockedFile(
                frameCoverOrganizer.As<IMetaProvider>().Object,
                (DirectoryCoverOrganizer)directoryCoverOrganizerMock.Object);

            target.Recipient = CoverRecipient.Directory;
            
            directoryCoverOrganizerMock.Verify(x => x.GetCover(), Times.Once());
            frameCoverOrganizer.Verify(x => x.GetCover(), Times.Never());
        }

        [Test]
        public void Should_set_SelectedAudioCover_regarding_recipient_as_Frame()
        {
            var cover = new Cover();
            var fileConductorViewModel = new FileConductorViewModel();
            var directoryCoverOrganizerMock = GetDirectoryCoverOrganizerMock();
            directoryCoverOrganizerMock.Setup(x => x.IsCoverExists()).Returns(true);
            var frameCoverOrganizer = GetFrameCoverOrganizerMock();
            frameCoverOrganizer.Setup(x => x.GetCover()).Returns(cover);
            frameCoverOrganizer.Setup(x => x.IsCoverExists()).Returns(true);
            
            fileConductorViewModel.SelectedAudio = GetAudioMockedFile(
                frameCoverOrganizer.As<IMetaProvider>().Object,
                (DirectoryCoverOrganizer)directoryCoverOrganizerMock.Object);

            fileConductorViewModel.Recipient = CoverRecipient.Frame;

            directoryCoverOrganizerMock.Verify(x => x.GetCover(), Times.Once());
            frameCoverOrganizer.Verify(x => x.GetCover(), Times.Once());
            Assert.That(fileConductorViewModel.SelectedAudioCover, Is.EqualTo(cover));
        }

        [Test]
        public void SelectedAudioCover_should_be_updatet_after_modify_SelectedAudio_or_Recipient_properties()
        {
            var fileConductorViewModel = new FileConductorViewModel();
            var directoryCoverOrganizerMock = GetDirectoryCoverOrganizerMock();
            directoryCoverOrganizerMock.Setup(x => x.IsCoverExists()).Returns(true);
            var frameCoverOrganizer = GetFrameCoverOrganizerMock();
            frameCoverOrganizer.Setup(x => x.IsCoverExists()).Returns(true);
            
            fileConductorViewModel.SelectedAudio = GetAudioMockedFile(
                frameCoverOrganizer.As<IMetaProvider>().Object,
                (DirectoryCoverOrganizer)directoryCoverOrganizerMock.Object);

            directoryCoverOrganizerMock.Verify(x => x.GetCover(), Times.Once());
            fileConductorViewModel.Recipient = CoverRecipient.Frame;
            frameCoverOrganizer.Verify(x => x.GetCover(), Times.Once());
        }

        [Test]
        public void SelectedAudioCover_should_be_set_in_null_when_cover_does_not_exists()
        {
            var fileConductorViewModel = new FileConductorViewModel();
            var directoryCoverOrganizerMock = GetDirectoryCoverOrganizerMock();
            directoryCoverOrganizerMock.Setup(x => x.IsCoverExists()).Returns(false);
            var frameCoverOrganizer = GetFrameCoverOrganizerMock();
            frameCoverOrganizer.Setup(x => x.IsCoverExists()).Returns(false);
            
            fileConductorViewModel.Recipient = CoverRecipient.Directory;

            fileConductorViewModel.SelectedAudio = GetAudioMockedFile(
                frameCoverOrganizer.As<IMetaProvider>().Object,
                (DirectoryCoverOrganizer)directoryCoverOrganizerMock.Object);

            fileConductorViewModel.SelectedAudioCover.Should().BeNull();
        }

        [Test]
        public void SelectedAudioCover_should_be_set_in_null_when_SelectedAudio_set_in_null()
        {
            var fileConductorViewModel = new FileConductorViewModel();
            fileConductorViewModel.Recipient = CoverRecipient.Directory;

            fileConductorViewModel.SelectedAudio = null;

            fileConductorViewModel.SelectedAudioCover.Should().BeNull();
        }

        private Mock<ICoverOrganizer> GetDirectoryCoverOrganizerMock()
        {
            var mock = new Mock<DirectoryCoverOrganizer>()
                .As<ICoverOrganizer>();

            return SetupEmptyCallResult(mock);
        }

        private Mock<ICoverOrganizer> GetFrameCoverOrganizerMock()
        {
            var mock = new Mock<ICoverOrganizer>();

            return SetupEmptyCallResult(mock);
        }

        private Mock<ICoverOrganizer> SetupEmptyCallResult(Mock<ICoverOrganizer> mock)
        {
            mock.Setup(x => x.SaveCover(It.IsAny<Cover>())).Returns(Observable.Empty<Unit>);
            return mock;
        }
    }
}