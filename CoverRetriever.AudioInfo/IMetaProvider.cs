namespace CoverRetriever.AudioInfo
{
	/// <summary>
	/// Contract to provide audio file summary
	/// </summary>
	public interface IMetaProvider
	{
		/// <summary>
		/// Indicate is Meta Data empty
		/// </summary>
		bool IsEmpty { get; }

		/// <summary>
		/// Get an album name
		/// </summary>
		/// <returns></returns>
		string GetAlbum();

		/// <summary>
		/// Get an artist
		/// </summary>
		/// <returns></returns>
		string GetArtist();

		/// <summary>
		/// Get year of album
		/// </summary>
		/// <returns></returns>
		string GetYear();

		/// <summary>
		/// Get name  of track
		/// </summary>
		/// <returns></returns>
		string GetTrackName();
	}
}