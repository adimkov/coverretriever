using System.Collections.Generic;
using System.Linq;

using CoverRetriever.Model;
using CoverRetriever.Service;

using Microsoft.Practices.ServiceLocation;

using Moq;

using NUnit.Framework;

namespace CoverRetriever.Test.Service
{
    using System.Concurrency;
    using System.Configuration;
    using System.Reactive.Testing.Mocks;

    [TestFixture]
    public class FileSystemServiceTest
    {
        private readonly string _ddtFolder;

        IEnumerable<string> _catalogContent = new[]
        {
            "AcDc - 1975 - High Voltage (Australia Only)",
            "AcDc - 1976 - Dirty Deeds Done Dirt Cheap",
            "AcDc - 1976 - High Voltage",
            "AcDc - 1976 - T.N.T. (Australia only)",
            "AcDc - 1976-Dirty Deeds Done Dirt Cheap (Australia Only)",
            "AcDc - 1977 - Let There Be Rock",
            "AcDc - 1977-Let There Be Rock (Australia Only)",
            "AcDc - 1978 - If You Want Blood You've Got It",
            "AcDc - 1978 - Powerage",
            "AcDc - 1979 - Highway To Hell",
            "AcDc - 1980 - Back In Black",
            "AcDc - 1981 - For Those About To Rock We Salute You",
            "AcDc - 1983 - Flick Of The Switch",
            "AcDc - 1984 - '74 Jailbreak",
            "AcDc - 1985 - Fly On The Wall",
            "AcDc - 1986 - Who Made Who",
            "AcDc - 1988 - Blow Up Your Video",
            "AcDc - 1990 - The Razors Edge",
            "AcDc - 1992 - Live",
            "AcDc - 1992 - Live (Special Collector's Edition)",
            "AcDc - 1995 - Ballbreaker",
            "AcDc - 1997 - Bonfire",
            "AcDc - 2000 - Stiff Upper Lip"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemServiceTest"/> class.
        /// </summary>
        public FileSystemServiceTest()
        {
            _ddtFolder = ConfigurationManager.AppSettings["test.folder.music"];
        }

        /// <summary>
        /// This test depend from environment
        /// </summary>
        [Test]
        public void GetFileSystemItems_should_return_tree_of_file_system_items()
        {
            var rootFolder = new RootFolder(_ddtFolder);
            var coverOrganizerMock = new Mock<DirectoryCoverOrganizer>();
            
            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock.Setup(x => x.GetInstance<DirectoryCoverOrganizer>())
                .Returns(coverOrganizerMock.Object);

            var target = new FileSystemService(serviceLocatorMock.Object);
            target.FillRootFolderAsync(rootFolder, null).Run();

            Assert.That(rootFolder.Children.Select(x => x.Name), Is.EqualTo(_catalogContent));
            rootFolder.Children.Take(23).ForEach(x => Assert.That(x, Is.InstanceOf<Folder>()));
        }

        /// <summary>
        /// This test depend from environment
        /// </summary>
        [Test]
        public void GetFileSystemItems_recursive_load_all_items_and_subfolders()
        {
            var rootFolder = new RootFolder(_ddtFolder);
            var coverOrganizerMock = new Mock<DirectoryCoverOrganizer>();

            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock.Setup(x => x.GetInstance<DirectoryCoverOrganizer>())
                .Returns(coverOrganizerMock.Object);

            var target = new FileSystemService(serviceLocatorMock.Object);
            target.FillRootFolderAsync(rootFolder, null).Run();

            Assert.That(rootFolder.Children.Select(x => x.Name), Is.EqualTo(_catalogContent));
            Assert.That(((Folder)rootFolder.Children[0]).Children.Count, Is.EqualTo(8));
            Assert.That(((Folder)rootFolder.Children[1]).Children.Count, Is.EqualTo(9));
            Assert.That(((Folder)rootFolder.Children[2]).Children.Count, Is.EqualTo(9));
            Assert.That(((Folder)rootFolder.Children[3]).Children.Count, Is.EqualTo(9));
            Assert.That(((Folder)rootFolder.Children[4]).Children.Count, Is.EqualTo(10));
            Assert.That(((Folder)rootFolder.Children[5]).Children.Count, Is.EqualTo(8));
            Assert.That(((Folder)rootFolder.Children[6]).Children.Count, Is.EqualTo(8));
            Assert.That(((Folder)rootFolder.Children[7]).Children.Count, Is.EqualTo(10));
            Assert.That(((Folder)rootFolder.Children[8]).Children.Count, Is.EqualTo(9));
            Assert.That(((Folder)rootFolder.Children[9]).Children.Count, Is.EqualTo(10));
        }

        [Test]
        public void Should_push_32_folders_in_operation_progress_and_one_complete()
        {
            var rootFolder = new RootFolder(_ddtFolder);
            var coverOrganizerMock = new Mock<DirectoryCoverOrganizer>();
            var scheduler = new TestScheduler();
            var testObserver = new MockObserver<string>(scheduler);

            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock.Setup(x => x.GetInstance<DirectoryCoverOrganizer>())
                .Returns(coverOrganizerMock.Object);

            var target = new FileSystemService(serviceLocatorMock.Object);
            target.FillRootFolderAsync(rootFolder, null).Run(testObserver);

            var countOfOnNextMessages = testObserver.Count(x => x.Value.Kind == NotificationKind.OnNext);
            var countOfOnCompleteMessages = testObserver.Count(x => x.Value.Kind == NotificationKind.OnCompleted);
            var countOfOnErrorMessages = testObserver.Count(x => x.Value.Kind == NotificationKind.OnError);
            
            Assert.That(testObserver.Count, Is.EqualTo(32));
            Assert.That(countOfOnNextMessages, Is.EqualTo(31));
            Assert.That(countOfOnCompleteMessages, Is.EqualTo(1));
            Assert.That(countOfOnErrorMessages, Is.EqualTo(0));
        }
    }
}