namespace CoverRetriever.AudioInfo
{
	/// <summary>
	/// Contract to provide audio file summary
	/// </summary>
	public interface IMetaProvider
	{
		string GetAlbum();
		string GetArtist();
		string GetYear();
		string GetTrackName();
	}
}