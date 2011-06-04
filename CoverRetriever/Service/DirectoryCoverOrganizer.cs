using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Linq;
using CoverRetriever.AudioInfo;
using Size = System.Windows.Size;

namespace CoverRetriever.Service
{
	/// <summary>
	/// Manage cover in the parent folder of audio file
	/// </summary>
	[Export(typeof(DirectoryCoverOrganizer))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class DirectoryCoverOrganizer : ICoverOrganizer, IActivator
	{
		private string _basePath;
		private readonly IEnumerable<string> _supportedGraphicsFiles = new[] {".jpg", ".jpeg", ".png", ".bmp"};
		private const string DefaultCoverName = "cover";
		private const int BufferSize = 0x4000;

		[Obsolete("Added for MEF compatibility")]
		public DirectoryCoverOrganizer()
		{
		}

		public DirectoryCoverOrganizer(string basePath)
		{
			_basePath = basePath;
		}

		/// <summary>
		/// Get found cover name
		/// </summary>
		public string CoverName { get; private set; }

		/// <summary>
		/// Indicate if can work with cover
		/// </summary>
		public bool IsCanProcessed
		{
			get
			{
				//todo: check for ability to read/write from disk
				return true;
			}
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
			var newCoverPath = Path.Combine(_basePath, CoverName);

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
		/// Activate an object
		/// </summary>
		/// <param name="param"></param>
		public void Activate(params object[] param)
		{
			if (param.Length != 1)
			{
				throw new ArgumentException("Base path of cover was not found", "param");
			}
			_basePath = (string) param[0];
		}

		/// <summary>
		/// Get cover path
		/// </summary>
		/// <returns></returns>
		private string GetCoverPath()
		{
			if (String.IsNullOrEmpty(CoverName))
			{
				var imagesFilder = Directory.GetFiles(_basePath)
					.Where(x => _supportedGraphicsFiles.Contains(Path.GetExtension(x).ToLower()));

				var imageAsCoverFirst = imagesFilder.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).Equals(DefaultCoverName, StringComparison.InvariantCultureIgnoreCase));

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
			return !String.IsNullOrEmpty(CoverName) ? Path.Combine(_basePath, CoverName) : String.Empty;
		}

		private Cover PrepareCover(string coverFullPath)
		{
			var ms = new MemoryStream();
			Size size;
			using (var bitmap = new Bitmap(coverFullPath))
			{
				size = new Size(bitmap.Width, bitmap.Height);
			}

			using (var coverStream = File.OpenRead(coverFullPath))
			{
				ms.SetLength(coverStream.Length);
				coverStream.Read(ms.GetBuffer(), 0, (int)coverStream.Length);
				ms.Flush();	
			}

			return new Cover(
				Path.GetFileName(coverFullPath),
				size, 
				ms.Length,
				Observable.Return(ms));
		}
	}
}