using System.Collections.Generic;

using CoverRetriever.Model;
using CoverRetriever.Service;

using Microsoft.Practices.ServiceLocation;

using Moq;

using NUnit.Framework;

namespace CoverRetriever.Test.Service
{
    using System;
    using System.Concurrency;
    using System.Configuration;
    using System.Linq;
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
            var testTuple = PrepareService();
            var service = testTuple.Item1;
            var rootFolder = testTuple.Item2;

            service.GetChildrenForRootFolder(rootFolder)
                .Run(x => rootFolder.Children.Add(x));

            Assert.That(rootFolder.Children.Select(x => x.Name), Is.EqualTo(_catalogContent));
            Assert.That(rootFolder.Children.Take(23).All(x => x is Folder), Is.True);
        }

        /// <summary>
        /// This test depend from environment
        /// </summary>
        [Test]
        public void GetFileSystemItems_recursive_load_all_items_and_subfolders()
        {
            var testTuple = PrepareService();
            var service = testTuple.Item1;
            var rootFolder = testTuple.Item2;

            service.GetChildrenForRootFolder(rootFolder)
                .Run(x => rootFolder.Children.Add(x));

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
        public void Should_push_23_folders_in_operation_and_one_complete()
        {
            var testTuple = this.PrepareService();
            var scheduler = new TestScheduler();
            var testObserver = new MockObserver<FileSystemItem>(scheduler);
            var service = testTuple.Item1;
            var rootFolder = testTuple.Item2;

            service.GetChildrenForRootFolder(rootFolder).Run(testObserver);

            var countOfOnNextMessages = testObserver.Count(x => x.Value.Kind == NotificationKind.OnNext);
            var countOfOnCompleteMessages = testObserver.Count(x => x.Value.Kind == NotificationKind.OnCompleted);
            var countOfOnErrorMessages = testObserver.Count(x => x.Value.Kind == NotificationKind.OnError);
            
            Assert.That(countOfOnNextMessages, Is.EqualTo(23));
            Assert.That(countOfOnCompleteMessages, Is.EqualTo(1));
            Assert.That(countOfOnErrorMessages, Is.EqualTo(0));
        }

        [Test]
        public void Should_add_294_audio_files_and_31_foders()
        {
            var testTuple = PrepareService();
            var service = testTuple.Item1;
            var rootFolder = testTuple.Item2;
            service.GetChildrenForRootFolder(rootFolder)
                .Run(x => rootFolder.Children.Add(x));

            var audioFiles = ToFlatCollecction<AudioFile>(rootFolder);
            var folders = ToFlatCollecction<Folder>(rootFolder);

            Assert.That(audioFiles.Count(), Is.EqualTo(267));
            Assert.That(folders.Count(), Is.EqualTo(31));
        }

        private Tuple<FileSystemService, RootFolder> PrepareService()
        {
            var rootFolder = new RootFolder(this._ddtFolder);
            var coverOrganizerMock = new Mock<DirectoryCoverOrganizer>();

            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock.Setup(x => x.GetInstance<DirectoryCoverOrganizer>())
                .Returns(coverOrganizerMock.Object);

            var target = new FileSystemService(serviceLocatorMock.Object);
    
            return new Tuple<FileSystemService, RootFolder>(target, rootFolder);
        }

        private IEnumerable<T> ToFlatCollecction<T>(Folder rootFolder)
        {
            var items = new List<T>();

            items.AddRange(rootFolder.Children.OfType<T>());
            foreach (var folrer in rootFolder.Children.OfType<Folder>())
            {
                items.AddRange(ToFlatCollecction<T>(folrer));
            }

            return items;
        }
    }
}