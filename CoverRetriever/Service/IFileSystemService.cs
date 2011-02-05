
using System;
using System.Windows.Threading;

using CoverRetriever.Model;

namespace CoverRetriever.Service
{
	public interface IFileSystemService
	{
		/// <summary>
		/// Recursive loads audio files and catalog list into parent <see cref="Folder"/>
		/// <remarks>
		///	IF dispatcher is null, fill folder in current thread
		/// </remarks>
		/// </summary>
		/// <param name="parent">Parent directory of file system items</param>
		/// <param name="dispatcher">Syncronization context</param>
		/// <param name="onComplete">Complete operation notify</param>
		void FillRootFolderAsync(Folder parent, Dispatcher dispatcher, Action onComplete);
	}
}