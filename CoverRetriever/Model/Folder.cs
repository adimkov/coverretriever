using System.Collections.ObjectModel;
using System.IO;

namespace CoverRetriever.Model
{
	public class Folder : FileSystemItem
	{
		public Folder(string name, FileSystemItem parent) 
			: base(name, parent)
		{
			Children = new ObservableCollection<FileSystemItem>();
		}

		protected Folder(string name)
			: base(name)
		{
			Children = new ObservableCollection<FileSystemItem>();
		}

		public ObservableCollection<FileSystemItem> Children { get; private set; }

		/// <summary>
		/// Get file system item full path
		/// </summary>
		/// <returns></returns>
		public override string GetFileSystemItemFullPath()
		{
			return base.GetFileSystemItemFullPath() + Path.DirectorySeparatorChar;
		}
	}
}