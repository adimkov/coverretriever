using System;
using System.ComponentModel.Composition;
using TagLib;
using File = TagLib.File;


namespace CoverRetriever.AudioInfo
{
	[Export("mp3", typeof(IMetaProvider))]
	[PartCreationPolicy(CreationPolicy.NonShared)]	
	public class Mp3MetaProvider : AudioFileMetaProvider
	{
		static Mp3MetaProvider()
		{
			TagLib.Id3v1.Tag.DefaultStringHandler = new AutoStringHandler();
			TagLib.Id3v2.Tag.DefaultEncoding = StringType.Latin1;
			ByteVector.UseBrokenLatin1Behavior = true;
		}

		[Obsolete("Added for MEF compatibility")]
		public Mp3MetaProvider()
		{
		}

		public Mp3MetaProvider(string fileName)
		{
			Activate(fileName);
		}

		/// <summary>
		/// Indicate is Meta Data empty
		/// </summary>
		public override bool IsEmpty
		{
			get
			{
				EnsureInstanceWasNotDisposed();
				return File.GetTag(TagTypes.Id3v2).IsEmpty &&
					File.GetTag(TagTypes.Id3v1).IsEmpty;
			}
		}

		/// <summary>
		/// Get an album name
		/// </summary>
		/// <returns></returns>
		public override string GetAlbum()
		{
			EnsureInstanceWasNotDisposed();
			var id3 = GetAudioTag(File, x => !String.IsNullOrEmpty(x.Album));
			return id3.Album ?? base.GetAlbum();
		}

		/// <summary>
		/// Get an artist
		/// </summary>
		/// <returns></returns>
		public override string GetArtist()
		{
			EnsureInstanceWasNotDisposed();
			var id3 = GetAudioTag(File, x => !String.IsNullOrEmpty(x.FirstArtist));
			return id3.FirstArtist ?? base.GetArtist();
		}

		/// <summary>
		/// Get name  of track
		/// </summary>
		/// <returns></returns>
		public override string GetTrackName()
		{
			EnsureInstanceWasNotDisposed();
			var id3 = GetAudioTag(File, x => !String.IsNullOrEmpty(x.Title));
			return id3.Title ?? base.GetTrackName();
		}

		/// <summary>
		/// Get year of album
		/// </summary>
		/// <returns></returns>
		public override string GetYear()
		{
			EnsureInstanceWasNotDisposed();
			var id3 = GetAudioTag(File, x => x.Year > 0);
			return id3.Year > 0 ? id3.Year.ToString() : base.GetYear();
		}

		/// <summary>
		/// Get tab from file.
		/// <remarks>
		///	IDv2 has highest priority.
		/// </remarks>
		/// </summary>
		/// <param name="file"></param>
		/// <param name="isCorrect">Validator for ñheck tag correct. If tag is invalid, try to get another tag</param>
		/// <returns></returns>
		private Tag GetAudioTag(File file, Func<Tag,bool> isCorrect)
		{
			if ((file.TagTypesOnDisk & TagTypes.Id3v2) == TagTypes.Id3v2 && 
				isCorrect(file.GetTag(TagTypes.Id3v2)))
			{
				return file.GetTag(TagTypes.Id3v2);
			}
			return file.GetTag(TagTypes.Id3v1);
		}
	}
}