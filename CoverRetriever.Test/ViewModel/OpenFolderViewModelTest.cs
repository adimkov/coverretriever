using System;

using CoverRetriever.Service;
using CoverRetriever.ViewModel;

using Moq;

using NUnit.Framework;

namespace CoverRetriever.Test.ViewModel
{
	[TestFixture]
	public class OpenFolderViewModelTest
	{
		[Test]
		public void CheckForFolderExists_should_call_file_system_service()
		{
			var fileSystemServiceStub = new Mock<IFileSystemService>();

			fileSystemServiceStub.Setup(x => x.IsDirectoryExists(It.IsAny<string>()))
				.Returns(true)
				.AtMostOnce();

			var target = new OpenFolderViewModel(fileSystemServiceStub.Object);
			target.CheckForFolderExists("TestPath");

			fileSystemServiceStub.VerifyAll();
		}

		[Test]
		public void ConfirmCommand_should_push_next_enable_to_close()
		{
			var isPushed = false;
			var fileSystemServiceStub = new Mock<IFileSystemService>();

			var target = new OpenFolderViewModel(fileSystemServiceStub.Object);

			target.PushRootFolder.Subscribe(x => isPushed = true);
			
			target.ConfirmCommand.Execute("TestPath");

			Assert.That(isPushed, Is.True);
			Assert.That(target.IsCloseEnabled, Is.True);
		}
	}
}