using System;

using CoverRetriever.Model;

namespace CoverRetriever.Service
{
	public interface IVersionControlService
	{
		/// <summary>
		/// Get latest version description
		/// </summary>
		/// <returns></returns>
		IObservable<RevisionVersion> GetLatestVersion();
	}
}