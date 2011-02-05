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
	public class DirectoryCoverOrganizer : ICoverOrganizer
	{
		private AudioFile _audioFile;
		private readonly IEnumerable<string> _supportedGraphicsFiles = new[] {".jpg", ".jpeg", ".png", ".bmp"};
		private const string DefaultCoverName = "cover";
		
		/// <summary>
		/// Get found cover name
		/// </summary>
		public string CoverName { get; private set; }
		
		public void Init(AudioFile relatedFile)
		{
			_audioFile = relatedFile;
		}

		public Stream GetCoverStream()
		{
			throw new NotImplementedException();
		}
		
		public bool IsCoverExists()
		{
			return !String.IsNullOrEmpty(GetCoverPath());
		}

		public void SaveCover(Stream coverStream, string name)
		{
			throw new NotImplementedException();
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
					.Where(x => _supportedGraphicsFiles.Contains(Path.GetExtension(x)));

				var imageAsCoverFirst = imagesFilder.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == DefaultCoverName);

				if (imageAsCoverFirst != null)
				{
					CoverName = Path.GetFileName(imageAsCoverFirst);
				}
				else
				{
					var firstImage = imagesFilder.FirstOrDefault();
					if (firstImage != null)
					{
						CoverName = Path.GetFileName(firstImage);
					}	
				}
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