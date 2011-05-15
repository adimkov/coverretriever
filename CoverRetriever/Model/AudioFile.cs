using System;
using System.Collections.Generic;
using System.Linq;

using CoverRetriever.AudioInfo;
using CoverRetriever.Service;

namespace CoverRetriever.Model
{
	public class AudioFile : FileSystemItem
	{
		private readonly Lazy<IMetaProvider> _metaProvider;
		private readonly IEnumerable<ICoverOrganizer> _coverOrganizer; 

		public AudioFile(string name, FileSystemItem parent, Lazy<IMetaProvider> metaProvider) 
			: base(name, parent)
		{
			Require.NotNull(parent, "parent");
			Require.NotNull(metaProvider, "metaProvider");
			
			_metaProvider = metaProvider;
			_coverOrganizer = new List<ICoverOrganizer>();
		}

		public AudioFile(string name, FileSystemItem parent, Lazy<IMetaProvider> metaProvider, IEnumerable<ICoverOrganizer> coverOrganizer)
			: this(name, parent, metaProvider)
		{
			_coverOrganizer = coverOrganizer;

			_coverOrganizer.ForEach(x => x.Init(this));
		}

		/// <summary>
		/// Get Directory cover organizer
		/// </summary>
		public ICoverOrganizer DirectoryCover
		{
			get { return _coverOrganizer.SingleOrDefault(x => x is DirectoryCoverOrganizer); }
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