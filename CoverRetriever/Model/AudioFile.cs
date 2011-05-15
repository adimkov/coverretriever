using System;

using CoverRetriever.AudioInfo;
using CoverRetriever.Service;

namespace CoverRetriever.Model
{
	public class AudioFile : FileSystemItem
	{
		private readonly Lazy<IMetaProvider> _metaProvider;
		private readonly DirectoryCoverOrganizer _directoryCoverOrganizer; 

		public AudioFile(string name, FileSystemItem parent, Lazy<IMetaProvider> metaProvider) 
			: base(name, parent)
		{
			Require.NotNull(parent, "parent");
			Require.NotNull(metaProvider, "metaProvider");
			
			_metaProvider = metaProvider;
		}

		public AudioFile(string name, FileSystemItem parent, Lazy<IMetaProvider> metaProvider, DirectoryCoverOrganizer directoryCoverOrganizer)
			: this(name, parent, metaProvider)
		{
			_directoryCoverOrganizer = directoryCoverOrganizer;
		}

		/// <summary>
		/// Get Directory cover organizer
		/// </summary>
		public ICoverOrganizer DirectoryCover
		{
			get { return _directoryCoverOrganizer; }
		}

		public string Artist
		{
			get
			{
				return _metaProvider.Value.GetArtist();
			}
		}

		public string Album
		{
			get
			{
				return _metaProvider.Value.GetAlbum();
			}
		}

		public string Year
		{
			get { return _metaProvider.Value.GetYear(); }
		}

		public string TrackName
		{
			get
			{
				return _metaProvider.Value.GetTrackName();
			}
		}
	}
}