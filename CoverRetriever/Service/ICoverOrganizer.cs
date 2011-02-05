using System.IO;

using CoverRetriever.Model;

namespace CoverRetriever.Service
{
	public interface ICoverOrganizer
	{
		/// <summary>
		/// Initialize instance for fork with current file
		/// </summary>
		/// <param name="relatedFile"></param>
		void Init(AudioFile relatedFile);

		/// <summary>
		/// Indicate the cover existence
		/// </summary>
		/// <returns><see cref="True"/> if cover exists</returns>
		bool IsCoverExists();
		
		/// <summary>
		/// Get cover stream
		/// </summary>
		/// <returns>Image stream</returns>
		Stream GetCoverStream();
		
		/// <summary>
		/// Save stream into cover
		/// </summary>
		/// <param name="coverStream">Stream of cover</param>
		/// <param name="name">Name of cover</param>
		void SaveCover(Stream coverStream, string name);
	}
}