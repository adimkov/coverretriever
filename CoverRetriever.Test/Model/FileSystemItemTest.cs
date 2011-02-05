using System;

using CoverRetriever.Model;

using NUnit.Framework;

namespace CoverRetriever.Test.Model
{
	[TestFixture]
	public class FileSystemItemTest : FileSystemItemBaseTest
	{
		[Test]
		public void Ctr_should_return_instance()
		{
			var fileName = "UnitTest";
			var target = new FileSystemItem(fileName, RootFolder);

			Assert.That(target.Name, Is.EqualTo(fileName));
			Assert.That(target.Parent, Is.Not.Null);
		}

		[Test]
		public void Ctr_should_throw_contract_require_exception()
		{
			var fileName = "UnitTest";
			Assert.Throws<ArgumentNullException>(() => new FileSystemItem(fileName, null));
		}

		[Test]
		public void GetFileSystemItemFullPath_should_return_full_path_to_file_system_item()
	    {
			var parentFolder = new FileSystemItem("root", RootFolder);
			var folder = new FileSystemItem("folder", parentFolder);
			
			Assert.That(folder.GetFileSystemItemFullPath(), Is.EqualTo(@"C:\temp\root\folder"));
	    }
	}
}