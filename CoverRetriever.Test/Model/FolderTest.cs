using CoverRetriever.Model;

using NUnit.Framework;

namespace CoverRetriever.Test.Model
{
	[TestFixture]
	public class FolderTest : FileSystemItemBaseTest
	{
		[Test]
		public void Ctr_return_instance_of_folder()
		{
			var target = new Folder("folder", RootFolder);

			Assert.That(target.Children, Is.Not.Null);
		}

		[Test]
		public void GetFileSystemItemFullPath_should_return_full_path_to_file_system_item_fith_last_slash()
		{
			var parentFolder = new Folder("root", RootFolder);
			var folder = new Folder("folder", parentFolder);

			Assert.That(folder.GetFileSystemItemFullPath(), Is.EqualTo(@"C:\temp\root\folder\"));
		}
	}
}