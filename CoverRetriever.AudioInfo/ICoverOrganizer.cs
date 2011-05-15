using System;

namespace CoverRetriever.AudioInfo
{
	public interface ICoverOrganizer
	{
		/// <summary>
		/// Indicate if can work with cover
		/// </summary>
		bool IsCanProcessed { get; }

		/// <summary>
		/// Indicate the cover existence
		/// </summary>
		/// <returns><see cref="True"/> if cover exists</returns>
		bool IsCoverExists();
		
		/// <summary>
		/// Get cover
		/// </summary>
		/// <returns>Cover info</returns>
		Cover GetCover();
		
		/// <summary>
		/// Save stream into cover
		/// </summary>
		/// <param name="cover">Cover to save</param>
		IObservable<Unit> SaveCover(Cover cover);
	}
}