using System.Diagnostics;
using System.IO;


namespace CoverRetriever.Model
{
	/// <summary>
	/// File or folder
	/// </summary>
	[DebuggerDisplay("FileName={Name}")]
	public class FileSystemItem
	{
		public FileSystemItem(string name)
		{
			Name = name;
		}
		public FileSystemItem(string name, FileSystemItem parent)
		{
			Require.NotNull(parent, "parent");

			Name = name;
			Parent = parent;
		}
		
		public string Name { get; private set; }
		public FileSystemItem Parent { get; private set; }

		/// <summary>
		/// Get file system item full path
		/// </summary>
		/// <returns></returns>
		public virtual string GetFileSystemItemFullPath()
		{
			return GetBasePath(this);
		}

		public static string GetBasePath(FileSystemItem fileSystemItem)
		{
			var path = fileSystemItem.Name;
			if (fileSystemItem.Parent != null)
			{
				path = Path.Combine(GetBasePath(fileSystemItem.Parent), path);
			}

			return path;
		}
	}
}