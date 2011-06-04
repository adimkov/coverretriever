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
		/// Get CoverOrganizer for save in folder
		/// </summary>
		public ICoverOrganizer DirectoryCover
		{
			get { return _directoryCoverOrganizer; }
		}

		/// <summary>
		/// Get CoverOrganizer for save in Audio frame
		/// </summary>
		public ICoverOrganizer FrameCover
		{
			get{ return  _metaProvider.Value as ICoverOrganizer;}
		}

		/// <summary>
		/// Get Artist from composition
		/// </summary>
		public string Artist
		{
			get
			{
				return _metaProvider.Value.GetArtist();
			}
		}

		/// <summary>
		/// Get album of composition
		/// </summary>
		public string Album
		{
			get
			{
				return _metaProvider.Value.GetAlbum();
			}
		}

		/// <summary>
		/// Get year of composition
		/// </summary>
		public string Year
		{
			get { return _metaProvider.Value.GetYear(); }
		}

		/// <summary>
		/// Get name of composition
		/// </summary>
		public string TrackName
		{
			get
			{
				return _metaProvider.Value.GetTrackName();
			}
		}
	}
}