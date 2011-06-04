using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using TagLib;
using Size = System.Windows.Size;

namespace CoverRetriever.AudioInfo.Helper
{
	/// <summary>
	/// Utility class provide helper methods to FramePicture
	/// </summary>
	public static class PictureHelper
	{
		private static Dictionary<string, string> _mimeTipeDictionary = new Dictionary<string, string>
		{
			{".jpeg", "image/jpeg"},
			{".jpg", "image/jpeg"},
			{".jpe", "image/jpeg"},
			{".png", "image/png"},
			{".bmp", "image/bmp"},
			{".gif", "image/gif"},
		};

		/// <summary>
		/// Get MimeType by file extension
		/// </summary>
		/// <param name="fileExtension"></param>
		/// <returns></returns>
		public static string GetMimeTipeFromFileExtension(string fileExtension)
		{
			if (_mimeTipeDictionary.ContainsKey(fileExtension))
			{
				return _mimeTipeDictionary[fileExtension];
			}
			throw new MetaProviderException("MimeType for extension '{0}' was not found");
		}

		/// <summary>
		/// Replace pictures in Tag file
		/// </summary>
		/// <param name="tag">Mate tag</param>
		/// <param name="newPictures">new pictures</param>
		public static void ReplacePictures(this Tag tag, params IPicture[] newPictures)
		{
			var pictures = new List<IPicture>(tag.Pictures);
			
			foreach (var picture in newPictures)
			{
				var localPicture = picture;
				int indexOfPicture = pictures.IndexOf(pictures.SingleOrDefault(x => x.Type == localPicture.Type));
				
				if (indexOfPicture >= 0)
				{
					pictures.RemoveAt(indexOfPicture);
					pictures.Insert(indexOfPicture, localPicture);
				}
				else
				{
					pictures.Add(localPicture);
				}
			}

			tag.Pictures = pictures.ToArray();
		}

		/// <summary>
		/// Prepare front cover for inject into audio file 
		/// </summary>
		/// <param name="coverStream">stream of cover</param>
		/// <param name="name">name of cover</param>
		/// <returns></returns>
		public static IPicture PreparePicture(Stream coverStream, string name, PictureType pictureType)
		{
			var buffer = new byte[1024 * 10];
			var read = 0;
			var downloadedCover = new MemoryStream();

			do
			{
				read = coverStream.Read(buffer, 0, buffer.Length);
				downloadedCover.Write(buffer, 0, read);
			} while (read != 0);
			
			var frontCover = new Picture(new ByteVector(downloadedCover.ToArray(), (int)downloadedCover.Length));
			frontCover.Type = pictureType;
			frontCover.MimeType = GetMimeTipeFromFileExtension(Path.GetExtension(name));

			return frontCover;
		}

		/// <summary>
		/// Prepare cover from picture of tag
		/// </summary>
		/// <param name="picture">Picture from tag</param>
		/// <returns></returns>
		public static Cover PrepareCover(this IPicture picture)
		{
			var name = String.Format("{0}.{1}", picture.Type, picture.MimeType.Split('/')[1]);
			var size = Size.Empty;
			var coverStream = new MemoryStream(picture.Data.Count);

			coverStream.Write(picture.Data.Data, 0, picture.Data.Count);
			coverStream.Flush();
			coverStream.Position = 0;

			using (var bitmap = new Bitmap(coverStream))
			{
				size = new Size(bitmap.Width, bitmap.Height);
			}
			coverStream.Position = 0;
			var cover = new Cover(name, size, coverStream.Length, Observable.Return(coverStream));
			return cover;	
		}

		/// <summary>
		/// Get cover from file if exists
		/// </summary>
		/// <returns></returns>
		public static IPicture GetCoverSafe(this Tag tag, PictureType pictureType)
		{
			return tag.Pictures.SingleOrDefault(x => x.Type == pictureType);
		}
	}
}