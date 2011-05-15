
using System;
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