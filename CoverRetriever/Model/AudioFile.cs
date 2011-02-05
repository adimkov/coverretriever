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

		public AudioFile(string name, FileSystemItem parent, Lazy<IMetaProvider> metaProvider) 
			: base(name, parent)
		{
			Require.NotNull(parent, "parent");
			Require.NotNull(metaProvider, "metaProvider");
			
			_metaProvider = metaProvider;
			CoverOrganizer = new List<ICoverOrganizer>();
		}

		public AudioFile(string name, FileSystemItem parent, Lazy<IMetaProvider> metaProvider, IEnumerable<ICoverOrganizer> coverOrganizer)
			: this(name, parent, metaProvider)
		{
			CoverOrganizer = coverOrganizer;

			CoverOrganizer.ForEach(x => x.Init(this));
		}

		/// <summary>
		/// Get methods for cover managment
		/// </summary>
		public IEnumerable<ICoverOrganizer> CoverOrganizer { get; private set; }

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