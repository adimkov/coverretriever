using System.Collections.Generic;
using System.Linq;
using System.Threading;

using CoverRetriever.Model;
using CoverRetriever.Service;

using Microsoft.Practices.ServiceLocation;

using Moq;

using NUnit.Framework;

namespace CoverRetriever.Test.Service
{
	[TestFixture]
	public class FileSystemServiceTest
	{
		IEnumerable<string> _catalogContent = new[]
		{
			"[1982] - ������ �� ������", "[1983] - ����������",
			"[1984] - ���������",
			"[1985] - �����",
			"[1988] - � ������� ��� ����",
			"[1990] - ��������",
			"[1991] - �������",
			"[1992] - ������� �����",
			"[1993] - ������ �� ���������",
			"[1994] - ��� ���",
			"[1996] - ������",
			"[1997] - �������� � ����",
			"[1999] - ��� ����� ����",
			"[1999] - �����������",
			"[2000] - ������ �������",
			"[2001] - ��� ��������. ���������. �. ������",
			"[2003] - �����������",
			"[2005] - ��������� ��� �����",
			"[2006] - Family CD1",
			"[2006] - Family CD2",
			"[2007] - ���������� ������",
			"���� ������ & ���������� �������� - L'Echoppe",
			"2. �����.mp3",
			"putin.mp3"
		};

		/// <summary>
		/// This test depend from environment
		/// </summary>
		[Test]
		public void GetFileSystemItems_should_return_tree_of_file_system_items()
		{
			var manualResetEvent = new ManualResetEvent(false);
			var basePath = @"g:\������\���\";
			var rootFolder = new RootFolder(basePath);
			var coverOrganizerMock = new Mock<DirectoryCoverOrganizer>();
			
			var serviceLocatorMock = new Mock<IServiceLocator>();
			serviceLocatorMock.Setup(x => x.GetInstance<DirectoryCoverOrganizer>())
				.Returns(coverOrganizerMock.Object);

			var target = new FileSystemService(serviceLocatorMock.Object);
			target.FillRootFolderAsync(rootFolder, null, () => manualResetEvent.Set());

			manualResetEvent.WaitOne();
			Assert.That(rootFolder.Children.Select(x => x.Name), Is.SubsetOf(_catalogContent));
			rootFolder.Children.Take(22).ForEach(x => Assert.That(x, Is.InstanceOf<Folder>()));
			rootFolder.Children.Skip(22).ForEach(x => Assert.That(x, Is.InstanceOf<AudioFile>()));
		}

		/// <summary>
		/// This test depend from environment
		/// </summary>
		[Test]
		public void GetFileSystemItems_recursive_load_all_items_and_subfolders()
		{
			var manuelResetEvent = new ManualResetEvent(false);
			var basePath = @"g:\������\���\";
			var rootFolder = new RootFolder(basePath);
			var coverOrganizerMock = new Mock<DirectoryCoverOrganizer>();

			var serviceLocatorMock = new Mock<IServiceLocator>();
			serviceLocatorMock.Setup(x => x.GetInstance<DirectoryCoverOrganizer>())
				.Returns(coverOrganizerMock.Object);

			var target = new FileSystemService(serviceLocatorMock.Object);
			target.FillRootFolderAsync(rootFolder, null, () => manuelResetEvent.Set());

			manuelResetEvent.WaitOne();
			Assert.That(rootFolder.Children.Select(x => x.Name), Is.SubsetOf(_catalogContent));
			Assert.That(((Folder)rootFolder.Children[0]).Children.Count, Is.EqualTo(15));
			Assert.That(((Folder)rootFolder.Children[1]).Children.Count, Is.EqualTo(9));
			Assert.That(((Folder)rootFolder.Children[2]).Children.Count, Is.EqualTo(8));
			Assert.That(((Folder)rootFolder.Children[3]).Children.Count, Is.EqualTo(8));
		}
	}
}