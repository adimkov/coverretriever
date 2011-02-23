using System;
using System.Globalization;
using System.Windows.Media.Imaging;

using CoverRetriever.AudioInfo;
using CoverRetriever.Converter;
using CoverRetriever.Model;

using Moq;

using NUnit.Framework;

namespace CoverRetriever.Test.Converters
{
	[TestFixture]
	public class FolderImageConverterTest
	{
		[Test]
		public void Convert_should_return_generic_folder_path()
		{
			var folder = new RootFolder("Test_Path");
			var folder2 = new Folder("Child_Folder", folder);
			folder2.Children.Add(folder);

			var target = new FolderImageConverter();

			var imageSource = (BitmapImage)target.Convert(folder, null, null, CultureInfo.CurrentCulture);
			Assert.That(imageSource.UriSource.ToString(), Is.EqualTo("/CoverRetriever;component/Assets/ShellFolder.png"));
		}

		[Test]
		public void Convert_should_return_generic_folder_path2()
		{
			var folder = new RootFolder("Test_Path");
			
			var target = new FolderImageConverter();

			var imageSource = (BitmapImage)target.Convert(folder, null, null, CultureInfo.CurrentCulture);
			Assert.That(imageSource.UriSource.ToString(), Is.EqualTo("/CoverRetriever;component/Assets/ShellFolder.png"));
		}

		[Test]
		public void Convert_should_return_audio_folder_folder_path()
		{
			var folder = new RootFolder("Test_Path");
			var mMetaStub = new Mock<IMetaProvider>();
			var file = new AudioFile("Child_Folder.mp3", folder, new Lazy<IMetaProvider>(() => mMetaStub.Object));

			folder.Children.Add(file);

			var target = new FolderImageConverter();

			var imageSource = (BitmapImage)target.Convert(folder, null, null, CultureInfo.CurrentCulture);
			Assert.That(imageSource.UriSource.ToString(), Is.EqualTo("/CoverRetriever;component/Assets/ShellFolder_Audio.png"));
		}
	}
}