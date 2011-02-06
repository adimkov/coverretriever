using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

using CoverRetriever.Model;

namespace CoverRetriever.Service
{
	/// <summary>
	/// Manage cover in the parent folder of audio file
	/// </summary>
	[Export(typeof(ICoverOrganizer))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class DirectoryCoverOrganizer : ICoverOrganizer
	{
		private AudioFile _audioFile;
		private readonly IEnumerable<string> _supportedGraphicsFiles = new[] {".jpg", ".jpeg", ".png", ".bmp"};
		private const string DefaultCoverName = "cover";
		private const int BufferSize = 0x4000;

		/// <summary>
		/// Get found cover name
		/// </summary>
		public string CoverName { get; private set; }
		
		public void Init(AudioFile relatedFile)
		{
			_audioFile = relatedFile;
		}

		/// <summary>
		/// Get cover path.
		/// </summary>
		/// <returns>Image path</returns>
		public string GetCoverFullPath()
		{
			var coverPath = GetCoverPath();
			if (!String.IsNullOrEmpty(coverPath))
			{
				return coverPath;
			}
			throw new InvalidOperationException("File of cover doesn't exists");
		}
		
		/// <summary>
		/// Indicate the cover existence
		/// </summary>
		/// <returns><see cref="True"/> if cover exists</returns>
		public bool IsCoverExists()
		{
			return !String.IsNullOrEmpty(GetCoverPath());
		}

		/// <summary>
		/// Save stream into cover.
		/// Stream will be closed
		/// </summary>
		/// <param name="coverStream">Stream of cover</param>
		/// <param name="name">Name of cover</param>
		public void SaveCover(Stream coverStream, string name)
		{
			if (IsCoverExists())
			{
				File.Delete(GetCoverPath());
			}

			var ext = Path.GetExtension(name);
			CoverName = DefaultCoverName + ext;
			var newCoverPath = Path.Combine(GetBasePath(), CoverName);

			try
			{
				using (var newCoverStream = File.Open(newCoverPath, FileMode.CreateNew, FileAccess.Write))
				{
					var buffer = new byte[BufferSize];
					int readed;

					do
					{
						readed = coverStream.Read(buffer, 0, BufferSize);
						newCoverStream.Write(buffer, 0, readed);
					}
					while (readed != 0);
					newCoverStream.Flush();
				}
			}
			finally
			{
				coverStream.Dispose();	
			}
		}

		/// <summary>
		/// Get cover path
		/// </summary>
		/// <returns></returns>
		private string GetCoverPath()
		{
			if (String.IsNullOrEmpty(CoverName))
			{
				var imagesFilder = Directory.GetFiles(GetBasePath())
					.Where(x => _supportedGraphicsFiles.Contains(Path.GetExtension(x).ToLower()));

				var imageAsCoverFirst = imagesFilder.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == DefaultCoverName);

				if (imageAsCoverFirst != null)
				{
					CoverName = Path.GetFileName(imageAsCoverFirst);
				}
//				else
//				{
//					var firstImage = imagesFilder.FirstOrDefault();
//					if (firstImage != null)
//					{
//						CoverName = Path.GetFileName(firstImage);
//					}	
//				}
			}
			return !String.IsNullOrEmpty(CoverName)? Path.Combine(GetBasePath(), CoverName):String.Empty;
		}

		/// <summary>
		/// Get parent full path
		/// </summary>
		/// <returns></returns>
		private string GetBasePath()
		{
			return FileSystemItem.GetBasePath(_audioFile.Parent);
		}
	}
}