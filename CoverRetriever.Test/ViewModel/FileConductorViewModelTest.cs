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
	public class FileConductorViewModelTest : ViewModelMock
	{
		[Test]
		public void Should_save_cover_into_directory()
		{
			var target = new FileConductorViewModel();
			var directoryORganizerMock = GetDirectoryCoverOrganizerMock();
			var frameCoverOrganizer = GetFrameCoverOrganizerMock();
			
			target.Recipient = CoverRecipient.Directory;
			target.SelectedAudio = GetAudioMockedFile(frameCoverOrganizer.As<IMetaProvider>().Object,
											(DirectoryCoverOrganizer)directoryORganizerMock.Object);

			target.SaveCover(GetRemoteCoverStub());

			directoryORganizerMock.Verify(x => x.SaveCover(It.IsAny<Cover>()), Times.Once());
			frameCoverOrganizer.Verify(x => x.SaveCover(It.IsAny<Cover>()), Times.Never());

		}

		[Test]
		public void Should_save_cover_into_audio_frame()
		{
			var target = new FileConductorViewModel();
			var directoryORganizerMock = GetDirectoryCoverOrganizerMock();
			var frameCoverOrganizer = GetFrameCoverOrganizerMock();

			target.Recipient = CoverRecipient.Frame;
			target.SelectedAudio = GetAudioMockedFile(frameCoverOrganizer.As<IMetaProvider>().Object,
											(DirectoryCoverOrganizer)directoryORganizerMock.Object);

			target.SaveCover(GetRemoteCoverStub());

			directoryORganizerMock.Verify(x => x.SaveCover(It.IsAny<Cover>()), Times.Never());
			frameCoverOrganizer.Verify(x => x.SaveCover(It.IsAny<Cover>()), Times.Once());
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