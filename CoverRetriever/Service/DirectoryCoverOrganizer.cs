using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
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

		public Cover GetCover()
		{
			if (IsCoverExists())
			{
				return PrepareCover(GetCoverPath());
			}
			throw new InvalidOperationException("File of cover doesn't exists");
		}

		/// <summary>
		/// Indicate the cover existence
		/// </summary>
		/// <returns><see cref="True"/> if cover exists</returns>
		public bool IsCoverExists()
		{
			return File.Exists(GetCoverPath());
		}

		/// <summary>
		/// Save stream into cover
		/// </summary>
		/// <param name="cover">Cover to save</param>
		public IObservable<Unit> SaveCover(Cover cover)
		{
			if (IsCoverExists())
			{
				File.Delete(GetCoverPath());
			}

			var ext = Path.GetExtension(cover.Name);
			CoverName = DefaultCoverName + ext;
			var newCoverPath = Path.Combine(GetBasePath(), CoverName);

			var coverSaver = cover.CoverStream.Do(
				stream =>
					{
						try
						{
							using (var newCoverStream = File.Open(newCoverPath, FileMode.CreateNew, FileAccess.Write))
							{
								var buffer = new byte[BufferSize];
								int readed;

								do
								{
									readed = stream.Read(buffer, 0, BufferSize);
									newCoverStream.Write(buffer, 0, readed);
								} while (readed != 0);
								newCoverStream.Flush();
							}
						}
						finally
						{
							stream.Dispose();
						}
					});

			return coverSaver.Select(x => new Unit());
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

		private Cover PrepareCover(string coverFullPath)
		{
			var bitmapImage = new BitmapImage(new Uri(coverFullPath, UriKind.Relative));
			var ms = new MemoryStream();
			
			using (var coverStream = File.OpenRead(coverFullPath))
			{
				ms.SetLength(coverStream.Length);
				coverStream.Read(ms.GetBuffer(), 0, (int)coverStream.Length);
				ms.Flush();	
			}

			return new Cover(
				Path.GetFileName(coverFullPath),
				new Size(bitmapImage.PixelWidth, bitmapImage.PixelHeight), 
				ms.Length,
				Observable.Return(ms));
		}
	}
}